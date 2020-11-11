using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace AngryBots2.Core.Player.Systems {

    // [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    // [UpdateBefore(typeof(BuildPhysicsWorld))]
    public unsafe class PlayerMovementSystem : SystemBase {

        struct CameraInfo {
            public float3 Position;
            public float3 Forward;
            public float3 Right;
        }

        private BuildPhysicsWorld buildPhysicsWorld;
        private CameraInfo* camInfo;

        protected override void OnCreate() {
            buildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
            camInfo = (CameraInfo*)UnsafeUtility.Malloc(
                UnsafeUtility.SizeOf<CameraInfo>(), 
                UnsafeUtility.AlignOf<CameraInfo>(), 
                Allocator.Persistent);
        }

        protected override void OnDestroy() {
            UnsafeUtility.Free(camInfo, Allocator.Persistent);
        }

        protected unsafe override void OnUpdate() {

            float deltaTime = Time.DeltaTime;
            CameraInfo* local = camInfo;

            Entities.WithAll<Camera>().ForEach((in LocalToWorld c0) => {
                local->Position = c0.Position;
                local->Forward  = c0.Forward;
                local->Right    = c0.Right;
            }).WithNativeDisableUnsafePtrRestriction(local).Run();

            ComponentDataFromEntity<PhysicsCollider> colliders = GetComponentDataFromEntity<PhysicsCollider>(true);
            CollisionWorld collisionWorld                      = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
            NativeArray<RigidBody> bodies                      = buildPhysicsWorld.PhysicsWorld.Bodies;

            Entities.ForEach((ref PhysicsVelocity c0, in InputContainer c1, in Speed c2, in Translation c3) => {

                float3 forward = local->Forward;
                float3 right   = local->Right;
                forward.y = right.y = 0;

                float3 dir = math.normalizesafe(forward * c1.Axis.y + right * c1.Axis.x);
                float3 start = c3.Value + new float3(0f, 10f, 0f);
                float3 next  = start + dir;
                next.y = -10f;

                RaycastInput ray = new RaycastInput {
                    Start  = start,
                    End    = next,
                    Filter = new CollisionFilter {
                        BelongsTo    = Filters.RayMask,
                        CollidesWith = Filters.GroundMask | Filters.PropMask,
                        GroupIndex   = 0
                    }
                };

                if (collisionWorld.CastRay(ray, out var hitInfo) && math.length(c1.Axis) > 0) {
                    PhysicsCollider collider = colliders[bodies[hitInfo.RigidBodyIndex].Entity];
                    uint belongsTo = collider.ColliderPtr->Filter.BelongsTo;

                    if ((belongsTo & Filters.GroundMask) > 0) {
                        float3 movement = math.normalizesafe(hitInfo.Position - c3.Value);
                        c0.Linear = movement * deltaTime * c2.Value;
                    } else {
                        c0.Linear = float3.zero;
                    }
                } else {
                    c0.Linear = float3.zero;
                }
            }).WithNativeDisableUnsafePtrRestriction(local).Run();
        }
    }
}

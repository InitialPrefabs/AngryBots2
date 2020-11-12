using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace AngryBots2.Core.Transforms.Systems {

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public unsafe class PlayerRotationSystem : SystemBase {

        private UnityEngine.Plane plane;
        private float3* intersection;

        protected override void OnCreate() {
            plane = new Plane(Vector3.up, Vector3.zero);

            intersection = (float3*)UnsafeUtility.Malloc(
                UnsafeUtility.SizeOf<float3>(), 
                UnsafeUtility.AlignOf<float3>(), 
                Allocator.Persistent);
        }

        protected override void OnDestroy() {
            UnsafeUtility.Free(intersection, Allocator.Persistent);
        }

        protected override void OnUpdate() {
            Vector2 mousePosition = Input.mousePosition;
            float3* inter = intersection;

            Entities.ForEach((Camera c0) => {
                Ray ray = c0.ScreenPointToRay(mousePosition);
                plane.Raycast(ray, out float dist);
                *inter = ray.GetPoint(dist);
            }).WithoutBurst().WithNativeDisableUnsafePtrRestriction(inter).Run();

            Entities.WithAll<InputContainer>().ForEach((ref Rotation c0, in Translation c1) => {
                float3 direction = math.normalizesafe(*inter - c1.Value);
                direction.y = 0;

                c0.Value = quaternion.LookRotation(direction, new float3(0, 1, 0));
            }).WithNativeDisableUnsafePtrRestriction(inter).Run();
        }
    }
}

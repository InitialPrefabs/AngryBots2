using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AngryBots2.Core.Transforms.Systems {

    public unsafe class CameraMovementSystem : SystemBase {

        private float3* playerPosition;

        protected override void OnCreate() {
            playerPosition = (float3*)UnsafeUtility.Malloc(
                UnsafeUtility.SizeOf<float3>(), 
                UnsafeUtility.AlignOf<float3>(), 
                Allocator.Persistent);
        }

        protected override void OnDestroy() {
            UnsafeUtility.Free(playerPosition, Allocator.Persistent);;
        }

        protected override void OnUpdate() {
            float3* position = playerPosition;
            Entities.WithAll<InputContainer>().ForEach((in Translation c0) => {
                *position = c0.Value;
            }).WithNativeDisableUnsafePtrRestriction(position).Run();

            Entities.WithAll<Camera>().ForEach((ref Translation c0, in VectorOffset c1) => {
                c0.Value = *position - c1.Value;
            }).WithNativeDisableUnsafePtrRestriction(position).Run();
        }
    }
}

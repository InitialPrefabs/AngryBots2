using Unity.Entities;
using Unity.Mathematics;

namespace AngryBots2.Core.Transforms {

    public struct VectorOffset : IComponentData {
        public float3 Value;
    }
}
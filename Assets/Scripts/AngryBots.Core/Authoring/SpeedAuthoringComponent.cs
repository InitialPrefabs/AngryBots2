using Unity.Entities;
using UnityEngine;

namespace AngryBots2.Core.Transforms {

    public class SpeedAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity {

        public float Speed = 20f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            dstManager.AddComponentData(entity, new Speed { Value = Speed });
        }
    }
}

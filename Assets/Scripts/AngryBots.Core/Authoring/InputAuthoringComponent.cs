using Unity.Entities;
using UnityEngine;

namespace AngryBots2.Core.Inputs {

    public class InputAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            dstManager.AddComponentData(entity, new InputContainer { });
        }
    }
}
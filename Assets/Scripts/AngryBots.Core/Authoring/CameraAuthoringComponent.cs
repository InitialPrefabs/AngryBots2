using Unity.Entities;
using UnityEngine;

namespace AngryBots2.Core {

    [RequireComponent(typeof(Camera))]
    public class CameraAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            Camera cam = GetComponent<Camera>();
            conversionSystem.AddHybridComponent(cam);
        }
    }
}
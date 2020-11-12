using AngryBots2.Core.Transforms;
using Unity.Entities;
using UnityEngine;

namespace AngryBots2.Core {

    [RequireComponent(typeof(Camera))]
    public class CameraAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity {

        public Transform Player;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            Camera cam = GetComponent<Camera>();
            conversionSystem.AddHybridComponent(cam);

            Vector3 distance = Player.position - transform.position;
            dstManager.AddComponentData(entity, new VectorOffset {
                Value = distance
            });
        }
    }
}

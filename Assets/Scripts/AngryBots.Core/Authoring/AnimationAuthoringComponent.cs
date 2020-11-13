using Unity.Entities;
using Unity.Animation.Hybrid;
using UnityEngine;

namespace AngryBots2.Core.Animations {

#if UNITY_EDITOR
    // TODO: Figure out how this works on an editor version
    [ConverterVersion("AnimationAuthoringComponent", 1)]
    public class AnimationAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity {

        public AnimationClip Clip;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            if (Clip == null) {
                return;
            }

            conversionSystem.DeclareAssetDependency(gameObject, Clip);

            dstManager.AddComponentData(entity, new DefaultClip {
                Value = conversionSystem.BlobAssetStore.GetClip(Clip)
            });

            dstManager.AddComponentData(entity, new DeltaTime { });
        }
    }
#endif
}

using Unity.Animation;
using Unity.DataFlowGraph;
using Unity.Entities;

namespace AngryBots2.Core.Animations.Systems {

    [UpdateInGroup(typeof(DefaultAnimationSystemGroup))]
    public class DeltaTimeSystem : SystemBase {
        protected override void OnUpdate() {

            var deltaTime = World.Time.DeltaTime;
            Entities.ForEach((Entity entity, ref DeltaTime c0) => {
                c0.Value = deltaTime;
            }).Run();
        }
    }

    [UpdateBefore(typeof(DefaultAnimationSystemGroup))]
    public class PlayClipSystem : SystemBase {

        private ProcessDefaultAnimationGraph graphSystem;
        private EntityQuery animationQuery;
        private EntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate() {
            base.OnCreate();

            graphSystem = World.GetOrCreateSystem<ProcessDefaultAnimationGraph>();
            graphSystem.AddRef();
            commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            animationQuery = GetEntityQuery(new EntityQueryDesc {
                None = new [] { ComponentType.ReadOnly<DefaultClip>() },
                All  = new [] { ComponentType.ReadOnly<ClipState>() }
            });

            graphSystem.Set.RendererModel = NodeSet.RenderExecutionModel.Islands;
        }

        protected override void OnDestroy() {
            if (graphSystem == null) {
                return;
            }

            graphSystem.RemoveRef();
            base.OnDestroy();
        }

        protected override void OnUpdate() {
            CompleteDependency();
            var commandBuffer = commandBufferSystem.CreateCommandBuffer();

            Entities.
                WithNone<ClipState>().
                WithoutBurst().
                WithStructuralChanges().
                ForEach((Entity entity, ref Rig rig, ref DefaultClip animation) => {
                    var state = AnimationExtensions.CreateGraph(entity, graphSystem, ref rig, ref animation);
                    commandBuffer.AddComponent(entity, state);
                }).Run();

            Entities.
                WithChangeFilter<DefaultClip>().
                WithoutBurst().ForEach((Entity entity, ref DefaultClip animation, ref ClipState clipState) => {
                    graphSystem.Set.SendMessage(
                        clipState.ClipPlayerNode, 
                        ClipPlayerNode.SimulationPorts.Clip, 
                        animation.Value);
                }).Run();

            Entities.
                WithNone<DefaultClip>().
                WithoutBurst().
                WithStructuralChanges().ForEach((Entity entity, ref ClipState state) => {
                    graphSystem.Dispose(state.Graph);
                }).Run();

            if (animationQuery.CalculateEntityCount() > 0) {
                commandBuffer.RemoveComponent<ClipState>(animationQuery);
            }
        }
    }
}

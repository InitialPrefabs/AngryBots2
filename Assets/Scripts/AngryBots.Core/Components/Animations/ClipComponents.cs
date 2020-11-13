using Unity.Animation;
using Unity.DataFlowGraph;
using Unity.Entities;

namespace AngryBots2.Core.Animations {

    public class ConvertDeltaTimeToFloatNode : ConvertToBase<
                                               ConvertDeltaTimeToFloatNode, 
                                               DeltaTime, 
                                               float, 
                                               ConvertDeltaTimeToFloatNode.Kernel> {

        // TODO: Add the BurstCompileTag
        public struct Kernel : IGraphKernel<KernelData, KernelDefs> {
            public void Execute(RenderContext ctx, KernelData data, ref KernelDefs ports) {
                ctx.Resolve(ref ports.Output) = ctx.Resolve(ports.Input).Value;
            }
        }
    }

    public static class AnimationExtensions {

        public static ClipState CreateGraph(
            Entity entity, 
            ProcessDefaultAnimationGraph graphSystem,
            ref Rig rig,
            ref DefaultClip clipState) {

            GraphHandle graph = graphSystem.CreateGraph();

            var data = new ClipState {
                Graph = graph,
                ClipPlayerNode = graphSystem.CreateNode<ClipPlayerNode>(graph)
            };

            var deltaTimeNode = graphSystem.CreateNode<ConvertDeltaTimeToFloatNode>(graph);
            var entityNode    = graphSystem.CreateNode(graph, entity);

            var set = graphSystem.Set;

            set.Connect(entityNode, deltaTimeNode, ConvertDeltaTimeToFloatNode.KernelPorts.Input);
            set.Connect(
                deltaTimeNode, 
                ConvertDeltaTimeToFloatNode.KernelPorts.Output, 
                data.ClipPlayerNode, 
                ClipPlayerNode.KernelPorts.DeltaTime);
            set.Connect(
                data.ClipPlayerNode, 
                ClipPlayerNode.KernelPorts.Output, 
                entityNode, 
                NodeSetAPI.ConnectionType.Feedback);

            set.SetData(data.ClipPlayerNode, ClipPlayerNode.KernelPorts.Speed, 1.0f);
            set.SendMessage(
                data.ClipPlayerNode, 
                ClipPlayerNode.SimulationPorts.Configuration, 
                new ClipConfiguration { Mask = ClipConfigurationMask.LoopTime });

            set.SendMessage(data.ClipPlayerNode, ClipPlayerNode.SimulationPorts.Rig, rig);
            set.SendMessage(data.ClipPlayerNode, ClipPlayerNode.SimulationPorts.Clip, clipState.Value);

            return data;
        }
    }

    public struct DeltaTime : IComponentData {
        public float Value;
    }

    public struct DefaultClip : IComponentData {
        public BlobAssetReference<Clip> Value;
    }

    public struct ClipState : ISystemStateComponentData {
        public GraphHandle Graph;
        public NodeHandle<ClipPlayerNode> ClipPlayerNode;
    }
}

using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Prototype.ECS.Runtime
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    public partial class PrototypeBeforePhysicsSystemGroup : ComponentSystemGroup
    {

    }

    [UpdateAfter(typeof(PhysicsSystemGroup))]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class PrototypeAfterPhysicsSystemGroup : ComponentSystemGroup
    {

    }

    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PrototypeBeforeTransformSystemGroup : ComponentSystemGroup
    {

    }

    [UpdateAfter(typeof(TransformSystemGroup))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PrototypeAfterTransformSystemGroup : ComponentSystemGroup
    {

    }
   
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PrototypeLevelGeneratorSystemGroup : ComponentSystemGroup
    {

    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PrototypeLevelSystemGroup : ComponentSystemGroup
    {

    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PrototypeDebugSystemGroup : ComponentSystemGroup
    {

    }

    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    public partial class AIVariableRateGroupSystem : ComponentSystemGroup
    {
        public AIVariableRateGroupSystem()
        {
            RateManager = new RateUtils.VariableRateManager(200, true);
        }
    }
}

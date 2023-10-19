using Unity.Entities;
using Unity.Physics.Systems;

namespace Unity.Physics.Stateful
{

    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(StatefulCollisionEventBufferSystem))]
    public partial class StatefulCollisionSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(StatefulTriggerEventBufferSystem))]
    public partial class StatefuTriggerSystemGroup : ComponentSystemGroup { }
}

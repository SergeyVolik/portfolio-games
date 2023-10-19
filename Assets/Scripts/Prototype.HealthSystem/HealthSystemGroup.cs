using Unity.Entities;

namespace Prototype.HealthSystem
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class HealthSystemGroup : ComponentSystemGroup { }
}

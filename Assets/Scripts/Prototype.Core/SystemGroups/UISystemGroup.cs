using Unity.Entities;

namespace Prototype.ECS.UI
{
    [UpdateAfter(typeof(BindSystemGroup))]
    public partial class UISystemGroup : ComponentSystemGroup { }
}


using Unity.Entities;
using Unity.Transforms;

namespace Prototype
{

    [UpdateInGroup(typeof(BindSystemGroup))]
    public partial class CopyGameObjectPositionToEntitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            //TODO: implement copy pos from GO to Entity
        }
    }
}
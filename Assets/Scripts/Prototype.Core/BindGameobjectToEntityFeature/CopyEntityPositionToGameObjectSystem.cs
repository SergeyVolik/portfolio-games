using Unity.Entities;
using Unity.Transforms;

namespace Prototype
{

    [UpdateInGroup(typeof(BindSystemGroup))]
    public partial class CopyEntityPositionToGameObjectSystem : SystemBase
    {
        protected override void OnUpdate()
        {

            foreach (var (comp, e) in SystemAPI.Query<BindedGameObjectsToEntityC>().WithNone<Disabled>().WithEntityAccess())
            {

                if (!SystemAPI.HasComponent<LocalToWorld>(comp.entity))
                    continue;

                var ltw = SystemAPI.GetComponentRO<LocalToWorld>(comp.entity);

                foreach (var item in comp.instances)
                {
                    item.transform.position = ltw.ValueRO.Position;
                    item.transform.rotation = ltw.ValueRO.Rotation;
                }

            }
        }
    }
}
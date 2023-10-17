using Unity.Entities;
using UnityEngine;

namespace SV.BallGame
{
    [DisallowMultipleComponent]
    public class DestroyableObjAuthoring : MonoBehaviour
    {
        void OnEnable() { }

        class Baker : Baker<DestroyableObjAuthoring>
        {
            public override void Bake(DestroyableObjAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new DestroyableObjC { });
            }
        }
    }

    public struct DestroyableObjC : IComponentData { }


}

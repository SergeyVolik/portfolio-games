using Unity.Entities;
using UnityEngine;

namespace Prototype.ECS.UI
{
    /// <summary>
    /// Bake to <see cref="UILookAtCameraC"> component.
    /// </summary>
    public class UILookAtCameraAuthroing : MonoBehaviour
    {
        public Transform value;

        class Baker : Baker<UILookAtCameraAuthroing>
        {
            public override void Bake(UILookAtCameraAuthroing authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponentObject<UILookAtCameraC>(entity, new UILookAtCameraC { value = authoring.value });
            }
        }
    }

    /// <summary>
    /// input for <see cref="UILookAtCameraSystem">.
    /// </summary>
    public class UILookAtCameraC : IComponentData
    {
        public Transform value;
    }
}
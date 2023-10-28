using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Prototype
{
    public class InCameraViewCheckAuthoring : MonoBehaviour
    {
        private void OnEnable() { }
        class Baker : Baker<InCameraViewCheckAuthoring>
        {
            public override void Bake(InCameraViewCheckAuthoring authoring)
            {

                if (authoring.enabled == false)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<InCameraViewTag>(entity);
                AddComponent<InCameraViewUpdateTag>(entity);
                SetComponentEnabled<InCameraViewTag>(entity, false);               
            }
        }
    }

    public struct InCameraViewUpdateTag : IComponentData { }

    public struct InCameraViewTag : IComponentData, IEnableableComponent { }

    public partial class InCameraViewChekcerSystem : SystemBase
    {
        private Camera m_Camera;

        protected override void OnUpdate()
        {
            if(!m_Camera)
                m_Camera = Camera.main;

            if (!m_Camera)
                return;

            float3 cameraVector = m_Camera.transform.forward;
            float3 cameraPos = m_Camera.transform.position;

            foreach (var (item, e) in SystemAPI.Query<RefRO<LocalToWorld>>().WithAll<InCameraViewUpdateTag>().WithEntityAccess())
            {
                var vectorToObject = math.normalize(item.ValueRO.Position - cameraPos);

                var dot = math.dot(vectorToObject, cameraVector);

                
                var dotResult = dot > 0.92f;
                SystemAPI.SetComponentEnabled<InCameraViewTag>(e, dotResult);
                
            }

        }
    }
}

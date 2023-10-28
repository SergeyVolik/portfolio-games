using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Prototype
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class SyncCameraTargetSystem : SystemBase
    {    
        protected override void OnCreate()
        {
            RequireForUpdate<CameraTarget>();
            base.OnCreate();
        }

        protected override void OnUpdate()
        {

            if (!CameraTargetGO.Instance)
                return;

            foreach (var target in SystemAPI.Query<CameraTarget>())
            {
                if (SystemAPI.HasComponent<LocalToWorld>(target.entity))
                {
                    var worldPos = SystemAPI.GetComponentRO<LocalToWorld>(target.entity).ValueRO.Position;
                    CameraTargetGO.Instance.transform.position = worldPos;

                    if (CameraTargetGO.Instance.brain.m_UpdateMethod != Cinemachine.CinemachineBrain.UpdateMethod.ManualUpdate)
                    {
                        CameraTargetGO.Instance.brain.ManualUpdate();
                    }
                }              
            }
        }
    }
}
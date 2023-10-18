using Unity.Entities;

namespace Prototype
{
    /// <summary>
    /// Vibration for mobile devices
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class VibrationTriggerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();           

            EntityManager.AddComponent<TriggerVibrationEvent>(SystemHandle);
            EntityManager.SetComponentEnabled<TriggerVibrationEvent>(SystemHandle, false);

            RequireForUpdate<TriggerVibrationEvent>();
        }
        protected override void OnUpdate()
        {

            foreach (var (tr, e) in SystemAPI.Query<TriggerVibrationEvent>().WithEntityAccess())
            {
#if UNITY_AINDROID || UNITY_IPHONE
                Handheld.Vibrate();
#endif
            }
            
        }
    }

    /// <summary>
    /// Input for system <see cref="VibrationTriggerSystem">
    /// </summary>
    [GenerateCleaUpDisableSystem]
    public struct TriggerVibrationEvent : IComponentData, IEnableableComponent { }

}

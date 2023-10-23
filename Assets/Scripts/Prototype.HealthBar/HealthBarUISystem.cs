using Prototype.ECS.UI;
using Prototype.HealthSystem;
using Unity.Entities;

namespace Prototype.HealthBar
{
    [UpdateInGroup(typeof(UISystemGroup))]
    public partial class UpdateHealthBarUISystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (ui, healthC, e) in SystemAPI.Query<
                SystemAPI.ManagedAPI.UnityEngineComponent<HealthBarUI>,
                RefRO<HealthC>>().WithChangeFilter<HealthC>().WithEntityAccess())
            {               
                ui.Value.UpdateHealth(healthC.ValueRO.health, healthC.ValueRO.healthMax);
            }
        }
    }
}
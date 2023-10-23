using Unity.Entities;

namespace Prototype.HealthSystem
{
    /// <summary>
    /// Ignore damage if an attaker has the same owner
    /// </summary>
    [UpdateInGroup(typeof(DamageFiltrationGroup))]
    public partial struct OwnerDamageFiltationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {

            foreach (var (receiveDamageBuffer, owner, entity) in SystemAPI
                        .Query<DynamicBuffer<ReceiveDamageB>, OwnerC>().WithEntityAccess())
            {

                for (int i = 0; i < receiveDamageBuffer.Length; i++)
                {
                    var item = receiveDamageBuffer[i];
                    if (SystemAPI.HasComponent<OwnerC>(item.attacker))
                    {
                        var ownerAttacker = SystemAPI.GetComponentRO<OwnerC>(item.attacker);

                        if (ownerAttacker.ValueRO.value == owner.value)
                        {
                            receiveDamageBuffer.RemoveAtSwapBack(i);
                            i--;
                        }
                    }
                }
            }
        }
    }
}

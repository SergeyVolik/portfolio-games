using DOTS.Dispatcher.Runtime;
using Unity.Entities;

namespace Prototype.HealthSystem
{
    public struct IsDeadTagC : IComponentData, IEnableableComponent
    {
    }

    [GenerateCleaUpDisableSystem]
    public struct SendDamageB : IBufferElementData, IEnableableComponent
    {
        public Entity attacker;
        public Entity receiver;
    }

    [GenerateCleaUpDisableSystem]
    public struct ReceiveDamageB : IBufferElementData, IEnableableComponent
    {
        public int damage;
        public Entity attacker;
    }

    [GenerateCleaUpDisableSystem]
    public struct DeadEventC : IComponentData, IEnableableComponent, IDisableableECSEvent
    {
    }
    public struct HealthRegenC : IComponentData
    {
        public float hpPerSec;
    }

    public struct HealthRegenProcessingC : IComponentData
    {
        public float delta;
    }

    public struct HasFullHpT : IComponentData, IEnableableComponent
    {
    }
    
    [System.Serializable]
    public struct HealthC : IComponentData, IEnableableComponent, IDisableableECSEvent
    {
        public int health;
        public int healthMax;

        public void SetFullHp()
        {
            health = healthMax;
        }
        
        public bool IsFullHp() => health == healthMax;
    }
}
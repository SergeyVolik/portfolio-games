using Unity.Entities;
using UnityEngine;

namespace Prototype
{
    public struct MovementC : IComponentData
    {
        public bool isMoving;
    }
    
    public struct UnitAllyT: IComponentData
    {
    }
    
    public struct UnitEnemyT: IComponentData
    {
    }
    
    public struct UnitResourceT: IComponentData
    {
    }
    
    public struct UnitAttackAnimationT : IComponentData, IEnableableComponent
    {
    }
    
    public struct UnitImpactAnimationT : IComponentData, IEnableableComponent
    {
    }
    
    public struct AttackSpeedC : IComponentData
    {
        public float attackSpeed;
    }

    public struct WalkParticleActivatorC : IComponentData
    {
        public Entity particleEntity;
    }
}

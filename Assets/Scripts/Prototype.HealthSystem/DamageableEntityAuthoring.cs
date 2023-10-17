using Unity.Entities;
using UnityEngine;

namespace Prototype.HealthSystem
{


    [DisallowMultipleComponent]
    public class DamageableEntityAuthoring : MonoBehaviour
    {
        public HealthC health;

        public bool destroyAfterDeath;

        void OnEnable() { }

        class Baker : Baker<DamageableEntityAuthoring>
        {
            public override void Bake(DamageableEntityAuthoring authoring)
            {
                if (authoring.enabled == false) return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                this.SetupDamageableEntity(entity, authoring.health);
              
                if (authoring.destroyAfterDeath)
                {
                    AddComponent<DestroyAfterDeath>(entity);
                }
            }
        }
    }
}

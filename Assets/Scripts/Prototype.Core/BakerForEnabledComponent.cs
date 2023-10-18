using Unity.Entities;
using UnityEngine;

namespace Prototype.ECS.Baking
{
    /// <summary>
    /// Add enable/disable feature for mono to ecs bakers
    /// </summary>
    /// <typeparam name="TC">component type</typeparam>
    public abstract class BakerForEnabledComponent<TC> : Baker<TC> where TC : MonoBehaviour
    {
        protected virtual void OnEnable() { }
        public override void Bake(TC authoring)
        {
            if (authoring.enabled == false)
                return;

            BakeIfEnabled(authoring);
        }

        public abstract void BakeIfEnabled(TC authoring);
    }

}

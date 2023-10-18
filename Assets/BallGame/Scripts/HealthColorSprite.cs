using Prototype.HealthSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;



namespace SV.BallGame
{

    [DisallowMultipleComponent]
    public class HealthColorSprite : MonoBehaviour
    {
        public Color maxHealth;
        public Color minHealth;
        public SpriteRenderer sprite;
        public TextMesh text;
        void OnEnable() { }

        class Baker : Baker<HealthColorSprite>
        {
            public override void Bake(HealthColorSprite authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new HealthColorSpriteC
                {
                    maxHealth = authoring.maxHealth,
                    minHealth = authoring.minHealth,
                    spriteHolder = GetEntity(authoring.sprite, TransformUsageFlags.None),
                    text = GetEntity(authoring.text, TransformUsageFlags.None)


                });
            }
        }
    }

    public struct HealthColorSpriteC : IComponentData
    {
        public Color maxHealth;
        public Color minHealth;
        public Entity spriteHolder;
        public Entity text;

    }


    public partial struct HealthColorSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (heath, healthSprite) in SystemAPI.Query<RefRO<HealthC>, HealthColorSpriteC>())
            {
                var sprite = SystemAPI.ManagedAPI.GetComponent<SpriteRenderer>(healthSprite.spriteHolder);
                var text = SystemAPI.ManagedAPI.GetComponent<TextMesh>(healthSprite.text);
                text.text = heath.ValueRO.health.ToString();
                var t = heath.ValueRO.health / (float)heath.ValueRO.healthMax;
                sprite.color = Color.Lerp(healthSprite.minHealth, healthSprite.maxHealth, t);

            }
        }
    }
}
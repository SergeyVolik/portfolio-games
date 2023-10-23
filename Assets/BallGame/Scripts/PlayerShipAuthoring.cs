using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace SV.BallGame
{
    [DisallowMultipleComponent]
    public class PlayerShipAuthoring : MonoBehaviour
    {
        public float moveSpeed;

        void OnEnable() { }

        class Baker : Baker<PlayerShipAuthoring>
        {
            public override void Bake(PlayerShipAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new BoardC
                {
                    moveSpeed = authoring.moveSpeed
                });

                AddComponent<ReadPlayerInput>(entity);
                AddComponent<ShipInputC>(entity);

            }
        }
    }

    public struct BoardC : IComponentData
    {
        public float moveSpeed;

    }


}
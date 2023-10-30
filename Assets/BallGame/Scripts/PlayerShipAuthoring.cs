using Prototype;
using Prototype.ProjectileSpawner;
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
        public GameObject shipRoot;
        public float shipMaxAnimRot;
        public float rotSpeed;
        void OnEnable() { }

        class Baker : Baker<PlayerShipAuthoring>
        {
            public override void Bake(PlayerShipAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new ShipControllerDataC
                {
                    moveSpeed = authoring.moveSpeed,
                    rotationSpeed = authoring.rotSpeed
                });

                AddComponent<ReadPlayerInput>(entity);
                AddComponent<ShipInputC>(entity);

                var guns = GetComponentsInChildren<ProjectileSpawnerAuthoring>();

                var buffer = AddBuffer<ShipGunsBuff>(entity);

                foreach (var gunsBuff in guns)
                {
                    buffer.Add(new ShipGunsBuff
                    {
                        gun = GetEntity(gunsBuff, TransformUsageFlags.Dynamic)
                    });
                }
            }
        }
    }

    public struct ShipGunsBuff : IBufferElementData
    {
        public Entity gun;
    }

    public struct ShipControllerDataC : IComponentData
    {
        public float moveSpeed;
        public float rotationSpeed;
    }
}
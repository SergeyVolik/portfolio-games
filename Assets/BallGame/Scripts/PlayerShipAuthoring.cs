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
                    moveSpeed = authoring.moveSpeed
                });
                AddComponent(entity, new ShipAnimationDataC
                {
                    rotSpeed = authoring.rotSpeed,
                    shipMaxAnimRot = authoring.shipMaxAnimRot,
                    shipRoot = GetEntity(authoring.shipRoot, TransformUsageFlags.Dynamic)
                });

                AddComponent<ReadPlayerInput>(entity);
                AddComponent<ShipInputC>(entity);

            }
        }
    }

    public struct ShipControllerDataC : IComponentData
    {
        public float moveSpeed;

    }

    public struct ShipAnimationDataC : IComponentData
    {
        public Entity shipRoot;
        public float shipMaxAnimRot;
        public float rotSpeed;


    }
}
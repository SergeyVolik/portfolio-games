using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace SV.BallGame
{
    [DisallowMultipleComponent]
    public class PlayerBoard : MonoBehaviour
    {
        public float moveSpeed;
        public float ballSpeed;
        public int ballDamage;

        public Transform ballSpawnPoint;
        public Transform ballPrefab;
        void OnEnable() { }

        class Baker : Baker<PlayerBoard>
        {
            public override void Bake(PlayerBoard authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new BoardC
                {
                    moveSpeed = authoring.moveSpeed,
                    ballSpeed = authoring.ballSpeed,
                    ballSpawnPoint = GetEntity(authoring.ballSpawnPoint, TransformUsageFlags.Dynamic),
                    ballPrefab = GetEntity(authoring.ballPrefab, TransformUsageFlags.Dynamic),
                    ballDamage = authoring.ballDamage,

                });

                AddComponent<ReadPlayerInput>(entity);
                AddComponent<BoardInputC>(entity);

            }
        }
    }

    public struct BoardC : IComponentData
    {
        public float moveSpeed;
        public float ballSpeed;
        public Entity ballSpawnPoint;
        public Entity ballPrefab;
        public int ballDamage;

    }


}
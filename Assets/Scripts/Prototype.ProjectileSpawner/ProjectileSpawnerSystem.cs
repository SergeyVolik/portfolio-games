using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;

namespace Prototype.ECS.Runtime
{
    public partial struct ProjectileSpawnerSystem : ISystem
    {       
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProjectileSpawnerC>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            SemiSpawner(ref state, ecb);
            AutoSpawner(ref state, ecb);
            BurstSpawner(ref state, ecb);
        }

        [BurstCompile]
        private void SemiSpawner(ref SystemState state, EntityCommandBuffer ecb)
        {

            foreach (var item in SystemAPI.Query<EnabledRefRW<ProjectileSpawnerC>>().WithAll<CooldownC, SemiSpawnC>())
            {
                item.ValueRW = false;
            }

            foreach (var (gun, e) in SystemAPI.Query<ProjectileSpawnerC>().WithNone<CooldownC>().WithAll<SemiSpawnC>().WithEntityAccess())
            {
                ecb.AddComponentAndEnable(e, new CooldownC
                {
                    t = 0,
                    duration = gun.delayBetweenShots,
                });

                SpawnProjectile(ref state, ecb, gun, e);

            }
        }

        [BurstCompile]
        private void AutoSpawner(ref SystemState state, EntityCommandBuffer ecb)
        {
          
            foreach (var (gun, e) in SystemAPI.Query<ProjectileSpawnerC>().WithNone<CooldownC>().WithAll<AutoSpawnC>().WithEntityAccess())
            {
                ecb.AddComponentAndEnable(e, new CooldownC
                {
                    t = 0,
                    duration = gun.delayBetweenShots,
                });

                SpawnProjectile(ref state,ecb, gun, e);

            }
        }

        [BurstCompile]
        private void BurstSpawner(ref SystemState state, EntityCommandBuffer ecb)
        {
            foreach (var (gun, burstSpawnRef,e) in SystemAPI.Query<ProjectileSpawnerC, RefRW<BurstSpawnC>>().WithNone<CooldownC>().WithAll<BurstSpawnC>().WithEntityAccess())
            {
                if (SystemAPI.IsComponentEnabled<CooldownC>(burstSpawnRef.ValueRO.cooldownDelayE))
                    continue;

                ecb.AddComponentAndEnable<CooldownC>(e, new CooldownC
                {
                    duration = burstSpawnRef.ValueRO.delays
                });

                burstSpawnRef.ValueRW.currentProduceCount++;

                SpawnProjectile(ref state, ecb, gun, e);

                if (burstSpawnRef.ValueRW.currentProduceCount >= burstSpawnRef.ValueRW.produceTimes)
                {
                    burstSpawnRef.ValueRW.currentProduceCount = 0;
                    ecb.AddComponentAndEnable(e, new CooldownC
                    {
                        t = 0,
                        duration = gun.delayBetweenShots,
                    });
                }
            }
        }

        [BurstCompile]
        private void SpawnProjectile(ref SystemState state, EntityCommandBuffer ecb, ProjectileSpawnerC gun, Entity e)
        {
            var fullOffset = gun.projectileOffset * gun.projectiles;
            var halfOffset = fullOffset / 2f - (gun.projectileOffset / 2f);

            var spawnPointLTW = SystemAPI.GetComponentRO<LocalToWorld>(gun.projectileSpawnPoint);
            var ltwSpawnPos = spawnPointLTW.ValueRO.Position;

            var ltwSpawnForward = gun.twoDimMode ? spawnPointLTW.ValueRO.Up : spawnPointLTW.ValueRO.Forward;
            var ltwSpawnRight = spawnPointLTW.ValueRO.Right;
            var ltwSpawnUp = gun.twoDimMode ? -spawnPointLTW.ValueRO.Forward : spawnPointLTW.ValueRO.Up;

            var allAngle = math.radians(gun.angleOffset * (gun.projectiles - 1));
            var halfAllAngle = allAngle / 2;

            for (int i = 0; i < gun.projectiles; i++)
            {
                float3 vector = ltwSpawnForward;
                float3 spawnPos = ltwSpawnPos;


                switch (gun.spawnType)
                {
                    case SpawnProjectileMoveType.Forward:
                        spawnPos = spawnPos + ltwSpawnRight * (-halfOffset + gun.projectileOffset * i);
                        break;
                    case SpawnProjectileMoveType.Angle:

                        var qut = quaternion.AxisAngle(ltwSpawnUp, math.radians(gun.angleOffset * i) - halfAllAngle);
                        vector = math.mul(qut, ltwSpawnForward);

                        break;
                    default:
                        break;
                }

                var instance = ecb.Instantiate(gun.projectilePrefab);

                ecb.SetComponent(instance, LocalTransform.FromPositionRotation(spawnPos, quaternion.LookRotation(vector, math.up())));

                ecb.SetComponent<PhysicsVelocity>(instance, new PhysicsVelocity
                {
                    Linear = vector * gun.speed
                });

                if (SystemAPI.HasComponent<OwnerC>(e))
                {
                    ecb.AddComponent<OwnerC>(instance, new OwnerC
                    {
                        value = SystemAPI.GetComponentRO<OwnerC>(e).ValueRO.value
                    });
                }
            }
        }
    }
}

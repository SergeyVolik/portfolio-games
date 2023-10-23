using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

namespace Prototype.ProjectileSpawner
{
    [UpdateInGroup(typeof(ProjectileSpawnerGroup))]
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

                SpawnProjectile(ref state, ecb, gun, e);

            }
        }

        [BurstCompile]
        private void BurstSpawner(ref SystemState state, EntityCommandBuffer ecb)
        {
            foreach (var (gun, burstSpawnRef, e) in
                SystemAPI.Query<ProjectileSpawnerC, RefRW<BurstSpawnC>>().WithNone<CooldownC>().WithAll<BurstSpawnC>().WithEntityAccess())
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
        private void SpawnProjectile(ref SystemState state, EntityCommandBuffer ecb, ProjectileSpawnerC gun, Entity gunEntity)
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

            var upWorld = gun.twoDimMode ? math.back() : math.up();


            for (int i = 0; i < gun.projectiles; i++)
            {
                float3 vector = ltwSpawnForward;
                float3 spawnPos = ltwSpawnPos;

                if ((gun.spawnType & SpawnProjectileMoveType.Forward) == SpawnProjectileMoveType.Forward)
                {
                    spawnPos = spawnPos + ltwSpawnRight * (-halfOffset + gun.projectileOffset * i);

                    var rotation = gun.twoDimMode ? quaternion.identity : quaternion.LookRotation(vector, math.up());
                    var instance = ecb.Instantiate(gun.projectilePrefab);

                    SetupProjectileInstance(ref state, ecb, gun, gunEntity, vector, spawnPos, rotation, instance);
                }

                if ((gun.spawnType & SpawnProjectileMoveType.Angle) == SpawnProjectileMoveType.Angle)
                {
                    var qut = quaternion.AxisAngle(ltwSpawnUp, math.radians(gun.angleOffset * i) - halfAllAngle);
                    vector = math.mul(qut, ltwSpawnForward);

                    var rotation = gun.twoDimMode ? quaternion.identity : quaternion.LookRotation(vector, math.up());
                    var instance = ecb.Instantiate(gun.projectilePrefab);

                    SetupProjectileInstance(ref state, ecb, gun, gunEntity, vector, spawnPos, rotation, instance);
                }
            }
        }

        private void SetupProjectileInstance(ref SystemState state, EntityCommandBuffer ecb, ProjectileSpawnerC gun, Entity gunEntity, float3 vector, float3 spawnPos, quaternion rotation, Entity projectileInstance)
        {
            //PrototypeDebug.Log($"SetupProjectileInstance {projectileInstance}");
            ecb.SetComponent(projectileInstance, LocalTransform.FromPositionRotation(spawnPos, rotation));

            ecb.AddComponent(projectileInstance, new PhysicsVelocity
            {
                Linear = vector * gun.speed
            });

            ecb.AddComponent(projectileInstance, new ProjectileC
            {
                damage = gun.damage
            });

            ecb.SetupLifetime(projectileInstance, new LifetimeC
            {
                value = gun.projectileLifetime
            });

            if (SystemAPI.HasComponent<OwnerC>(gunEntity))
            {
                ecb.AddComponent(projectileInstance, new OwnerC
                {
                    value = SystemAPI.GetComponentRO<OwnerC>(gunEntity).ValueRO.value
                });
            }
        }
    }
}

using Prototype.ECS.Runtime;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Prototype.ECS.Runtime
{
    public enum SpawnProjectileMoveType
    {
        Forward,
        Angle
    }

    public enum SpawnDelayType
    {
        Semi,
        Auto,
        Burst
    }


    public struct BurstSpawnC : IComponentData, IEnableableComponent
    {
        public float delays;
        public int produceTimes;
        public int currentProduceCount;
        public Entity cooldownDelayE;
    }

    public struct SemiSpawnC : IComponentData, IEnableableComponent { }

    public struct AutoSpawnC : IComponentData, IEnableableComponent { }

    public struct ProjectileSpawnerC : IComponentData, IEnableableComponent
    {
        public Entity projectilePrefab;
        public float delayBetweenShots;
        public int projectiles;
        public Entity projectileSpawnPoint;
        public float speed;
        public SpawnProjectileMoveType spawnType;
        public float angleOffset;
        public float projectileOffset;
        public bool twoDimMode;
    }

}




namespace Prototype.ECS.Baking
{



    [DisallowMultipleComponent]
    public class ProjectileSpawnerAuthoring : MonoBehaviour
    {
        public Transform spawnPoint;
        public GameObject projectilePrefab;
        public SpawnProjectileMoveType spawnMoveType;
        public SpawnDelayType delayType;

        public float delayBetweenShots;
        public float projectileSpeed;
        public bool twoDimMode;


        [Min(1)]
        public int projectiles = 1;

        [BoxGroup("Angle")]
        [ShowIf("@this.spawnMoveType == SpawnProjectileMoveType.Angle")]
        public float angleOffset = 1;

        [BoxGroup("Forward")]
        [ShowIf("@this.spawnMoveType == SpawnProjectileMoveType.Forward")]
        public float projectileOffset = 0.1f;

        [BoxGroup("Burst")]
        [ShowIf("@this.delayType == SpawnDelayType.Burst")]
        public float burstSpawnDelays = 0.1f;
        [BoxGroup("Burst")]
        [ShowIf("@this.delayType == SpawnDelayType.Burst")]
        public int burstSpawns = 2;

        void OnEnable() { }
        class Baker : Baker<ProjectileSpawnerAuthoring>
        {
            public override void Bake(ProjectileSpawnerAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new ProjectileSpawnerC
                {
                    delayBetweenShots = authoring.delayBetweenShots,
                    speed = authoring.projectileSpeed,
                    projectileSpawnPoint = GetEntity(authoring.spawnPoint, TransformUsageFlags.Dynamic),
                    projectilePrefab = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic),
                    projectiles = authoring.projectiles,
                    spawnType = authoring.spawnMoveType,
                    angleOffset = authoring.angleOffset,
                    projectileOffset = authoring.projectileOffset,
                    twoDimMode = authoring.twoDimMode,
                });

                SetComponentEnabled<ProjectileSpawnerC>(entity, false);

                AddComponent<AutoSpawnC>(entity);
                SetComponentEnabled<AutoSpawnC>(entity, false);

                var burstCooldownE = CreateAdditionalEntity(TransformUsageFlags.None, false, "gun cooldown");

                AddComponent<CooldownC>(burstCooldownE);
                SetComponentEnabled<CooldownC>(burstCooldownE, false);

                AddComponent<BurstSpawnC>(entity, new BurstSpawnC
                {
                    delays = authoring.burstSpawnDelays,
                    produceTimes = authoring.burstSpawns,
                    cooldownDelayE = burstCooldownE
                });

                SetComponentEnabled<BurstSpawnC>(entity, false);

                //AddComponent<CooldownC>(entity);
                //SetComponentEnabled<CooldownC>(entity, false);

                AddComponent<SemiSpawnC>(entity);
                SetComponentEnabled<SemiSpawnC>(entity, false);


                switch (authoring.delayType)
                {
                    case SpawnDelayType.Semi:
                        SetComponentEnabled<SemiSpawnC>(entity, true);
                        break;
                    case SpawnDelayType.Auto:
                        SetComponentEnabled<AutoSpawnC>(entity, true);

                        break;
                    case SpawnDelayType.Burst:
                        SetComponentEnabled<BurstSpawnC>(entity, true);

                        break;
                    default:
                        break;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            switch (spawnMoveType)
            {
                case SpawnProjectileMoveType.Forward:
                    var fullOffset = projectileOffset * projectiles;
                    var halfOffset = fullOffset / 2f - (projectileOffset / 2f);

                    for (int i = 0; i < projectiles; i++)
                    {

                        var forward = twoDimMode ? transform.up : transform.forward;
                        var right = transform.right;

                        var pos1 = spawnPoint.position;
                        var pos2 = spawnPoint.position + forward * 5f;

                        pos1 = pos1 + right * (-halfOffset + projectileOffset * i);
                        pos2 = pos2 + right * (-halfOffset + projectileOffset * i);

                        Gizmos.DrawLine(pos1, pos2);
                    }
                    break;

                case SpawnProjectileMoveType.Angle:



                    var allAngle = math.radians(angleOffset * (projectiles - 1));
                    var halfAllAngle = allAngle / 2;
                    var forwardVec = twoDimMode ? transform.up : transform.forward;
                    var upVec = twoDimMode ? -transform.forward : transform.up;
                    for (int i = 0; i < projectiles; i++)
                    {

                        var qut = quaternion.AxisAngle(upVec, math.radians(angleOffset * i) - halfAllAngle);

                        var forward = math.mul(qut, forwardVec);

                        var pos1 = spawnPoint.position;
                        var pos2 = spawnPoint.position + (Vector3)forward * 5f;

                        Gizmos.DrawLine(pos1, pos2);
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
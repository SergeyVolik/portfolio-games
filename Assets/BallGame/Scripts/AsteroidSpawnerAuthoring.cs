using Prototype;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace SV.BallGame
{

    [DisallowMultipleComponent]
    public class AsteroidSpawnerAuthoring : MonoBehaviour
    {
        public GameObject asteroidPrefab;

        public Vector2 asteroidSizeRange;
        public Vector2 asteroidSpeedRange;
        public Vector2 asteroidRotationSpeedRange;
        public float spawnInterval;

        void OnEnable() { }

        class Baker : Baker<AsteroidSpawnerAuthoring>
        {
            public override void Bake(AsteroidSpawnerAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new AsteroidSpawnerC
                {
                    asteroidPrefab = GetEntity(authoring.asteroidPrefab, TransformUsageFlags.Dynamic),
                    asteroidSizeRange = authoring.asteroidSizeRange,
                    asteroidRotationSpeedRange = authoring.asteroidRotationSpeedRange,
                    asteroidSpeedRange = authoring.asteroidSpeedRange,
                    spawnInterval = authoring.spawnInterval,

                });

                AddComponent<CooldownC>(entity, new CooldownC
                {
                    duration = authoring.spawnInterval
                });
            }
        }
    }

    public struct AsteroidSpawnerC : IComponentData
    {
        public Entity asteroidPrefab;

        public float2 asteroidSizeRange;
        public float2 asteroidSpeedRange;
        public float2 asteroidRotationSpeedRange;

        public float spawnInterval;
    }

    public partial class AsteroidSpawnerSystem : SystemBase
    {
        private Unity.Mathematics.Random rnd;
        private Camera cam;

        protected override void OnCreate()
        {
            base.OnCreate();
            rnd = new Unity.Mathematics.Random(100);
            RequireForUpdate<CameraTarget>();

        }
        protected override void OnUpdate()
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);

            if(cam == null)
                cam = Camera.main;


            var worldToScreen = WorldToScreen.Create(cam);
            var targetPos = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<CameraTarget>()).Position;

            foreach (var (item, e) in SystemAPI.Query<AsteroidSpawnerC>().WithNone<CooldownC>().WithEntityAccess())
            {
                ecb.SetCooldown(e, new CooldownC
                {
                    t = 0,
                    duration = item.spawnInterval
                });

                var asteroidInstance = ecb.Instantiate(item.asteroidPrefab);

                float3 pos = float3.zero;              
                int count = 0;

                while (true)
                {
                    var rndPos = rnd.NextFloat2(new float2(-400, -400), new float2(400, 400));
                    pos = new float3(rndPos.x, 0, rndPos.y) + targetPos;

                    if (!worldToScreen.IsPositionInsideScreen(pos))
                    {
                        break;
                    }

                    count++;

                    if (count > 100)
                    {
                        Debug.LogError("Can't generate position for asteroid");
                        break;
                    }


                }

                float3 vector = math.normalize(targetPos - pos);
                var size = rnd.NextFloat(item.asteroidSizeRange.x, item.asteroidSizeRange.y);

                var localTransform = LocalTransform.FromPositionRotationScale(pos, quaternion.identity, size);
               
                ecb.SetComponent<LocalTransform>(asteroidInstance, localTransform);

                var dir = vector * rnd.NextFloat(item.asteroidSpeedRange.x, item.asteroidSpeedRange.y);

                ecb.SetComponent<PhysicsVelocity>(asteroidInstance, new PhysicsVelocity
                {
                    Linear = dir,
                    Angular = rnd.NextFloat3Direction() * rnd.NextFloat(item.asteroidRotationSpeedRange.x, item.asteroidRotationSpeedRange.y)
                });


            }
        }
    }
}

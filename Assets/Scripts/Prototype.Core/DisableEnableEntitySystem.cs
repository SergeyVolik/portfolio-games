using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Prototype
{
    public struct DisableEntityActionC : IComponentData
    {
        public Entity entity;
        public int skipFrames;
    }

    public struct EnableEntityActionC : IComponentData
    {
        public Entity entity;
        public int skipFrames;
    }



    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial struct DisableEnableEntitySystem : ISystem
    {

        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder().WithAny<DisableEntityActionC, EnableEntityActionC>().Build();
            state.RequireForUpdate(query);
           

        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            DisableEntity(ref state, ecb);
            EnableEntity(ref state, ecb);

        }

        private void EnableEntity(ref SystemState state, EntityCommandBuffer ecb)
        {
            foreach (var (item, e) in SystemAPI.Query<RefRW<EnableEntityActionC>>().WithEntityAccess())
            {
                var entity = item.ValueRW.entity;
                if (item.ValueRO.skipFrames > 0)
                {
                    item.ValueRW.skipFrames--;
                    continue;
                }

                if (SystemAPI.HasBuffer<LinkedEntityGroup>(entity))
                {
                    var linkeItems = SystemAPI.GetBuffer<LinkedEntityGroup>(entity);
                    //Debug.Log("Enable LinkedEntityGroup");
                    foreach (var item1 in linkeItems)
                    {
                        if (SystemAPI.HasComponent<Disabled>(item1.Value))
                            ecb.RemoveComponent<Disabled>(item1.Value);
                    }
                }
                else
                {
                    //Debug.Log("Enable Entity");
                    if (SystemAPI.HasComponent<Disabled>(entity))
                        ecb.RemoveComponent<Disabled>(entity);
                }

                ecb.DestroyEntity(e);


            }
        }

        private void DisableEntity(ref SystemState state, EntityCommandBuffer ecb)
        {
            foreach (var (item, e) in SystemAPI.Query<RefRW<DisableEntityActionC>>().WithEntityAccess())
            {
                var entity = item.ValueRW.entity;
                if (item.ValueRO.skipFrames > 0)
                {
                    item.ValueRW.skipFrames--;
                    continue;
                }

                if (SystemAPI.HasBuffer<LinkedEntityGroup>(entity))
                {
                    var linkeItems = SystemAPI.GetBuffer<LinkedEntityGroup>(entity);
                    //Debug.Log("Disable LinkedEntityGroup");
                    foreach (var item1 in linkeItems)
                    {
                        if (!SystemAPI.HasComponent<Disabled>(item1.Value) && SystemAPI.HasComponent<Simulate>(item1.Value))
                            ecb.AddComponent<Disabled>(item1.Value);
                    }
                }
                else
                {
                    //Debug.Log("Disable Entity");
                    if (!SystemAPI.HasComponent<Disabled>(entity) && SystemAPI.HasComponent<Simulate>(entity))
                        ecb.AddComponent<Disabled>(entity);
                }

                ecb.DestroyEntity(e);


            }
        }
    }
}
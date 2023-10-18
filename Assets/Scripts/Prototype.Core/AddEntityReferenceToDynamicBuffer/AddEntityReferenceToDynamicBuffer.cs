using Unity.Burst.Intrinsics;
using Unity.Entities;

namespace Prototype
{
    public interface IBufferRefSource
    {
        public Entity GetBufferEntity();
    }

    public interface IEntityBufferRefDestination
    {
        public void SetRef(Entity entity);
    }

    public abstract partial class AddEntityReferenceToDynamicBuffer<TBufferHolder, TBufferType> : SystemBase
        where TBufferHolder : unmanaged, IBufferRefSource, IComponentData
        where TBufferType : unmanaged, IEntityBufferRefDestination, IBufferElementData
    {
        EntityQuery query;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            query = GetEntityQuery(ComponentType.ReadWrite<TBufferHolder>());
        }

        protected override void OnUpdate()
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);
            
            var job = new AddJob
            {
                ecb = ecb,
                bufferHolderHandle = SystemAPI.GetComponentTypeHandle<TBufferHolder>(),
                entityHandle = SystemAPI.GetEntityTypeHandle(),
                typeHandle = SystemAPI.GetBufferLookup<TBufferType>()
            };

            Dependency = job.Schedule(query, Dependency);
        }
        
        public partial struct AddJob : IJobChunk
        {
            public EntityCommandBuffer ecb;
            internal BufferLookup<TBufferType> typeHandle;
            internal ComponentTypeHandle<TBufferHolder> bufferHolderHandle;
            internal EntityTypeHandle entityHandle;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                var compHandle = chunk.GetNativeArray(ref bufferHolderHandle);

                var entities = chunk.GetNativeArray(entityHandle);

                for (var i = 0; i < entities.Length; i++)
                {
                    var e = entities[i];
                    var comp = compHandle[i];

                    if (typeHandle.TryGetBuffer(comp.GetBufferEntity(), out var buffer))
                    {
                        TBufferType data = default;
                        data.SetRef(e);
                        buffer.Add(data);
                    }

                    ecb.RemoveComponent<TBufferHolder>(e);
                }
            }
        }
    }
}
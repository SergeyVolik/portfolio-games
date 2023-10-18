using Prototype;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Prototype.ECS.Runtime
{
    /// <summary>
    /// Internal <see cref="HybridAnimationEvents"> data. Needs to cleanup an internal data.
    /// </summary>
    public struct AnimatorEventsLisneter : ICleanupComponentData
    {
        public int InstanceId;
    }

    /// <summary>
    /// Convert MonoBehaviour Animation events to ECS
    /// </summary>
    public partial class HybridAnimationEvents : SystemBase
    {
        private struct AnimationEventData
        {
            public int hash;
            public int goInstance;
        }

        NativeHashMap<int, Entity> m_GOInstanceIdToEntity;
        NativeList<AnimationEventData> m_Events;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_GOInstanceIdToEntity = new NativeHashMap<int, Entity>(100, Allocator.Persistent);
            m_Events = new NativeList<AnimationEventData>(100, Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            m_Events.Dispose();
            m_GOInstanceIdToEntity.Dispose();
        }
        protected override void OnUpdate()
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);

            SetupAnimatorEvents(ecb);
            CleanUpEvents(ecb);
            ProduceEvents(ecb);

           

        }

        private void CleanUpEvents(EntityCommandBuffer ecb)
        {
            foreach (var (item, e) in SystemAPI.Query<AnimatorEventsLisneter>().WithNone<Simulate>().WithEntityAccess())
            {
                m_GOInstanceIdToEntity.Remove(item.InstanceId);
                ecb.RemoveComponent<AnimatorEventsLisneter>(e);
            }
        }

        private void ProduceEvents(EntityCommandBuffer ecb)
        {
            foreach (var item in m_Events)
            {
                if (m_GOInstanceIdToEntity.TryGetValue(item.goInstance, out var e))
                {
                    ecb.SetComponent<AnimationEvent>(e, new AnimationEvent
                    {
                        eventHash = item.hash
                    });
                    ecb.SetComponentEnabled<AnimationEvent>(e, true);
                }
            }

            m_Events.Clear();
        }

        private void SetupAnimatorEvents(EntityCommandBuffer ecb)
        {
            foreach (var (item, e) in SystemAPI.Query<SystemAPI.ManagedAPI.UnityEngineComponent<Animator>>().WithNone<AnimatorEventsLisneter>().WithEntityAccess())
            {
                if (!item.Value.TryGetComponent<AnimationEventListener>(out var listener))
                {
                    listener = item.Value.gameObject.AddComponent<AnimationEventListener>();
                }

                var goInstnaceId = item.Value.gameObject.GetInstanceID();
                listener.OnAnimationEvent += Listener_OnAnimationEvent;
                ecb.AddComponent<AnimationEvent>(e);
                ecb.SetComponentEnabled<AnimationEvent>(e, false);
                ecb.AddComponent<AnimatorEventsLisneter>(e, new AnimatorEventsLisneter
                {
                    InstanceId = goInstnaceId
                });

                m_GOInstanceIdToEntity.Add(goInstnaceId, e);
            }
        }

        private void Listener_OnAnimationEvent(int hash, int goInstanceId)
        {
            //Debug.Log($"Listener_OnAnimationEvent hash: {hash} goInstanceId: {goInstanceId}");
            m_Events.Add(new AnimationEventData
            {
                goInstance = goInstanceId,
                hash = hash
            });
        }
    }
}

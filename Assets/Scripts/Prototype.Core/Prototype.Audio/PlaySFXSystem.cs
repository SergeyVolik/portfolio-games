using System;
using Unity.Collections;
using Unity.Entities;

namespace Prototype.Audio
{
    /// <summary>
    /// Input for <see cref="PlaySFXSystem">
    /// live 1 frame
    /// Helps to play sounds from ECS system <see cref="PlaySFXSystem">
    /// </summary>
    public struct PlaySFXCommandC : IComponentData, IEquatable<PlaySFXCommandC>
    {
        public Unity.Entities.Hash128 sfxSettingGuid;
        public float playTime;
        public bool Equals(PlaySFXCommandC other)
        {
            return other.GetHashCode() == GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is PlaySFXCommandC sfx)
            {
                return Equals(sfx);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return sfxSettingGuid.GetHashCode();
        }
    }

    /// <summary>
    /// Produce sounds with <see cref="AudioManager">
    /// </summary>
    public partial struct PlaySFXSystem : ISystem
    {
        private EntityQuery playSFXQuery;

        public void OnCreate(ref SystemState state)
        {
            playSFXQuery = SystemAPI.QueryBuilder().WithAll<PlaySFXCommandC>().Build();

            state.RequireForUpdate<SFXDatabaseComponent>();
            state.RequireForUpdate(playSFXQuery);

        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var sfxdatabase = SystemAPI.ManagedAPI.GetSingleton<SFXDatabaseComponent>().value;

            NativeHashSet<PlaySFXCommandC> playerdSfxes = new NativeHashSet<PlaySFXCommandC>(0, Allocator.Temp);

            var time = (float)SystemAPI.Time.ElapsedTime;

            foreach (var (sfx, e) in SystemAPI.Query<PlaySFXCommandC>().WithEntityAccess())
            {
                if (sfx.playTime > time)
                {
                    continue;
                }

                if (!playerdSfxes.Contains(sfx))
                {
                    AudioManager.GetInstance().PlaySFX(sfxdatabase.GetItem(sfx.sfxSettingGuid));

                    playerdSfxes.Add(sfx);

                   
                }

                ecb.DestroyEntity(e);
            }
        }
    }
}
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;

namespace Unity.Physics.Stateful
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct PhysicsImpulseExecutorSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (pv, m, imp, trans, e) in SystemAPI
                         .Query<RefRW<PhysicsVelocity>, RefRO<PhysicsMass>, DynamicBuffer<AddImpulseC>,
                             RefRW<LocalTransform>>().WithEntityAccess())
            {
                foreach (var item in imp)
                {
                    pv.ValueRW.ApplyImpulse(m.ValueRO, trans.ValueRO.Position, trans.ValueRO.Rotation, item.impulse,
                        trans.ValueRO.Position);

                    pv.ValueRW.ApplyAngularImpulse(m.ValueRO, item.angular);
                }

                imp.Clear();
                SystemAPI.SetBufferEnabled<AddImpulseC>(e, false);
            }
        }
    }
}
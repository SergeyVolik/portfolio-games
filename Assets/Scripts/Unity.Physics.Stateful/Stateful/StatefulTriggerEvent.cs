using Unity.Entities;
using Unity.Assertions;

namespace Unity.Physics.Stateful
{
    // Trigger Event that can be stored inside a DynamicBuffer
    public struct StatefulTriggerEvent : IStatefulSimulationEvent<StatefulTriggerEvent>
    {
        public Entity _EntityA;
        public Entity _EntityB;

        public Entity EntityA
        {
            get
            {
                return _EntityA;
            }
            set
            {
                _EntityA = value;
            }
        }

        public Entity EntityB
        {
            get
            {
                return _EntityB;
            }
            set
            {
                _EntityB = value;
            }
        }

        public int BodyIndexA { get; set; }
        public int BodyIndexB { get; set; }
        public ColliderKey ColliderKeyA { get; set; }
        public ColliderKey ColliderKeyB { get; set; }
        public StatefulEventState State { get; set; }

        public StatefulTriggerEvent(TriggerEvent triggerEvent)
        {
            _EntityA = triggerEvent.EntityA;
            _EntityB = triggerEvent.EntityB;

            //EntityA = triggerEvent.EntityA;
            //EntityB = triggerEvent.EntityB;
            BodyIndexA = triggerEvent.BodyIndexA;
            BodyIndexB = triggerEvent.BodyIndexB;
            ColliderKeyA = triggerEvent.ColliderKeyA;
            ColliderKeyB = triggerEvent.ColliderKeyB;
            State = default;
        }

        // Returns other entity in EntityPair, if provided with one
        public Entity GetOtherEntity(Entity entity)
        {
            Assert.IsTrue((entity == EntityA) || (entity == EntityB));
            return (entity == EntityA) ? EntityB : EntityA;
        }

        public int CompareTo(StatefulTriggerEvent other) => ISimulationEventUtilities.CompareEvents(this, other);
    }
}

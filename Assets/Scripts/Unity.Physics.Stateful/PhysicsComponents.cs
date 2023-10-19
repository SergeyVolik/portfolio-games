using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Physics.Stateful
{
    public struct AddImpulseC : IBufferElementData, IEnableableComponent
    {
        public float3 impulse;
        public float3 angular;
    }
}
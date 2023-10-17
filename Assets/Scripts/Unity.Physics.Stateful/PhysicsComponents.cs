using Unity.Entities;
using Unity.Mathematics;

namespace Prototype
{
    public struct AddImpulseC : IBufferElementData, IEnableableComponent
    {
        public float3 impulse;
        public float3 angular;
    }
}
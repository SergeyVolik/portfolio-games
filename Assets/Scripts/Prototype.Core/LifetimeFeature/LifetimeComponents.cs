using Unity.Entities;

namespace Prototype
{
    public struct LifetimeC : IComponentData
    {
        public float value;
    }

    public struct CurrentLifetimeC : IComponentData
    {
        public float value;
    }
}
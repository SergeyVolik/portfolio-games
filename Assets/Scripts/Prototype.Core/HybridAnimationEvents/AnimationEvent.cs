using Unity.Entities;

namespace Prototype
{
    /// <summary>
    /// ECS Representation of MonoBehaviour events
    /// </summary>
    [GenerateCleaUpDisableSystem]
    public struct AnimationEvent : IComponentData, IEnableableComponent
    {
        /// <summary>
        /// event hash code
        /// </summary>
        public int eventHash;
    }
}
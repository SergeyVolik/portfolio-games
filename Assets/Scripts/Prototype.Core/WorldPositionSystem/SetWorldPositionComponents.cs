using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Prototype
{
    /// <summary>
    /// Input for <see cref="SetWorldPositionSystem">
    /// </summary>
    public struct SetWorldPositionAndRotationC : IComponentData, IEnableableComponent
    {
        public float3 position;
        public quaternion rotation;

    }

    /// <summary>
    /// Input for <see cref="SetWorldPositionSystem">
    /// Link entity to set pos for delayed action
    /// </summary>
    public struct SetWorldPositionLinkC : IComponentData
    {
        public Entity e;

    }
}
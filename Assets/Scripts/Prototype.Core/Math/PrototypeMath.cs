using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Prototype
{
    public struct LaunchData
    {
        public readonly float3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(float3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }

        public override string ToString()
        {
            return $"initialVelocity: {initialVelocity} timeToTarget: {timeToTarget}";
        }

    }

    public static class PrototypeMath
    {
        public static float3 orthogonal(float3 v)
        {
            float x = math.abs(v.x);
            float y = math.abs(v.y);
            float z = math.abs(v.z);

            float3 other = x < y ? (x < z ? math.right() : math.forward()) : (y < z ? math.up() : math.forward());
            return math.cross(v, other);
        }

        public static LaunchData GetPredictedVelocity(float3 startPosition, float3 targetPostion, float gravity, float maxHeight)
        {
            float displacementY = targetPostion.y - startPosition.y;
            float3 displacementXZ = new float3(targetPostion.x - startPosition.x, 0, targetPostion.z - startPosition.z);
            float time = math.sqrt(-2 * maxHeight / gravity) + math.sqrt(2 * (displacementY - maxHeight) / gravity);
            float3 velocityY = math.up() * math.sqrt(-2 * gravity * maxHeight);
            float3 velocityXZ = displacementXZ / time;

            return new LaunchData(velocityXZ + velocityY * -math.sign(gravity), time);
        }

    }
}
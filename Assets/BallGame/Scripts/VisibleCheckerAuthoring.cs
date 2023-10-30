using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace SV.BallGame
{

    [DisallowMultipleComponent]
    public class VisibleCheckerAuthoring : MonoBehaviour
    {

        void OnEnable() { }

        class Baker : Baker<VisibleCheckerAuthoring>
        {
            public override void Bake(VisibleCheckerAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new VisibleChecker { });
            }
        }
    }

    public struct VisibleChecker : IComponentData
    {
        public bool visible;
    }



    public partial struct VisibleCheckerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldToScreenSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var worldToScreen = SystemAPI.GetSingleton<WorldToScreenSingleton>();


            foreach (var (check, ltw) in SystemAPI.Query<RefRW<VisibleChecker>, LocalToWorld>())
            {
                check.ValueRW.visible = worldToScreen.Value.IsPositionInsideScreen(ltw.Position);

                Debug.Log($"Visible {check.ValueRW.visible}");
            }
          
        }
    }
}

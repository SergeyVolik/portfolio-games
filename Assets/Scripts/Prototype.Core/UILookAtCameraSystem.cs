using Unity.Entities;
using UnityEngine;

namespace Prototype.ECS.UI
{
    /// <summary>
    /// Look UI At camera system
    /// </summary>
    [UpdateInGroup(typeof(UISystemGroup))]
    public partial class UILookAtCameraSystem : SystemBase
    {
        private Camera m_Camera;

        protected override void OnUpdate()
        {
            if (!m_Camera)
                m_Camera = Camera.main;

            if (!m_Camera)
                return;

            var transform = m_Camera.transform;

            foreach (var comp in SystemAPI.Query<UILookAtCameraC>())
            {
                comp.value.forward = transform.forward;
            }
        }
    }
}
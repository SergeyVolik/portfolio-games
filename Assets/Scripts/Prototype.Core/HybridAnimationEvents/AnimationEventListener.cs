using System;
using UnityEngine;


namespace Prototype
{
    /// <summary>
    /// Listen events from animations
    /// </summary>
    public class AnimationEventListener : MonoBehaviour
    {
        private int instanceId;

        public event AnimatorEventExecuted OnAnimationEvent;

        public delegate void AnimatorEventExecuted(int hash, int goInstanceId);

        private void Awake()
        {
            instanceId = gameObject.GetInstanceID();
        }
        public void Event(string name)
        {
            OnAnimationEvent?.Invoke(Animator.StringToHash(name), instanceId);
        }
    }

}


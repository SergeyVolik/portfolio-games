using Unity.Entities;
using UnityEngine;

namespace Prototype
{
    /// <summary>
    /// Unity MonoBeahviour events proxy
    /// </summary>
    public class UnityEventsProvider : Singleton<UnityEventsProvider>
    {
        public delegate void OnApplicationFocusDelegate(bool focus);
        public delegate void OnApplicationPauseDelegate(bool pause);
        public delegate void OnApplicationQuitDelegate();

        public event OnApplicationPauseDelegate onApplicationPause = delegate { };
        public event OnApplicationFocusDelegate onApplicationFocus = delegate { };
        public event OnApplicationQuitDelegate onApplicationQuit = delegate { };

        private void OnApplicationPause(bool pause)
        {           
            onApplicationPause.Invoke(pause);
        }

        private void OnApplicationQuit()
        {          
            onApplicationQuit.Invoke();
        }

        private void OnApplicationFocus(bool focus)
        {            
            onApplicationFocus.Invoke(focus);
        }
    }

}


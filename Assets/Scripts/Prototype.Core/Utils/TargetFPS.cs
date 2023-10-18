using UnityEngine;


namespace Prototype.Utils
{
    public class TargetFPS : MonoBehaviour
    {
        public int target;
        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = target;
        }
    }
}


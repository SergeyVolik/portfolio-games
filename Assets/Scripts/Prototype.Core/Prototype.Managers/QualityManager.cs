using Unity.Entities;
using UnityEngine;

namespace Prototype
{
    public enum GameQuality
    {
        Performance = 0,
        Balanced = 1,
        Max = 2,
    }

    public class QualityManager : Singleton<QualityManager>
    {
        public GameQuality currentQuality;

        const string SaveKey = "GameQualityLevel";

        void Awake()
        {
            LoadSettings();
        }

        void LoadSettings()
        {
            var value = PlayerPrefs.GetInt(SaveKey, 2);

            SetQuality((GameQuality)value);
        }

        void OnDestroy()
        {
            SaveSettings();
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveSettings();
            }
        }

        void SaveSettings()
        {
            PlayerPrefs.SetInt(SaveKey, (int)currentQuality);
        }

        public void SetQuality(GameQuality quality)
        {
            currentQuality = quality;

            //Debug.Log($"Quality Chaged {currentQulity}");

            var targetFps = 30;
            var targetFixedRate = 30;

            QualitySettings.SetQualityLevel((int)quality);

            switch (quality)
            {
                case GameQuality.Performance:
                    targetFps = 30;
                    targetFixedRate = 30;

                    break;
                case GameQuality.Balanced:
                    targetFps = 50;
                    targetFixedRate = 50;

                    break;
                case GameQuality.Max:
                    targetFixedRate = 60;
                    targetFps = 60;
                    break;
            }

            Application.targetFrameRate = targetFps;

            var fixedSimulationGroup = World.DefaultGameObjectInjectionWorld
                ?.GetExistingSystemManaged<FixedStepSimulationSystemGroup>();

            if (fixedSimulationGroup != null)
            {
                fixedSimulationGroup.Timestep = 1.0f / targetFixedRate;
            }
        }
    }
}
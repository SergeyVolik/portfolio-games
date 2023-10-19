using UnityEngine;

namespace Prototype
{
    public class PauseManager : MonoBehaviour
    {
        public static void PauseGame()
        {
            Time.timeScale = 0f;
            //AudioListener.pause = true;
        }
        public static void ResumeGame()
        {
            Time.timeScale = 1;
            //AudioListener.pause = false;
        }
    }
}
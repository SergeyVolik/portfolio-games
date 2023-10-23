using System.Diagnostics;

namespace Prototype
{
    /// <summary>
    /// <see cref="PrototypeDebug"> works with DEVELOPMENT_BUILD or UNITY_EDITOR
    /// </summary>
    public static class PrototypeDebug
    {
        const string DevBuidCond = "DEVELOPMENT_BUILD";
        const string EditorCond = "UNITY_EDITOR";

        [Conditional(DevBuidCond)]
        [Conditional(EditorCond)]
        public static void Log(string log)
        {
            UnityEngine.Debug.Log(log);
        }

        [Conditional(EditorCond)]
        [Conditional(DevBuidCond)]
        public static void LogWarning(string log)
        {
            UnityEngine.Debug.Log(log);
        }

        [Conditional(EditorCond)]
        [Conditional(DevBuidCond)]
        public static void LogError(string log)
        {
            UnityEngine.Debug.Log(log);
        }
    }

}

using Sirenix.OdinInspector;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Prototype.SO
{
    public class AssetWithGuid : ScriptableObject
    {

        [ReadOnly]
        public UnityEngine.Hash128 guid;

        public static UnityEngine.Hash128 emptyGuid;

        public virtual void OnValidate()
        {
#if UNITY_EDITOR
            UpdateGuid();
#endif

        }
#if UNITY_EDITOR
        [Button]
        public void UpdateGuid()
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out var guidStr, out long _);
            guid = UnityEngine.Hash128.Parse(guidStr);
        }
#endif
        public UnityEngine.Hash128 GetGuid()
        {
#if UNITY_EDITOR
            UpdateGuid();
#endif
            return guid;
        }


    }
}

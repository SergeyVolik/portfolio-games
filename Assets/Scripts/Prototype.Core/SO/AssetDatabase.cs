using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;

using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Prototype.SO
{
    public class AssetDatabase<T> : ScriptableObject where T : AssetWithGuid
    {
        [Searchable]
        [InlineEditor]
        public T[] items;

        public Dictionary<UnityEngine.Hash128, T> itemsDic;

#if UNITY_EDITOR
        public static IEnumerable<T> FindAssetsByType()
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            foreach (var t in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(t);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    yield return asset;
                }
            }
        }
#endif
        protected virtual void OnEnable()
        {
            InitDictionary();


        }

#if UNITY_EDITOR
        [Button]
        public void FindItemsInProject()
        {
            items = FindAssetsByType().ToArray();
          
        }
#endif

        [Button]
        public void InitDictionary()
        {
            if (items == null)
                return;

            itemsDic = new Dictionary<UnityEngine.Hash128, T>();

            foreach (var item in items)
            {
                if (item == null)
                {
                    Debug.LogError($"AssetDatabase {typeof(T)} item is null");
                    continue;
                }
                var guid = item.GetGuid();

                itemsDic.Add(guid, item);

            }
        }
        public T GetItem(UnityEngine.Hash128 sfxSettingGuid)
        {
            T result = null;

            itemsDic.TryGetValue(sfxSettingGuid, out result);

            return result;
        }
    }
}
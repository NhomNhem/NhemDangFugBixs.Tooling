using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VContainer.Unity;
using NhemDangFugBixs.Generated;

namespace NhemDangFugBixs.Editor
{
    [InitializeOnLoad]
    public static class SceneInjectedProcessor
    {
        static SceneInjectedProcessor()
        {
            EditorSceneManager.sceneSaving += OnSceneSaving;
        }

        private static void OnSceneSaving(UnityEngine.SceneManagement.Scene scene, string path)
        {
            ProcessScene(scene);
        }

        [MenuItem("Tools/NhemDangFugBixs/Force Sync Scene Injection")]
        public static void ForceSync()
        {
            ProcessScene(EditorSceneManager.GetActiveScene());
        }

        private static void ProcessScene(UnityEngine.SceneManagement.Scene scene)
        {
            var lifetimeScope = UnityEngine.Object.FindObjectsByType<LifetimeScope>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .FirstOrDefault(s => s.gameObject.scene == scene);

            if (lifetimeScope == null) return;

            var blueprintTypes = SceneInjectionBlueprint.ComponentTypes;
            if (blueprintTypes == null || blueprintTypes.Length == 0) return;

            bool changed = false;
            var autoInjectList = lifetimeScope.autoInjectGameObjects;

            // 1. Remove missing or invalid references
            int removedCount = autoInjectList.RemoveAll(go => go == null);
            if (removedCount > 0) changed = true;

            // 2. Find and add components marked with [AutoInjectScene]
            foreach (var type in blueprintTypes)
            {
                var foundObjects = UnityEngine.Object.FindObjectsByType(type, FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .Cast<Component>()
                    .Where(c => c.gameObject.scene == scene)
                    .Select(c => c.gameObject)
                    .Distinct();

                foreach (var go in foundObjects)
                {
                    if (!autoInjectList.Contains(go))
                    {
                        autoInjectList.Add(go);
                        changed = true;
                        Debug.Log($"[AutoInject] Added {go.name} to {lifetimeScope.name}");
                    }
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(lifetimeScope);
            }
        }
    }
}

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Racer.EzSceneSwitcher.Editor
{
    internal static class Constants
    {
        public const string ContextMenuPath = "Racer/EzSceneSwitcher/";
        public const string DropdownID = OverlayID + "/scenes-dropdown";

        public const string DisplayName = "Ez Scene Switcher";
        public const string OverlayID = "scene-switcher";
        public const string Tooltip = "Ezly switch to another scene here.";

        public const string SceneIconId = "d_SceneAsset Icon";
        public const string PingIconId = "d_Animation.FilterBySelection";
        public const string ActiveIconId = "d_winbtn_mac_max_a";
        public const string CloseIconId = "d_winbtn_mac_close_a";
    }


    internal static class SceneSwitcherEditor
    {
        private const string PkgId = "com.racer.ezsceneswitcher";
        private static RemoveRequest _removeRequest;

        public static readonly List<string> ScenePaths = new();

        public static readonly List<OpenSceneMode> OpenSceneModes = new()
        {
            OpenSceneMode.Single,
            OpenSceneMode.Additive,
            OpenSceneMode.AdditiveWithoutLoading,
        };

        public static OpenSceneMode OpenSceneMode { get; set; }
        public static (string, string) SelectedSceneData { get; set; }


        public static Scene GetActiveScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            return activeScene;
        }

        public static void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode);
        }

        public static void ActivateScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scenePath));
        }

        public static void UnloadScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.CloseScene(SceneManager.GetSceneByPath(scenePath), true);
        }

        public static void LoadScenes()
        {
            // refresh the list of scenes 
            ScenePaths.Clear();

            // find all scenes in the Assets folder
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });

            foreach (var sceneGuid in sceneGuids)
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset));
                ScenePaths.Add(scenePath);
            }
        }

        [MenuItem(Constants.ContextMenuPath + "Toggle", priority = 0)]
        private static void ToggleOnOff()
        {
            var lastActiveSceneView = SceneView.lastActiveSceneView;

            if (lastActiveSceneView == null)
            {
                Debug.LogWarning("Scene Switcher Overlay cannot work without an active Scene View window open.");
                return;
            }

            lastActiveSceneView.TryGetOverlay(Constants.OverlayID, out var overlay);

            if (!lastActiveSceneView.hasFocus)
            {
                Debug.LogWarning(
                    "An active Scene View window must be open, in order to toggle the Scene Switcher Overlay.");
                return;
            }

            if (overlay != null)
                overlay.displayed = !overlay.displayed;
        }

        [MenuItem(Constants.ContextMenuPath + "Remove Package", priority = 1)]
        private static void RemovePackage()
        {
            _removeRequest = Client.Remove(PkgId);
            EditorApplication.update += RemoveRequest;
        }

        private static void RemoveRequest()
        {
            if (!_removeRequest.IsCompleted) return;

            switch (_removeRequest.Status)
            {
                case >= StatusCode.Failure:
                    Debug.LogError($"Failed to remove package: '{PkgId}'\n{_removeRequest.Error.message}");
                    break;
            }

            EditorApplication.update -= RemoveRequest;
        }
    }


    internal static class Styles
    {
        public static readonly GUIContent SwitchToSceneBtn = new("Switch", "Switches to the selected scene by mode");

        public static readonly GUIContent CloseSceneBtn =
            new(EditorGUIUtility.IconContent(Constants.CloseIconId,
                "|Closes the selected scene that is either additively loaded or unloaded"));

        public static readonly GUIContent ActiveSceneBtn =
            new(EditorGUIUtility.IconContent(Constants.ActiveIconId, "|Sets the selected scene active, if its loaded"));

        public static readonly GUIContent PingSceneBtn =
            new(EditorGUIUtility.IconContent(Constants.PingIconId, "|Highlights the location of the selected scene"));
    }
}
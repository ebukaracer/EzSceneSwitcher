using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Racer.EzSceneSwitcher.Editor
{
    internal class PopupWin : PopupWindowContent
    {
        private (string, string) _selectedSceneData;
        private string _activeSceneName;

        private OpenSceneMode _selectedSceneMode;


        public PopupWin()
        {
            RefreshActiveScene();
            InitSelectedScene();
            _selectedSceneMode = SceneSwitcherEditor.OpenSceneMode;
        }

        private void RefreshActiveScene()
        {
            _activeSceneName = SceneSwitcherEditor.GetActiveScene().name;
        }

        private void InitSelectedScene()
        {
            if (string.IsNullOrEmpty(SceneSwitcherEditor.SelectedSceneData.Item1))
            {
                _selectedSceneData.Item1 = _activeSceneName;
                _selectedSceneData.Item2 = SceneSwitcherEditor.GetActiveScene().path;
            }
            else
                _selectedSceneData = SceneSwitcherEditor.SelectedSceneData;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 85);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Switch to a selected scene", EditorStyles.boldLabel);

            EditorGUILayout.Space(5);
            GUILayout.Label($"Active scene: {_activeSceneName}", EditorStyles.miniLabel);

            EditorGUILayout.BeginHorizontal();
            if (EditorGUILayout.DropdownButton(
                    new GUIContent(_selectedSceneData.Item1, $"Path: {_selectedSceneData.Item2}"),
                    FocusType.Passive,
                    GUILayout.Width(150)))
            {
                var sceneListMenu = new GenericMenu();

                foreach (var scenePath in SceneSwitcherEditor.ScenePaths)
                {
                    var sceneName = Path.GetFileNameWithoutExtension(scenePath);

                    sceneListMenu.AddItem(new GUIContent(sceneName), _selectedSceneData.Item1 == sceneName,
                        () => OnSceneListDropdownSelected(sceneName, scenePath));
                }

                sceneListMenu.ShowAsContext();
            }

            if (GUILayout.Button(Styles.PingSceneBtn, GUILayout.Width(30)))
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<SceneAsset>(_selectedSceneData.Item2));

            if (GUILayout.Button(Styles.ActiveSceneBtn))
            {
                SceneSwitcherEditor.ActivateScene(_selectedSceneData.Item2);
                RefreshActiveScene();
            }

            if (GUILayout.Button(Styles.CloseSceneBtn))
            {
                SceneSwitcherEditor.UnloadScene(_selectedSceneData.Item2);
                RefreshActiveScene();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (EditorGUILayout.DropdownButton(new GUIContent(_selectedSceneMode.ToString(),
                        "Loads the selected scene using this mode"),
                    FocusType.Passive,
                    GUILayout.Width(150)))
            {
                var sceneModeMenu = new GenericMenu();

                foreach (var sceneMode in SceneSwitcherEditor.OpenSceneModes)
                {
                    sceneModeMenu.AddItem(new GUIContent(sceneMode.ToString()), _selectedSceneMode == sceneMode,
                        () => OnSceneModeDropdownSelected(sceneMode));
                }

                sceneModeMenu.ShowAsContext();
            }

            if (GUILayout.Button(Styles.SwitchToSceneBtn))
            {
                SceneSwitcherEditor.OpenScene(_selectedSceneData.Item2);
                RefreshActiveScene();
            }

            EditorGUILayout.EndHorizontal();
        }


        private void OnSceneListDropdownSelected(string sceneName, string path)
        {
            SceneSwitcherEditor.SelectedSceneData = _selectedSceneData = (sceneName, path);
        }

        private void OnSceneModeDropdownSelected(OpenSceneMode openSceneMode)
        {
            SceneSwitcherEditor.OpenSceneMode = _selectedSceneMode = openSceneMode;
        }
    }
}
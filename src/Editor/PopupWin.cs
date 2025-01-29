using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Racer.EzSceneSwitcher.Editor
{
    internal class PopupWin : PopupWindowContent
    {
        private string _selectedSceneName;
        private string _selectedScenePath;


        public PopupWin()
        {
            var activeScene = SceneManager.GetActiveScene();

            _selectedSceneName = activeScene.name;
            _selectedScenePath = activeScene.path;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 70);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Switch or ping to selected scene", EditorStyles.boldLabel);

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            var menu = new GenericMenu();

            if (EditorGUILayout.DropdownButton(new GUIContent(_selectedSceneName,
                        $"Path: {_selectedScenePath}"),
                    FocusType.Passive,
                    GUILayout.Width(150)))
            {
                foreach (var scenePath in SceneSwitcherEditor.ScenePaths)
                {
                    var sceneName = Path.GetFileNameWithoutExtension(scenePath);

                    menu.AddItem(new GUIContent(sceneName), _selectedSceneName == sceneName,
                        () => OnDropdownItemSelected(sceneName, scenePath));
                }

                menu.ShowAsContext();
            }

            if (GUILayout.Button(Styles.SwitchToSceneBtn))
                SceneSwitcherEditor.OpenScene(_selectedScenePath);

            if (GUILayout.Button(Styles.PingSceneBtn, GUILayout.Width(30)))
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<SceneAsset>(_selectedScenePath));

            EditorGUILayout.EndHorizontal();
        }

        private void OnDropdownItemSelected(string sceneName, string scenePath)
        {
            _selectedSceneName = sceneName;
            _selectedScenePath = scenePath;
        }
    }
}
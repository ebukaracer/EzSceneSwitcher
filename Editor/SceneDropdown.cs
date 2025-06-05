using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Racer.EzSceneSwitcher.Editor
{
    [EditorToolbarElement(Constants.DropdownID, typeof(SceneView))]
    internal class SceneDropdown : EditorToolbarDropdown
    {
        public SceneDropdown()
        {
            var content =
                EditorGUIUtility.TrTextContentWithIcon(SceneManager.GetActiveScene().name, Constants.Tooltip,
                    Constants.SceneIconId);

            text = content.text;
            tooltip = content.tooltip;
            icon = content.image as Texture2D;

            // hacky: the text element is the second one here so we can set the padding
            //        but this is not really robust I think
            ElementAt(1).style.paddingLeft = 5;
            ElementAt(1).style.paddingRight = 5;

            clicked += ToggleDropdown;

            // keep track of panel events
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void ToggleDropdown()
        {
            UnityEditor.PopupWindow.Show(worldBound, new PopupWin());
        }

        protected virtual void OnAttachToPanel(AttachToPanelEvent evt)
        {
            // subscribe to the event where the play mode has changed
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            // subscribe to the event where scene assets were potentially modified
            EditorApplication.projectChanged += OnProjectChanged;

            // subscribe to the event where a scene has been opened
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        protected virtual void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            // unsubscribe from the event where the play mode has changed
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            // unsubscribe from the event where scene assets were potentially modified
            EditorApplication.projectChanged -= OnProjectChanged;

            // unsubscribe from the event where a scene has been opened
            EditorSceneManager.sceneOpened -= OnSceneOpened;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            switch (stateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    SetEnabled(true);
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    // don't allow changing scenes while in play mode
                    SetEnabled(false);
                    break;
            }
        }

        private void OnProjectChanged()
        {
            // update the dropdown label whenever the active scene has potentially be renamed
            text = SceneManager.GetActiveScene().name;
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            // update the dropdown label whenever a scene has been opened
            text = scene.name;
        }
    }
}
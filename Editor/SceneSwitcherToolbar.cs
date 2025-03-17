using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

namespace Racer.EzSceneSwitcher.Editor
{
    [Overlay(typeof(SceneView), Constants.OverlayID, Constants.DisplayName, defaultLayout = Layout.HorizontalToolbar),
     Icon(Constants.SceneIconId)]
    internal class SceneSwitcherToolbar : ToolbarOverlay
    {
        private SceneSwitcherToolbar() : base(Constants.DropdownID)
        {
        }

        public override void OnCreated()
        {
            // load the scenes when the toolbar overlay is initially created
            SceneSwitcherEditor.LoadScenes();

            // subscribe to the event where scene assets were potentially modified
            EditorApplication.projectChanged += OnProjectChanged;
        }

        // Called when an Overlay is about to be destroyed.
        // Usually this corresponds to the EditorWindow in which this Overlay resides closing. (Scene View in this case)
        public override void OnWillBeDestroyed()
        {
            // unsubscribe from the event where scene assets were potentially modified
            EditorApplication.projectChanged -= OnProjectChanged;
        }

        private static void OnProjectChanged()
        {
            // reload the scenes whenever scene assets were potentially modified
            SceneSwitcherEditor.LoadScenes();
        }
    }
}
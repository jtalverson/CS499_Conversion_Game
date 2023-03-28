using UnityEditor;

namespace UI.Pagination
{
    [CustomEditor(typeof(PagedRect_Scrollbar))]
    public partial class PagedRect_ScrollbarEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
        }
    }
}

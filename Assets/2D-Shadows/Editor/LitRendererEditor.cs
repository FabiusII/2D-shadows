using SpriteShadows.Scripts;
using UnityEditor;
using UnityEngine;

namespace SpriteShadows.Editor
{
    [CustomEditor(typeof(LitRenderer))]
    public class LitRendererEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var renderer = (target as LitRenderer);

            EditorGUILayout.LabelField(renderer.shadowMesh ? "Shadow mesh generated" : "Shadow mesh missing!");

            if (GUILayout.Button("Generate Shadow Mesh"))
            {
                var polygonCollider = renderer.gameObject.AddComponent<PolygonCollider2D>();
                renderer.shadowMesh = ShadowVolumeUtil.CreateMeshWithoutProjection(polygonCollider);
                EditorApplication.delayCall += () => DestroyImmediate(polygonCollider);
            }
        }
    }
}
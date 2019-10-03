using SpriteShadows.Scripts;
using UnityEditor;
using UnityEngine;

namespace SpriteShadows.Editor
{
    [CustomEditor(typeof(AnimatedLitRenderer))]
    public class AnimatedLitRendererEditor : UnityEditor.Editor
    {
        private Texture2D _spriteSheet;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var renderer = target as AnimatedLitRenderer;

            EditorGUILayout.LabelField(renderer.GetComponent<SpriteAndMeshSheet>()
                ? "Shadow data generated"
                : "Shadow data missing!");

            var spriterect = EditorGUILayout.GetControlRect();
            _spriteSheet =
                EditorGUI.ObjectField(spriterect, "Spritesheet", _spriteSheet, typeof(Texture2D),
                    false) as Texture2D;

            if (_spriteSheet)
            {
                if (GUILayout.Button("Generate shadow mesh"))
                {
                    GenerateShadowData(_spriteSheet);
                }
            }
        }

        protected void GenerateShadowData(Texture2D spritesheet)
        {
            var renderer = target as AnimatedLitRenderer;
            var spriteMeshSheet = renderer.GetComponent<SpriteAndMeshSheet>();
            if (spriteMeshSheet != null)
                spriteMeshSheet.Clear();
            else
                spriteMeshSheet = renderer.gameObject.AddComponent<SpriteAndMeshSheet>();

            var spriteSheetPath = AssetDatabase.GetAssetPath(spritesheet);
            var sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath);
            for (int i = 0; i < sprites.Length; ++i)
            {
                var sprite = sprites[i] as Sprite;
                if (sprite == null)
                    continue;

                renderer.renderer.sprite = sprite;
                var polygonCollider = renderer.gameObject.AddComponent<PolygonCollider2D>();
                var shadowMesh = ShadowVolumeUtil.CreateMeshWithoutProjection(polygonCollider);
                spriteMeshSheet.Add(sprite, shadowMesh);
                EditorApplication.delayCall += () => DestroyImmediate(polygonCollider);
            }
        }
    }
}
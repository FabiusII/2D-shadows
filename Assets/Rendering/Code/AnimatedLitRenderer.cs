using UnityEngine;

namespace SpriteShadows.Rendering.Lighting.Code
{
    public class AnimatedLitRenderer : LitRenderer
    {
        [SerializeField] private SpriteAndMeshSheet spriteMeshSheet;

        public void OnSpriteChanged()
        {
            shadowMesh = spriteMeshSheet.FindForSprite(renderer.sprite) ?? shadowMesh;
        }


        #region Editor

#if UNITY_EDITOR
        public Texture2D editorSpriteSheet;

        protected override void GenerateShadowDataEditor()
        {
            spriteMeshSheet = gameObject.GetComponent<SpriteAndMeshSheet>();
            if (spriteMeshSheet != null)
                spriteMeshSheet.Clear();
            else
                spriteMeshSheet = gameObject.AddComponent<SpriteAndMeshSheet>();

            var spriteSheetPath = UnityEditor.AssetDatabase.GetAssetPath(editorSpriteSheet);
            var sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath);
            for (int i = 0; i < sprites.Length; ++i)
            {
                var sprite = sprites[i] as Sprite;
                if (sprite == null)
                    continue;

                renderer.sprite = sprite;
                var polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
                var shadowMesh = ShadowVolumeUtil.CreateMeshWithoutProjection(polygonCollider);
                spriteMeshSheet.Add(sprite, shadowMesh);
                UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(polygonCollider);
            }
        }
#endif

        #endregion
    }
}
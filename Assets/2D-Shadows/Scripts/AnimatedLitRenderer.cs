using UnityEngine;

namespace SpriteShadows.Scripts
{
    public class AnimatedLitRenderer : LitRenderer
    {
        [SerializeField] private SpriteAndMeshSheet spriteMeshSheet;

        public void OnSpriteChanged()
        {
            shadowMesh = spriteMeshSheet.FindForSprite(renderer.sprite) ?? shadowMesh;
        }
    }
}
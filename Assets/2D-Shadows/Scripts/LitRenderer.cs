﻿using UnityEngine;

namespace SpriteShadows.Scripts
{
    public class LitRenderer : MonoBehaviour
    {
        public SpriteRenderer renderer;
        public Color shadowColor = Color.black;
        [HideInInspector] public Mesh shadowMesh;

        private static int _litForegroundLayer = -1;

        public bool ShadowCaster => renderer.sortingLayerID == _litForegroundLayer;

        private void Awake()
        {
            if (_litForegroundLayer == -1)
            {
                _litForegroundLayer = SortingLayer.NameToID("Lit Foreground");
            }
        }

        #region Editor        

#if UNITY_EDITOR
        [SerializeField] private bool drawGizmos;

        private void OnDrawGizmosSelected()
        {
            if (shadowMesh == null || !drawGizmos)
                return;

            var spotlight2DPos = FindObjectOfType<Spotlight2D>().transform.position;
            for (int i = 0; i < shadowMesh.triangles.Length; i += 3)
            {
                var first = transform.TransformPoint(shadowMesh.vertices[shadowMesh.triangles[i]]);
                var second = transform.TransformPoint(shadowMesh.vertices[shadowMesh.triangles[i + 1]]);
                var third = transform.TransformPoint(shadowMesh.vertices[shadowMesh.triangles[i + 2]]);

                if (shadowMesh.uv[shadowMesh.triangles[i]][0] > 0)
                    first += (first - spotlight2DPos) * 10;

                if (shadowMesh.uv[shadowMesh.triangles[i + 1]][0] > 0)
                    second += (second - spotlight2DPos) * 10;

                if (shadowMesh.uv[shadowMesh.triangles[i + 2]][0] > 0)
                    third += (third - spotlight2DPos) * 10;

                Gizmos.DrawLine(first, second);
                Gizmos.DrawLine(first, third);
                Gizmos.DrawLine(second, third);
            }
        }
#endif

        #endregion
    }
}
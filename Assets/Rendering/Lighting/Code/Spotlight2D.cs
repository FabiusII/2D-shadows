using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace SpriteShadows.Rendering.Lighting.Code
{
    public abstract class Spotlight2D : MonoBehaviour
    {
        [SerializeField] protected float range;
        [SerializeField] protected float angle;
        [SerializeField] protected Color color;
        [SerializeField] protected Camera sceneCamera;
        [SerializeField] protected Material shadowMaterial;
        [SerializeField] protected Material lightMaterial;
        [SerializeField] protected Material stencilMaterial;
        [SerializeField] [Range(0, 1)] protected float ambientLight;
        [SerializeField] protected Color ambientColor;
        [SerializeField] protected RenderTexture shadowMap;
        [SerializeField] private float strength;
        [SerializeField] [Range(0.1f, 179.9f)] private float maxAngle;
        [SerializeField] [Range(0.1f, 179.9f)] private float minAngle;

        private readonly Color _clearColor = Color.black;
        private readonly Vector3[] _lightMeshVertices = new Vector3[3];

        private CommandBuffer _shadowPass;
        private Mesh _lightMesh;

        public abstract List<LitRenderer> GetAllLitRenderers();

        public void DecreaseAngle(float decreaseBy)
        {
            angle = Mathf.Max(angle - decreaseBy, minAngle);
        }

        public void IncreaseAngle(float increaseBy)
        {
            angle = Mathf.Min(angle + increaseBy, maxAngle);
        }

        public bool InsideLightCone(Bounds bounds)
        {
            Vector2 upper, lower;
            GetLightDirection(out upper, out lower);

            Vector2 upperBound, lowerBound;
            GetDirectionToUpperAndLowerBounds(bounds, out upperBound, out lowerBound);

            var distanceUpper = upperBound.magnitude;
            var distanceLower = lowerBound.magnitude;
            if (distanceUpper > range && distanceLower > range)
                return false;

            var hit1 = upperBound.x < 0
                ? upperBound.y <= (lower * distanceUpper).y && lowerBound.y >= (upper * distanceLower).y
                : upperBound.y <= (upper * distanceUpper).y && lowerBound.y >= (distanceLower * lower).y;
            var hit2 = lowerBound.x < 0
                ? lowerBound.y <= (distanceLower * upper).y && upperBound.y >= (lower * distanceUpper).y
                : lowerBound.y <= (lower * distanceLower).y && upperBound.y >= (upper * distanceUpper).y;

            return hit1 || hit2;
        }

        public virtual float GetLightIntensity(float distance)
        {
            return minAngle / angle * strength * Mathf.Max(1 - distance / range, 0);
        }

        private void Start()
        {
            _lightMesh = new Mesh
            {
                vertices = new[] {Vector3.zero, Vector3.zero, Vector3.zero},
                normals = new Vector3[3],
                triangles = new[] {2, 1, 0}
            };

            _shadowPass = new CommandBuffer {name = "ShadowPass"};
            sceneCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _shadowPass);

            Shader.SetGlobalColor("_AmbientLight", ambientLight * ambientColor);
            Shader.SetGlobalColor("_SpotlightColor", color);
            Shader.SetGlobalTexture("_ShadowMap", shadowMap);
        }

        private void LateUpdate()
        {
            PrepareLightPass();
            CalculateLightShape();
            _shadowPass.SetGlobalVector("_LightPos", transform.position);
            _shadowPass.SetGlobalFloat("_LightRange", range);

            _shadowPass.DrawMesh(_lightMesh, Matrix4x4.identity, lightMaterial);

            var litRenderers = GetAllLitRenderers();
            foreach (var litRenderer in litRenderers)
            {
                if (!litRenderer.ShadowCaster || !litRenderer.isActiveAndEnabled)
//                    || !litRenderer.Renderer.isVisible || !InsideLightCone(litRenderer.Collider.bounds))
                    continue;

                var shadow = litRenderer.shadowMesh;
                if (shadow == null)
                    continue;

                _shadowPass.DrawRenderer(litRenderer.renderer, stencilMaterial);
                _shadowPass.SetGlobalColor("_ShadowColor", litRenderer.shadowColor);
                _shadowPass.DrawMesh(shadow, litRenderer.transform.localToWorldMatrix, shadowMaterial);
                _shadowPass.ClearRenderTarget(true, false, _clearColor);
            }
        }

        private void GetDirectionToUpperAndLowerBounds(Bounds bounds, out Vector2 upper, out Vector2 lower,
            float deviation = 0)
        {
            upper = new Vector2(bounds.center.x, bounds.max.y);
            lower = new Vector2(bounds.center.x, bounds.min.y);

            upper.y -= deviation;
            lower.y += deviation;

            upper -= new Vector2(transform.position.x, transform.position.y);
            lower -= new Vector2(transform.position.x, transform.position.y);
        }

        private void CalculateLightShape()
        {
            Vector2 upper;
            Vector2 lower;
            GetLightDirection(out upper, out lower);
            Vector2 twoDpos = transform.position;
            var projectedUpper = twoDpos + (upper * range);
            var projectedLower = twoDpos + (lower * range);

            _lightMeshVertices[0] = twoDpos;
            _lightMeshVertices[1] = projectedLower;
            _lightMeshVertices[2] = projectedUpper;
            _lightMesh.vertices = _lightMeshVertices;
        }

        private void GetLightDirection(out Vector2 upper, out Vector2 lower)
        {
            var lightRotation = transform.rotation.eulerAngles.z;
            var halfAngle = angle / 2;
            upper.x = Mathf.Cos((lightRotation + halfAngle) * Mathf.Deg2Rad);
            upper.y = Mathf.Sin((lightRotation + halfAngle) * Mathf.Deg2Rad);
            lower.x = Mathf.Cos((lightRotation - halfAngle) * Mathf.Deg2Rad);
            lower.y = Mathf.Sin((lightRotation - halfAngle) * Mathf.Deg2Rad);
        }

        private void PrepareLightPass()
        {
            _shadowPass.Clear();
            _shadowPass.SetRenderTarget(shadowMap);
            _shadowPass.ClearRenderTarget(true, true, _clearColor);
        }

        #region Editor

#if UNITY_EDITOR
        public bool drawLightCone;

        private void OnDrawGizmos()
        {
            if (drawLightCone && _lightMesh != null)
            {
                Gizmos.color = color;
                Gizmos.DrawWireMesh(_lightMesh);
            }
        }

        private void OnValidate()
        {
            Shader.SetGlobalColor("_AmbientLight", ambientLight * ambientColor);
            Shader.SetGlobalColor("_SpotlightColor", color);
        }
#endif

        #endregion
    }
}
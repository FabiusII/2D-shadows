#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace SpriteShadows.Rendering.Lighting.Code
{
    public static class ShadowVolumeUtil
    {
        private static readonly int[] Triangles = {0, 1, 2, 2, 1, 3};

        private static readonly Vector3[] Normals = new Vector3[4];

        //Use the x of the uv positions to decide whether or not to project the vertex
        private static readonly Vector2[] Projection = {Vector2.zero, Vector2.one, Vector2.zero, Vector2.one};

        public static Mesh CreateMeshWithoutProjection(PolygonCollider2D geometry)
        {
            var meshes = new List<Mesh>();

            for (int i = 0; i < geometry.pathCount; ++i)
            {
                var path = geometry.GetPath(i);
                if (path.Length == 0)
                    continue;

                var vertices = new List<Vector3>();
                var projectedVertices = new List<Vector3>();
                for (int j = 0; j < path.Length; ++j)
                {
                    Vector2 currentPoint = path[j];
                    vertices.Add(currentPoint);
                    projectedVertices.Add(currentPoint);
                }
                var mesh = CreateMeshFromVertices(vertices, projectedVertices);
                meshes.Add(mesh);
            }
            var consolidatedMesh = ConsolidateMeshes(meshes);
            var tris = consolidatedMesh.triangles.Length / 3f;
            var verts = consolidatedMesh.vertexCount;
            UnityEditor.MeshUtility.Optimize(consolidatedMesh);
            Debug.Log(string.Format("saved by optimizing: triangles {0}, vertices {1}", tris - consolidatedMesh.triangles.Length / 3f,
                verts - consolidatedMesh.vertexCount));
            return consolidatedMesh;
        }

        private static Mesh CreateMeshFromVertices(List<Vector3> vertices, List<Vector3> projectedVertices)
        {
            Debug.Assert(vertices.Count == projectedVertices.Count);
            var meshes = new List<Mesh>();
            for (int i = 0; i < vertices.Count - 1; ++i)
            {
                var mesh = new Mesh();
                mesh.vertices = new[] {vertices[i], projectedVertices[i], vertices[i + 1], projectedVertices[i + 1]};
                mesh.triangles = Triangles;
                mesh.normals = Normals;
                mesh.uv = Projection;
                meshes.Add(mesh);
            }
            return ConsolidateMeshes(meshes);
        }

        private static Mesh ConsolidateMeshes(List<Mesh> meshes)
        {
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            foreach (var mesh in meshes)
            {
                var combineInstance = new CombineInstance();
                combineInstance.mesh = mesh;
                combineInstances.Add(combineInstance);
            }
            var consolidatedMesh = new Mesh();
            consolidatedMesh.CombineMeshes(combineInstances.ToArray(), true, false);
            return consolidatedMesh;
        }
    }
}
#endif
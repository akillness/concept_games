using UnityEngine;
using Object = UnityEngine.Object;

namespace MossHarbor.Art
{
    public static class MeshUvGenerator
    {
        private const float AxisEpsilon = 0.0001f;

        public static bool EnsureRuntimeUvMesh(Mesh sourceMesh, out Mesh runtimeMesh)
        {
            runtimeMesh = sourceMesh;

            if (sourceMesh == null)
            {
                return false;
            }

            if (!sourceMesh.isReadable || sourceMesh.vertexCount == 0)
            {
                return false;
            }

            var existingUvs = sourceMesh.uv;
            if (existingUvs != null && existingUvs.Length == sourceMesh.vertexCount && existingUvs.Length > 0)
            {
                return false;
            }

            runtimeMesh = Object.Instantiate(sourceMesh);
            runtimeMesh.name = string.IsNullOrWhiteSpace(sourceMesh.name)
                ? "RuntimeUvMesh"
                : $"{sourceMesh.name}_RuntimeUv";
            runtimeMesh.RecalculateBounds();
            runtimeMesh.uv = BuildPlanarUvArray(runtimeMesh);
            return true;
        }

        private static Vector2[] BuildPlanarUvArray(Mesh mesh)
        {
            var vertices = mesh != null ? mesh.vertices : null;
            if (vertices == null || vertices.Length == 0)
            {
                return new Vector2[0];
            }

            var bounds = mesh.bounds;
            var useXzProjection = bounds.size.x > AxisEpsilon && bounds.size.z > AxisEpsilon;
            var useXyProjection = bounds.size.x > AxisEpsilon && bounds.size.y > AxisEpsilon;
            var uvs = new Vector2[vertices.Length];

            for (var i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];
                if (useXzProjection)
                {
                    uvs[i] = new Vector2(
                        Normalize(vertex.x, bounds.min.x, bounds.size.x),
                        Normalize(vertex.z, bounds.min.z, bounds.size.z));
                }
                else if (useXyProjection)
                {
                    uvs[i] = new Vector2(
                        Normalize(vertex.x, bounds.min.x, bounds.size.x),
                        Normalize(vertex.y, bounds.min.y, bounds.size.y));
                }
                else
                {
                    uvs[i] = Vector2.zero;
                }
            }

            return uvs;
        }

        private static float Normalize(float value, float min, float size)
        {
            if (size <= AxisEpsilon)
            {
                return 0f;
            }

            return Mathf.Clamp01((value - min) / size);
        }
    }
}

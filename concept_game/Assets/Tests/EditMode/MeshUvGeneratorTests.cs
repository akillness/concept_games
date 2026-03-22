using MossHarbor.Art;
using NUnit.Framework;
using UnityEngine;

namespace MossHarbor.Tests.EditMode
{
    public sealed class MeshUvGeneratorTests
    {
        [Test]
        public void EnsureRuntimeUvMesh_DoesNotCloneWhenUvsAlreadyExist()
        {
            var sourceMesh = CreateQuadMesh("ExistingUvMesh");
            sourceMesh.uv = new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f),
            };

            var generated = MeshUvGenerator.EnsureRuntimeUvMesh(sourceMesh, out var runtimeMesh);

            Assert.IsFalse(generated);
            Assert.AreSame(sourceMesh, runtimeMesh);

            Object.DestroyImmediate(sourceMesh);
        }

        [Test]
        public void EnsureRuntimeUvMesh_ClonesAndGeneratesUvsWhenMissing()
        {
            var sourceMesh = CreateQuadMesh("MissingUvMesh");

            var generated = MeshUvGenerator.EnsureRuntimeUvMesh(sourceMesh, out var runtimeMesh);

            Assert.IsTrue(generated);
            Assert.AreNotSame(sourceMesh, runtimeMesh);
            Assert.AreEqual(sourceMesh.vertexCount, runtimeMesh.uv.Length);

            Object.DestroyImmediate(sourceMesh);
            Object.DestroyImmediate(runtimeMesh);
        }

        [Test]
        public void EnsureRuntimeUvMesh_ProducesNormalizedUvsForTypicalBounds()
        {
            var sourceMesh = new Mesh
            {
                name = "TypicalBoundsMesh"
            };
            sourceMesh.vertices = new[]
            {
                new Vector3(-2f, 0f, -1f),
                new Vector3(2f, 0f, -1f),
                new Vector3(2f, 0f, 3f),
                new Vector3(-2f, 0f, 3f),
            };

            var generated = MeshUvGenerator.EnsureRuntimeUvMesh(sourceMesh, out var runtimeMesh);

            Assert.IsTrue(generated);
            Assert.AreEqual(sourceMesh.vertexCount, runtimeMesh.uv.Length);

            foreach (var uv in runtimeMesh.uv)
            {
                Assert.That(uv.x, Is.InRange(0f, 1f));
                Assert.That(uv.y, Is.InRange(0f, 1f));
            }

            Object.DestroyImmediate(sourceMesh);
            Object.DestroyImmediate(runtimeMesh);
        }

        private static Mesh CreateQuadMesh(string meshName)
        {
            return new Mesh
            {
                name = meshName,
                vertices = new[]
                {
                    new Vector3(-1f, 0f, -1f),
                    new Vector3(1f, 0f, -1f),
                    new Vector3(1f, 0f, 1f),
                    new Vector3(-1f, 0f, 1f),
                },
                triangles = new[]
                {
                    0, 1, 2,
                    0, 2, 3,
                },
            };
        }
    }
}

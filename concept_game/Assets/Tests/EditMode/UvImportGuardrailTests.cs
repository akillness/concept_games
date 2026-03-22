using MossHarbor.Art;
using NUnit.Framework;
using UnityEngine;

namespace MossHarbor.Tests.EditMode
{
    public sealed class UvImportGuardrailTests
    {
        [Test]
        public void ValidateMesh_ReturnsCleanReport_WhenReadableMeshHasCompletePrimaryUv()
        {
            var mesh = CreateQuadMesh("CompleteUvMesh");
            mesh.uv = new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f),
            };

            var report = UvImportGuardrail.AuditMesh(mesh, "Assets/Test/CompleteUvMesh.asset", mesh.name);

            Assert.IsFalse(report.HasIssues);
            Assert.AreEqual(1, report.meshCount);
            Assert.AreEqual(1, report.readableMeshCount);
            Assert.IsTrue(UvImportGuardrail.HasValidUvChannel(mesh, 0, out var uvCount));
            Assert.AreEqual(mesh.vertexCount, uvCount);

            Object.DestroyImmediate(mesh);
        }

        [Test]
        public void ValidateMesh_FlagsMissingPrimaryUvChannel()
        {
            var mesh = CreateQuadMesh("MissingPrimaryUvMesh");

            var report = UvImportGuardrail.AuditMesh(mesh, "Assets/Test/MissingPrimaryUvMesh.asset", mesh.name);

            Assert.IsTrue(report.HasIssues);
            Assert.That(report.Issues[0].message, Does.Contain("UV0"));
            Assert.IsFalse(UvImportGuardrail.HasValidUvChannel(mesh, 0, out _));

            Object.DestroyImmediate(mesh);
        }

        [Test]
        public void ValidateMesh_FlagsUnreadableMeshBeforeUvInspection()
        {
            var mesh = CreateQuadMesh("UnreadableMesh");
            mesh.UploadMeshData(true);

            var report = UvImportGuardrail.AuditMesh(mesh, "Assets/Test/UnreadableMesh.asset", mesh.name);

            Assert.IsTrue(report.HasIssues);
            Assert.That(report.Issues[0].message, Does.Contain("not readable"));
            Assert.AreEqual(1, report.meshCount);
            Assert.AreEqual(0, report.readableMeshCount);
            Assert.IsFalse(UvImportGuardrail.HasValidUvChannel(mesh, 0, out _));

            Object.DestroyImmediate(mesh);
        }

        [Test]
        public void ValidateUvCount_ReturnsFalse_WhenLengthDiffers()
        {
            Assert.IsFalse(UvImportGuardrail.IsValidUvCount(4, 2));
            Assert.IsTrue(UvImportGuardrail.IsValidUvCount(4, 4));
            Assert.IsFalse(UvImportGuardrail.IsValidUvCount(0, 0));
        }

        [Test]
        public void WarningOnlyIssue_DoesNotMarkReportAsFailed()
        {
            var report = new UvGuardrailReport();
            report.AddIssue("Assets/Test/SecondaryUvMismatchMesh.asset", "SecondaryUvMismatchMesh", "UV1 count mismatch.", critical: false);

            Assert.IsFalse(report.HasIssues);
            Assert.AreEqual(0, report.CriticalIssueCount);
            Assert.AreEqual(1, report.WarningIssueCount);
            Assert.That(report.Issues[0].message, Does.Contain("UV1"));
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

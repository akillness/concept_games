using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MossHarbor.Art
{
    public sealed class UvGuardrailIssue
    {
        public UvGuardrailIssue(string assetPath, string sourceName, string message, bool critical = true)
        {
            this.assetPath = assetPath;
            this.sourceName = sourceName;
            this.message = message;
            this.critical = critical;
        }

        public string assetPath { get; }
        public string sourceName { get; }
        public string message { get; }
        public bool critical { get; }
    }

    public sealed class UvGuardrailReport
    {
        private readonly List<UvGuardrailIssue> _issues = new List<UvGuardrailIssue>();

        public int targetCount { get; private set; }
        public int meshCount { get; private set; }
        public int readableMeshCount { get; private set; }

        public IReadOnlyList<UvGuardrailIssue> Issues => _issues;
        public int CriticalIssueCount
        {
            get
            {
                var count = 0;
                for (var i = 0; i < _issues.Count; i++)
                {
                    if (_issues[i].critical)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public int WarningIssueCount => _issues.Count - CriticalIssueCount;
        public bool HasIssues => CriticalIssueCount > 0;

        public void RecordTarget()
        {
            targetCount++;
        }

        public void RecordMesh(bool readable)
        {
            meshCount++;
            if (readable)
            {
                readableMeshCount++;
            }
        }

        public void AddIssue(string assetPath, string sourceName, string message, bool critical = true)
        {
            _issues.Add(new UvGuardrailIssue(assetPath, sourceName, message, critical));
        }

        public void Merge(UvGuardrailReport other)
        {
            if (other == null)
            {
                return;
            }

            targetCount += other.targetCount;
            meshCount += other.meshCount;
            readableMeshCount += other.readableMeshCount;
            _issues.AddRange(other._issues);
        }

        public string ToSummaryString(int maxIssueLines = 6)
        {
            var builder = new StringBuilder();
            builder.Append("UV Guardrail: ");
            builder.Append(HasIssues ? "FAIL" : "OK");
            builder.Append(" | targets=");
            builder.Append(targetCount);
            builder.Append(", meshes=");
            builder.Append(meshCount);
            builder.Append(", readable=");
            builder.Append(readableMeshCount);
            builder.Append(", critical=");
            builder.Append(CriticalIssueCount);
            builder.Append(", warnings=");
            builder.Append(WarningIssueCount);

            var limit = Mathf.Max(0, maxIssueLines);
            for (var i = 0; i < _issues.Count && i < limit; i++)
            {
                var issue = _issues[i];
                builder.AppendLine();
                builder.Append("- ");
                if (!string.IsNullOrWhiteSpace(issue.assetPath))
                {
                    builder.Append(issue.assetPath);
                    builder.Append(" :: ");
                }

                if (!string.IsNullOrWhiteSpace(issue.sourceName))
                {
                    builder.Append(issue.sourceName);
                    builder.Append(" - ");
                }

                builder.Append(issue.message);
            }

            if (_issues.Count > limit)
            {
                builder.AppendLine();
                builder.Append("- ");
                builder.Append(_issues.Count - limit);
                builder.Append(" more issue(s) omitted");
            }

            return builder.ToString();
        }
    }

    public static class UvImportGuardrail
    {
        private const int MaxUvChannels = 8;
        private static readonly List<Vector4> UvBuffer = new List<Vector4>();

        public static UvGuardrailReport AuditGameObject(GameObject root, string assetPath = null)
        {
            var report = new UvGuardrailReport();
            report.RecordTarget();

            if (root == null)
            {
                report.AddIssue(assetPath, "<null>", "Target GameObject is missing.");
                return report;
            }

            foreach (var meshFilter in root.GetComponentsInChildren<MeshFilter>(true))
            {
                var sourceName = BuildHierarchyPath(meshFilter.transform);
                if (meshFilter.sharedMesh == null)
                {
                    report.RecordMesh(false);
                    report.AddIssue(assetPath, sourceName, "MeshFilter does not reference a mesh.");
                    continue;
                }

                report.Merge(AuditMesh(meshFilter.sharedMesh, assetPath, sourceName));
            }

            foreach (var skinnedMeshRenderer in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                var sourceName = BuildHierarchyPath(skinnedMeshRenderer.transform);
                if (skinnedMeshRenderer.sharedMesh == null)
                {
                    report.RecordMesh(false);
                    report.AddIssue(assetPath, sourceName, "SkinnedMeshRenderer does not reference a mesh.");
                    continue;
                }

                report.Merge(AuditMesh(skinnedMeshRenderer.sharedMesh, assetPath, sourceName));
            }

            return report;
        }

        public static UvGuardrailReport AuditMesh(Mesh mesh, string assetPath = null, string sourceName = null)
        {
            var report = new UvGuardrailReport();
            report.RecordTarget();

            if (mesh == null)
            {
                report.AddIssue(assetPath, sourceName ?? "<mesh>", "Mesh is missing.");
                return report;
            }

            var vertexCount = mesh.vertexCount;
            var isReadable = mesh.isReadable;
            report.RecordMesh(isReadable);

            if (!isReadable)
            {
                report.AddIssue(assetPath, sourceName ?? mesh.name, "Mesh is not readable. UV channels cannot be audited.");
                return report;
            }

            if (vertexCount <= 0)
            {
                report.AddIssue(assetPath, sourceName ?? mesh.name, "Mesh has no vertices.");
                return report;
            }

            for (var channelIndex = 0; channelIndex < MaxUvChannels; channelIndex++)
            {
                if (!TryGetUvCount(mesh, channelIndex, out var uvCount))
                {
                    if (channelIndex == 0)
                    {
                        report.AddIssue(assetPath, sourceName ?? mesh.name, "UV0 is missing.");
                    }

                    continue;
                }

                if (uvCount != vertexCount)
                {
                    report.AddIssue(
                        assetPath,
                        sourceName ?? mesh.name,
                        $"UV{channelIndex} has {uvCount} entries but mesh has {vertexCount} vertices.",
                        critical: channelIndex == 0);
                }
            }

            return report;
        }

        public static bool IsValidUvCount(int vertexCount, int uvCount)
        {
            return vertexCount > 0 && uvCount == vertexCount;
        }

        public static bool HasValidUvChannel(Mesh mesh, int channelIndex, out int uvCount)
        {
            uvCount = 0;
            if (mesh == null || channelIndex < 0 || channelIndex >= MaxUvChannels || !mesh.isReadable)
            {
                return false;
            }

            return TryGetUvCount(mesh, channelIndex, out uvCount) && IsValidUvCount(mesh.vertexCount, uvCount);
        }

        private static bool TryGetUvCount(Mesh mesh, int channelIndex, out int uvCount)
        {
            uvCount = 0;
            if (mesh == null || channelIndex < 0 || channelIndex >= MaxUvChannels)
            {
                return false;
            }

            if (channelIndex == 0)
            {
                var uvs = mesh.uv;
                uvCount = uvs != null ? uvs.Length : 0;
                return uvCount > 0;
            }

            if (channelIndex == 1)
            {
                var uvs = mesh.uv2;
                uvCount = uvs != null ? uvs.Length : 0;
                return uvCount > 0;
            }

            if (channelIndex == 2)
            {
                var uvs = mesh.uv3;
                uvCount = uvs != null ? uvs.Length : 0;
                return uvCount > 0;
            }

            if (channelIndex == 3)
            {
                var uvs = mesh.uv4;
                uvCount = uvs != null ? uvs.Length : 0;
                return uvCount > 0;
            }

            UvBuffer.Clear();
            mesh.GetUVs(channelIndex, UvBuffer);
            uvCount = UvBuffer.Count;
            return uvCount > 0;
        }

        private static string BuildHierarchyPath(Transform transform)
        {
            if (transform == null)
            {
                return string.Empty;
            }

            var path = new List<string>();
            var current = transform;
            while (current != null)
            {
                path.Add(current.name);
                current = current.parent;
            }

            path.Reverse();
            return string.Join("/", path.ToArray());
        }
    }
}

using MossHarbor.Art;
using UnityEditor;
using UnityEngine;

namespace MossHarbor.EditorTools
{
    public static class UvImportGuardrailMenu
    {
        private static readonly string[] CriticalAssetPaths =
        {
            "Assets/Art/Resources/Art/Characters/player_avatar.prefab",
            "Assets/Art/Characters/StylizedCharacterPack/Models/Leopard.fbx",
        };

        [MenuItem("Tools/Moss Harbor/Validation/Audit Selected UV Guardrail")]
        public static void AuditSelected()
        {
            var report = AuditSelectionOrFallback();
            LogReport(report);
        }

        [MenuItem("Tools/Moss Harbor/Validation/Audit Player UV Guardrail")]
        public static void AuditPlayerCriticalAssets()
        {
            var report = AuditCriticalAssets();
            LogReport(report);
        }

        private static UvGuardrailReport AuditSelectionOrFallback()
        {
            var report = new UvGuardrailReport();
            var auditedAny = false;

            foreach (var selectedObject in Selection.objects)
            {
                if (TryAuditObject(selectedObject, report))
                {
                    auditedAny = true;
                }
            }

            if (!auditedAny)
            {
                report.Merge(AuditCriticalAssets());
            }

            return report;
        }

        private static bool TryAuditObject(Object target, UvGuardrailReport report)
        {
            if (target is GameObject gameObject)
            {
                report.Merge(UvImportGuardrail.AuditGameObject(gameObject, AssetDatabase.GetAssetPath(gameObject)));
                return true;
            }

            if (target is Mesh mesh)
            {
                report.Merge(UvImportGuardrail.AuditMesh(mesh, AssetDatabase.GetAssetPath(mesh), mesh.name));
                return true;
            }

            return false;
        }

        private static UvGuardrailReport AuditCriticalAssets()
        {
            var report = new UvGuardrailReport();

            foreach (var assetPath in CriticalAssetPaths)
            {
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (gameObject == null)
                {
                    report.AddIssue(assetPath, assetPath, "Asset could not be loaded for UV audit.");
                    continue;
                }

                report.Merge(UvImportGuardrail.AuditGameObject(gameObject, assetPath));
            }

            return report;
        }

        private static void LogReport(UvGuardrailReport report)
        {
            if (report == null)
            {
                Debug.LogWarning("UV Guardrail: FAIL | report missing");
                return;
            }

            if (report.HasIssues)
            {
                Debug.LogWarning(report.ToSummaryString());
                return;
            }

            Debug.Log(report.ToSummaryString());
        }
    }
}

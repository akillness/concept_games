using MossHarbor.Expedition;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MossHarbor.Tests.EditMode
{
    public sealed class ExpeditionCameraOcclusionRulesTests
    {
        [Test]
        public void ResolveAdjustedPosition_PullsCameraForwardWhenGeometryBlocksView()
        {
            var blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            try
            {
                blocker.transform.position = new Vector3(0f, 1.45f, -3f);
                blocker.transform.localScale = new Vector3(2f, 2f, 0.5f);
                Physics.SyncTransforms();

                var focus = new Vector3(0f, 1.45f, 0f);
                var desired = new Vector3(0f, 1.45f, -8f);

                var resolved = ExpeditionCameraOcclusionRules.ResolveAdjustedPosition(focus, desired, 0.2f, 0.25f, 2f);

                Assert.Greater(resolved.z, desired.z);
                Assert.Less(resolved.z, -2f);
            }
            finally
            {
                Object.DestroyImmediate(blocker);
            }
        }
    }
}

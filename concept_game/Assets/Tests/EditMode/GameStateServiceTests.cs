using MossHarbor.Core;
using NUnit.Framework;

namespace MossHarbor.Tests.EditMode
{
    public sealed class GameStateServiceTests
    {
        [Test]
        public void SetState_ChangesCurrentState()
        {
            var service = new GameStateService();

            service.SetState(GameFlowState.Expedition);

            Assert.AreEqual(GameFlowState.Expedition, service.CurrentState);
        }
    }
}

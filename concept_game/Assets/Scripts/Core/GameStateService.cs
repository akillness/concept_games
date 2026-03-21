using System;

namespace MossHarbor.Core
{
    public enum GameFlowState
    {
        Boot = 0,
        MainMenu = 1,
        Hub = 2,
        Expedition = 3,
        Results = 4,
    }

    public sealed class GameStateService
    {
        public GameFlowState CurrentState { get; private set; } = GameFlowState.Boot;

        public event Action<GameFlowState> StateChanged;

        public void SetState(GameFlowState nextState)
        {
            if (CurrentState == nextState)
            {
                return;
            }

            CurrentState = nextState;
            StateChanged?.Invoke(CurrentState);
        }
    }
}

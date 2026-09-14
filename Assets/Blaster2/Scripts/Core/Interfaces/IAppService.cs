
namespace TheGamerUrso.Core
{
    public interface IAppService
    {
        public GameStateEnum CurrentGameState { get; }
        public bool IsPaused { get; }
        void LoadMainMenu();
        void ResetLevel();
        void PauseTheGame(bool value);
        void SetGameState(GameStateEnum nextGameState);
    }
}

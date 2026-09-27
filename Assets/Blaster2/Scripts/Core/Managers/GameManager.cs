using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;


namespace TheGamerUrso.Core
{
    public enum GameStateEnum
    {
        PRELOAD, GAME, PAUSED, QUITING
    }

    [DisallowMultipleComponent]
    public class GameManager : ServiceComponent<IAppService> , IAppService
    {
        private static float DefaultTimeDeltaScale;   
        private bool autoKillMode;
        private bool useSafeMode;
        private LogBehaviour logBehaviour;

        public GameStateEnum CurrentGameState { get; private set; } = GameStateEnum.PRELOAD;
        public bool IsPaused { get; set; }

        protected override void Awake()
        {
            base.Awake();
            DefaultTimeDeltaScale = Time.fixedDeltaTime;
            DOTween.Init(autoKillMode, useSafeMode, logBehaviour);                 
        }      

        public void SetGameState(GameStateEnum nextGameState)
        {
            var previosuGameState = CurrentGameState;
            CurrentGameState = nextGameState;
        }

        public void Pause()
        {
            if(CurrentGameState == GameStateEnum.PAUSED)
            {
                return;
            }

            if (CurrentGameState == GameStateEnum.GAME)
            {
                SetGameState(GameStateEnum.PAUSED);
                Time.timeScale = 0;
                Time.fixedDeltaTime = 0;
                IsPaused = true;
            }
        }
        public void Unpause()
        {
            if (CurrentGameState == GameStateEnum.GAME)
            {
                return;
            }

            if (CurrentGameState == GameStateEnum.PAUSED)
            {
                SetGameState(GameStateEnum.GAME);
                Time.timeScale = 1;
                Time.fixedDeltaTime = DefaultTimeDeltaScale;
                IsPaused = false;
            }
        }

        public void LoadMainMenu()
        {
            SceneLoader.LoadScene(LevelEnum.Main);
        }

        public void ResetLevel()
        {
            SceneLoader.RestartLevel();
        }
    }
}



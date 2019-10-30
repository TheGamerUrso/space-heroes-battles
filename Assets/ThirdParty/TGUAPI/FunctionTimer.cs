using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheGamerUrso.Utils
{
    public class FunctionTimer
    {
        private static List<FunctionTimer> activeTimerList;
        private static GameObject initGameObject;

        private static void InitIfNeeded()
        {
            if (initGameObject == null)
            {
                initGameObject = new GameObject("EventHandler_initGameObject");
                activeTimerList = new List<FunctionTimer>();
            }
        }


        private Action action;
        private float timer;
        private string timerName;
        private GameObject gameObject;
        private bool isDestroyed;

        public static FunctionTimer Create(Action action, float timer, string timerName = null)
        {
            InitIfNeeded();
            GameObject gameObject = new GameObject("FunctionTimer", typeof(MonoBehaviorHook));

            FunctionTimer eventHandler = new FunctionTimer(action, timer, timerName, gameObject);

            gameObject.GetComponent<MonoBehaviorHook>().onUpdate = eventHandler.Update;

            activeTimerList.Add(eventHandler);
            return eventHandler;
        }

        public static void RemoveTimer(FunctionTimer eventHandler)
        {
            InitIfNeeded();
            activeTimerList.Remove(eventHandler);
        }

        public static void StopTimer(string timerName)
        {
            for (int i = 0; i < activeTimerList.Count; i++)
            {
                if (activeTimerList[i].timerName == timerName)
                {
                    activeTimerList[i].DestroySelf();
                    i--;
                }
            }
        }

        private class MonoBehaviorHook : MonoBehaviour
        {
            public Action onUpdate;
            private void Update()
            {
                if (onUpdate != null) onUpdate();
            }
        }


        public FunctionTimer(Action action, float timer, string timerName, GameObject gameObject)
        {
            this.action = action;
            this.timer = timer;
            this.timerName = timerName;
            this.gameObject = gameObject;
            isDestroyed = false;
        }

        public void Update()
        {
            if (!isDestroyed)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    action();
                    DestroySelf();
                }
            }
        }

        private void DestroySelf()
        {       
            isDestroyed = true;
            UnityEngine.Object.Destroy(gameObject);
            RemoveTimer(this); 
        }

    }

}

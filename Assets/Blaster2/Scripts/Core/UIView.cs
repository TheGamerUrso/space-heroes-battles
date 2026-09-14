using System;
using TheGamerUrso.Core;
using UnityEngine;

namespace TheGamerUrso.UI
{
    public abstract class UIView : MonoBehaviour
    {
        [SerializeField] protected GameObject panel;
        [SerializeField] protected CanvasGroup canvasGroup;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        //====================================================================================================
        public void Open()
        {
            panel.SetActive(true);
            OnOpened();
        }
        public void Close()
        {
            panel.SetActive(false);
            OnClosed();
        }
        //====================================================================================================
        protected virtual void OnOpened() { }
        //====================================================================================================
        protected virtual void OnClosed() { }
    }
}

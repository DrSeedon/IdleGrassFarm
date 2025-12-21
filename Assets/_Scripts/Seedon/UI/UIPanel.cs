using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Cysharp.Threading.Tasks;

namespace Seedon
{
    /// <summary>
    /// Класс для управления UI панелью.
    /// </summary>
    public class UIPanel : MonoBehaviour
    {
        [HideInInspector] public UIPanelsManager uiPanelsManager;
        public UnityEvent OnShow;
        public UnityEvent OnHide;
        [SerializeField] private bool _hideOnStart = false;

        private void Start()
        {
            if (_hideOnStart)
            {
                Hide();
            }
        }

        public async UniTask Show()
        {
            if (uiPanelsManager != null)
            {
                uiPanelsManager.HideAll();
            }

            gameObject.SetActive(true);
            OnShow?.Invoke();
            await UniTask.Yield();
        }

        public void Hide()
        {
            OnHide?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
﻿using System;
using AnimationPro.RunTime;
using Core.Input;
using Core.Utility.UI.Component;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace Module.UI.Title.Credit
{
    public class CreditManager : UIManager
    {
        [SerializeField] private ScrollContent scrollable;
        [SerializeField] private HoldVisibleObject holdVisual;
        
        public event Action OnCreditFinished;

        private InputEvent anyKeyEvent;
        
        private void Start()
        {
            anyKeyEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.AnyKey);
            scrollable.OnScrollFinished += OnCreditFinished;
            holdVisual.OnHoldFinished += OnCreditFinished;
        }

        public override void Initialized(ContentTransform content, bool isReset = false)
        {
            base.Initialized(content, isReset);
            holdVisual.SetVisible(true);
            scrollable.Play();
        }

        private void Update()
        {
            if (!scrollable.IsEnable) return;
            float value = anyKeyEvent.ReadValue<float>();
            if (holdVisual.IsVisible == value > 0 && holdVisual.IsVisible)
            {
                holdVisual.UpdateHoldTime(Time.deltaTime);
            }
            holdVisual.SetVisible(value > 0);
        }
    }
}
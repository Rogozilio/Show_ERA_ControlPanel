using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UISceneTool.Scripts
{
    public class ClickSoundManipulator : PointerManipulator
    {
        private Action _onClickAction;

        public ClickSoundManipulator(Action onClickAction)
        {
            _onClickAction = onClickAction;
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
        }
        public void OnPointerDown(PointerDownEvent evt)
        {
            _onClickAction?.Invoke();
        }
    }
}
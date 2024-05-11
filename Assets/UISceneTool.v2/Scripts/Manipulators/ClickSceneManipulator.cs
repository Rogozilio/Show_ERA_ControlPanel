using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UISceneTool.Scripts
{
    public class ClickSceneManipulator : PointerManipulator
    {
        private bool _isLeavePointer;
        
        private Action _onClickAction;
        private UnityEvent _onClickEvent;

        public ClickSceneManipulator(Action onClickAction, UnityEvent onClickEvent)
        {
            _onClickAction = onClickAction;
            _onClickEvent = onClickEvent;
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            target.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
        }
        private void OnPointerEnter(PointerEnterEvent evt)
        {
            _isLeavePointer = false;
        }
        
        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            _isLeavePointer = true;
        }
        public void OnPointerDown(PointerDownEvent evt)
        {
            if(_isLeavePointer) return;
            
            _onClickAction?.Invoke();
            _onClickEvent?.Invoke();

            _isLeavePointer = false;
        }
    }
}
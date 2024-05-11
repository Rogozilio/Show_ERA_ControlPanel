using System;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UISceneTool.Scripts
{
    public class MouseScrollManipulator : PointerManipulator
    {
        private Action<float> _onClickAction;

        public MouseScrollManipulator(Action<float> onClickAction)
        {
            _onClickAction = onClickAction;
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<WheelEvent>(OnWheelScroll);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<WheelEvent>(OnWheelScroll);
        }
        private void OnWheelScroll(WheelEvent evt)
        {
            ChangeBarVolume(evt.delta.y);
            _onClickAction?.Invoke(evt.delta.y);
        }

        private void ChangeBarVolume(float deltaWheel)
        {
            var bar = target.Q<VisualElement>("SoundBar");
            var newScale = bar.transform.scale;
            newScale.x = Math.Clamp(newScale.x - deltaWheel, 0.05f, 1f);
            bar.transform.scale = newScale;
        }
    }
}
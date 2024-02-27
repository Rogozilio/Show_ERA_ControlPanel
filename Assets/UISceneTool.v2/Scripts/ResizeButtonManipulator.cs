using System.Linq;
using UnityEngine.UIElements;

namespace UISceneTool.Scripts
{
    public class ResizeButtonManipulator : PointerManipulator
    {
        private readonly float _toScale = 0.9f;

        private bool _isMouseDown;

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
        }

        public void OnPointerDown(PointerDownEvent evt)
        {
            if(_isMouseDown || target == null) return;
            
            DecreaseButton();
            _isMouseDown = true;
        }

        public void OnPointerUp(PointerUpEvent evt)
        {
            if(!_isMouseDown || target == null) return;
            
            IncreaseButton();
            _isMouseDown = false;
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            if(!_isMouseDown) return;
            
            IncreaseButton();
            _isMouseDown = false;
        }

        private void DecreaseButton()
        {
            target.transform.scale *= _toScale;
            target.Children().First().Children().First().transform.scale *= (1 / _toScale);
        }
        private void IncreaseButton()
        {
            target.transform.scale *= (1 / _toScale);
            target.Children().First().Children().First().transform.scale *= _toScale;
        }
    }
}
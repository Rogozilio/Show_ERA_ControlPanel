using UnityEngine;
using UnityEngine.UIElements;

namespace UISceneTool.Scripts
{
    public class MouseHoverManipulator : PointerManipulator
    {
        private bool _isPointerLeave;
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.parent.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.parent.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.parent.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.parent.UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            
        }
        private void OnPointerEnter(PointerEnterEvent evt)
        {
            _isPointerLeave = false;
            target.AddToClassList(
                target.ClassListContains("ColorWhite") ? "ColorWhiteHover" : "ColorGreenHover");
        } 
        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            _isPointerLeave = true;
            target.RemoveFromClassList(
                target.ClassListContains("ColorWhite") ? "ColorWhiteHover" : "ColorGreenHover");
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            target.RemoveFromClassList("ColorGreenHover");
            target.RemoveFromClassList("ColorWhiteHover");
        }
        private void OnPointerUp(PointerUpEvent evt)
        {
            if(_isPointerLeave) return;
            
            if (target.ClassListContains("ColorWhite"))
            {
                target.RemoveFromClassList("ColorGreenHover");
                target.AddToClassList("ColorWhiteHover");
            }
            else if (target.ClassListContains("ColorGreen"))
            {
                target.RemoveFromClassList("ColorWhiteHover");
                target.AddToClassList("ColorGreenHover");
            }
        }
    }
}
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
            target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.parent.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            
        }
        private void OnPointerEnter(PointerEnterEvent evt)
        {
            _isPointerLeave = false;
            target.AddToClassList(
                target.ClassListContains("ColorGreen") ? "ColorGreenHover" : "ColorWhiteHover");
        } 
        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            _isPointerLeave = true;
            target.RemoveFromClassList(
                target.ClassListContains("ColorGreen") ? "ColorGreenHover" : "ColorWhiteHover");
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
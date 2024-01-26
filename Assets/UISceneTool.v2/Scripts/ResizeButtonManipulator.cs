using System.Linq;
using UnityEngine.UIElements;

namespace UISceneTool.Scripts
{
    public class ResizeButtonManipulator : PointerManipulator
    {
        private readonly float _toScale = 0.9f;

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            DecreaseButton();
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            IncreaseButton();
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            IncreaseButton();
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
            //Fix Scale onсe
        }
    }
}
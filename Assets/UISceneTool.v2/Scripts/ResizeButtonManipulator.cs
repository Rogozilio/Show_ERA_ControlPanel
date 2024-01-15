using System.Linq;
using UnityEngine.UIElements;

namespace UISceneTool.Scripts
{
    public class ResizeButtonManipulator : PointerManipulator
    {
        private  readonly float _toScale = 0.9f;
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }
        
        private void OnPointerDown(PointerDownEvent evt)
        {
            target.transform.scale *= _toScale;
            target.Children().First().Children().First().transform.scale *= (1 / _toScale);
        }
        
        private void OnPointerUp(PointerUpEvent evt)
        {
            target.transform.scale *= (1 / _toScale);
            target.Children().First().Children().First().transform.scale *= _toScale;
        }
    }
}
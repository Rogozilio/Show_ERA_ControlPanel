using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ERA
{
    public class ButtonHoldSceneTool : ButtonSceneTool, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler,
        IPointerEnterHandler
    {
        [HideInInspector] public bool isPressed;
        [HideInInspector] public UnityEvent onHold;

        private void Update()
        {
            if (!isPressed) return;
            onHold?.Invoke();
        }

        public override void Activate()
        {
            base.Activate();
            _image.sprite = normal;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            _image.sprite = disabled;
        }

        public override void UseEventButton()
        {
            base.UseEventButton();

            if (isDisabled)
            {
                isPressed = false;
                return;
            }

            if (Input.GetKey(keyboardButton))
            {
                _image.sprite = push;
                onHold?.Invoke();
            }

            if (Input.GetKeyUp(keyboardButton))
            {
                _image.sprite = normal;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isDisabled) return;

            base.OnPointerEnter(eventData);
            _image.sprite = hover;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isDisabled) return;

            base.OnPointerDown(eventData);
            isPressed = true;
            _image.sprite = push;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isDisabled) return;

            isPressed = false;
            _image.sprite = _image.sprite == push ? hover : _image.sprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isDisabled) return;

            base.OnPointerExit(eventData);
            isPressed = false;
            _image.sprite = normal;
        }
    }
}
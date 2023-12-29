using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ERA
{
    public enum ButtonSceneToolType
    {
        None,
        Camera,
        Up,
        Down,
        Left,
        Right,
        StartReturn,
        Minus,
        Plus,
        Sound,
        Scenes
    }

    [Serializable]
    public class ButtonSceneTool : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public KeyCode keyboardButton;
        [Space] public ButtonSceneToolType type;
        public Sprite normal;
        public Sprite hover;
        public Sprite push;
        public string tooltip;
        public Sprite disabled;
        [HideInInspector] public bool isDisabled;
        [HideInInspector] public List<Condition> disabledConditions;

        protected Image _image;
        private UnityEvent<string> _onShowTooltip;
        private UnityEvent _onHideTooltip;
        private UnityEvent _onActivate;
        private UnityEvent _onDeactivate;

        public UnityAction<string> AddOnShowTooltip
        {
            set => _onShowTooltip.AddListener(value);
        }

        public UnityAction AddOnHideTooltip
        {
            set => _onHideTooltip.AddListener(value);
        }

        public UnityAction AddOnActivate
        {
            set => _onActivate.AddListener(value);
        }

        public UnityAction AddOnDeactivate
        {
            set => _onDeactivate.AddListener(value);
        }

        protected void Awake()
        {
            _image = GetComponent<Image>();
            _onShowTooltip = new UnityEvent<string>();
            _onHideTooltip = new UnityEvent();
            _onActivate = new UnityEvent();
            _onDeactivate = new UnityEvent();
        }

        public virtual void Activate()
        {
            isDisabled = false;
            _onActivate?.Invoke();
        }

        public virtual void Deactivate()
        {
            isDisabled = true;
            _onDeactivate?.Invoke();
        }

        public virtual void UseEventButton()
        {
            if (disabledConditions.Count == 0) return;
            foreach (var condition in disabledConditions)
            {
                if (!condition.GetResult())
                {
                    if (isDisabled) Activate();
                    return;
                }
            }

            if (!isDisabled) Deactivate();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _onShowTooltip?.Invoke(tooltip);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _onHideTooltip?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _onHideTooltip?.Invoke();
        }

        private void OnDestroy()
        {
            _onShowTooltip.RemoveAllListeners();
            _onHideTooltip.RemoveAllListeners();
        }
    }
}
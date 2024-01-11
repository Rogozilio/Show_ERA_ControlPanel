using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ERA
{
    public enum ClickType
    {
        Button,
        Switch,
        ToggleGroup
    }

    public class ButtonClickSceneTool : ButtonSceneTool, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler,
        IPointerEnterHandler, IPointerClickHandler
    {
        public enum ClickButtonState
        {
            Activate,
            Deactivate,
            Disabled
        }

        public ClickType clickType;
        public ClickButtonState clickButtonState;
        public Sprite normalOff;
        public Sprite hoverOff;
        public Sprite pushOff;
        public Sprite selected;
        [HideInInspector] public UnityEvent onClick;

        private TextMeshProUGUI _textMeshPro;

        private bool _isSelect;

        public bool IsSelect
        {
            get => _isSelect;
            set
            {
                if (clickType == ClickType.ToggleGroup)
                    IsSelected(value);
                _isSelect = value;
            }
        }

        public bool IsMouseHover => _image.sprite == hover;

        private void Awake()
        {
            base.Awake();
            _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void IsSelected(bool value)
        {
            if (value)
            {
                _image.sprite = selected;
                _textMeshPro.color = new Color32(51, 102, 0, 255);
            }
            else
            {
                _image.sprite = normal;
                _textMeshPro.color = Color.white;
            }
        }

        private void SwitchStateButton()
        {
            switch (clickButtonState)
            {
                case ClickButtonState.Activate:
                    clickButtonState = ClickButtonState.Deactivate;
                    _image.sprite = _image.sprite == push ? hoverOff : normalOff;
                    break;
                case ClickButtonState.Deactivate:
                    clickButtonState = ClickButtonState.Activate;
                    _image.sprite = _image.sprite == pushOff ? hover : normal;
                    break;
            }
        }

        public override void Activate()
        {
            base.Activate();
            clickButtonState = ClickButtonState.Activate;
            _image.sprite = _image.sprite == pushOff ? hover : normal;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            clickButtonState = ClickButtonState.Disabled;
            _image.sprite = disabled;
        }

        public override void UseEventButton()
        {
            base.UseEventButton();

            if (isDisabled) return;

            if (clickType == ClickType.Button)
            {
                if (Input.GetKeyDown(keyboardButton))
                {
                    _image.sprite = push;
                    onClick?.Invoke();
                }

                if (Input.GetKeyUp(keyboardButton))
                {
                    _image.sprite = normal;
                }

                return;
            }

            if (Input.GetKeyDown(keyboardButton))
            {
                SwitchStateButton();
                onClick?.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (clickButtonState == ClickButtonState.Disabled) return;

            base.OnPointerEnter(eventData);
            if (clickButtonState == ClickButtonState.Activate)
                _image.sprite = IsSelect ? _image.sprite : hover;
            else
                _image.sprite = IsSelect ? _image.sprite : hoverOff;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (clickButtonState == ClickButtonState.Disabled) return;

            base.OnPointerDown(eventData);
            if (clickType == ClickType.Button)
            {
                _image.sprite = push;
            }
            else if (clickType == ClickType.Switch)
            {
                _image.sprite = clickButtonState == ClickButtonState.Activate ? push : pushOff;
            }
            else if (clickType == ClickType.ToggleGroup)
            {
                if (!IsSelect)
                    onClick?.Invoke();
                IsSelect = true;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (clickButtonState == ClickButtonState.Disabled) return;

            if (clickType == ClickType.Button)
                _image.sprite = _image.sprite != normal ? hover : normal;
            else if (clickType == ClickType.Switch)
                IsSelect = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (clickButtonState == ClickButtonState.Disabled) return;

            base.OnPointerExit(eventData);
            if (clickType == ClickType.Button)
                _image.sprite = normal;
            else if (clickType == ClickType.Switch || clickType == ClickType.ToggleGroup && !_isSelect)
                _image.sprite = clickButtonState == ClickButtonState.Activate ? normal : normalOff;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickButtonState == ClickButtonState.Disabled) return;

            if (eventData.dragging) return;

            if (clickType == ClickType.Switch)
                SwitchStateButton();

            if (clickType != ClickType.ToggleGroup)
                onClick?.Invoke();
        }
    }

#if(UNITY_EDITOR)
    [CustomEditor(typeof(ButtonClickSceneTool))]
    public class ButtonClickSceneToolEditor : Editor
    {
        private SerializedProperty _propertyKeyboardButton;
        private SerializedProperty _propertySpriteNormal;
        private SerializedProperty _propertySpriteHover;
        private SerializedProperty _propertySpritePush;
        private SerializedProperty _propertySpriteSelected;
        private SerializedProperty _propertySpriteNormalOff;
        private SerializedProperty _propertySpriteHoverOff;
        private SerializedProperty _propertySpritePushOff;
        private SerializedProperty _propertySpriteDisabled;
        private SerializedProperty _propertyTooltip;
        private SerializedProperty _propertyClickType;
        private SerializedProperty _propertyOnClick;

        private void OnEnable()
        {
            _propertyKeyboardButton = serializedObject.FindProperty("keyboardButton");
            _propertySpriteNormal = serializedObject.FindProperty("normal");
            _propertySpriteHover = serializedObject.FindProperty("hover");
            _propertySpritePush = serializedObject.FindProperty("push");
            _propertySpriteSelected = serializedObject.FindProperty("selected");
            _propertySpriteNormalOff = serializedObject.FindProperty("normalOff");
            _propertySpriteHoverOff = serializedObject.FindProperty("hoverOff");
            _propertySpritePushOff = serializedObject.FindProperty("pushOff");
            _propertySpriteDisabled = serializedObject.FindProperty("disabled");
            _propertyTooltip = serializedObject.FindProperty("tooltip");
            _propertyClickType = serializedObject.FindProperty("clickType");
            _propertyOnClick = serializedObject.FindProperty("onClick");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_propertyKeyboardButton);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_propertySpriteNormal);
            EditorGUILayout.PropertyField(_propertySpriteHover);
            EditorGUILayout.PropertyField(_propertySpritePush);
            EditorGUILayout.PropertyField(_propertyTooltip);
            EditorGUILayout.PropertyField(_propertyClickType);

            if (_propertyClickType.enumValueIndex == (int)ClickType.ToggleGroup)
            {
                EditorGUILayout.PropertyField(_propertySpriteSelected);
            }
            else
            {
                EditorGUILayout.PropertyField(_propertySpriteNormalOff);
                EditorGUILayout.PropertyField(_propertySpriteHoverOff);
                EditorGUILayout.PropertyField(_propertySpritePushOff);
                EditorGUILayout.PropertyField(_propertySpriteDisabled);
            }

            EditorGUILayout.PropertyField(_propertyOnClick);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
using System;
using System.Collections.Generic;
using System.Reflection;
using ERA;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace ERA
{
    public class SceneTool : MonoBehaviour
    {
        public RectTransform window;
        public ButtonClickSceneTool buttonCamera;
        public ButtonHoldSceneTool buttonUp;
        public ButtonHoldSceneTool buttonDown;
        public ButtonHoldSceneTool buttonLeft;
        public ButtonHoldSceneTool buttonRight;
        public ButtonClickSceneTool buttonStartReturn;
        public ButtonHoldSceneTool buttonMinus;
        public ButtonHoldSceneTool buttonPlus;
        public ButtonClickSceneTool buttonSound;
        public GameObject scenes;
        public DragUI dragUI;
        public ChangeVolumeSound changeVolumeSound;
        public TooltipSceneTool tooltip;

        private ButtonClickSceneTool[] _buttonsScene;
        
        private void Start()
        {
            if (scenes.transform.childCount > 1)
            {
                ActivateButtonScene(scenes.transform.GetChild(0).GetComponent<ButtonClickSceneTool>());
            }

            buttonCamera.AddOnShowTooltip = ShowTooltip;
            buttonUp.AddOnShowTooltip = ShowTooltip;
            buttonDown.AddOnShowTooltip = ShowTooltip;
            buttonLeft.AddOnShowTooltip = ShowTooltip;
            buttonRight.AddOnShowTooltip = ShowTooltip;
            buttonStartReturn.AddOnShowTooltip = ShowTooltip;
            buttonMinus.AddOnShowTooltip = ShowTooltip;
            buttonPlus.AddOnShowTooltip = ShowTooltip;
            buttonSound.AddOnShowTooltip = ShowTooltip;

            buttonCamera.AddOnHideTooltip = HideTooltip;
            buttonUp.AddOnHideTooltip = HideTooltip;
            buttonDown.AddOnHideTooltip = HideTooltip;
            buttonLeft.AddOnHideTooltip = HideTooltip;
            buttonRight.AddOnHideTooltip = HideTooltip;
            buttonStartReturn.AddOnHideTooltip = HideTooltip;
            buttonMinus.AddOnHideTooltip = HideTooltip;
            buttonPlus.AddOnHideTooltip = HideTooltip;
            buttonSound.AddOnHideTooltip = HideTooltip;

            if (!scenes.activeSelf) return;
            _buttonsScene = new ButtonClickSceneTool[scenes.transform.childCount];

            for (var i = 0; i < scenes.transform.childCount; i++)
            {
                _buttonsScene[i] = scenes.transform.GetChild(i).GetComponent<ButtonClickSceneTool>();
                _buttonsScene[i].AddOnShowTooltip = ShowTooltip;
                _buttonsScene[i].AddOnHideTooltip = HideTooltip;
            }
        }

        private void ShowTooltip(string text)
        {
            tooltip.Show(text);
        }

        private void HideTooltip()
        {
            tooltip.Hide();
        }

        public void SwitchPanel()
        {
            buttonUp.gameObject.SetActive(!buttonUp.gameObject.activeSelf);
            buttonDown.gameObject.SetActive(!buttonDown.gameObject.activeSelf);
            buttonLeft.gameObject.SetActive(!buttonLeft.gameObject.activeSelf);
            buttonRight.gameObject.SetActive(!buttonRight.gameObject.activeSelf);
            buttonStartReturn.gameObject.SetActive(!buttonStartReturn.gameObject.activeSelf);
            buttonMinus.gameObject.SetActive(!buttonMinus.gameObject.activeSelf);
            buttonPlus.gameObject.SetActive(!buttonPlus.gameObject.activeSelf);
            buttonSound.gameObject.SetActive(!buttonSound.gameObject.activeSelf);
            if (scenes.transform.childCount > 1)
                scenes.SetActive(!scenes.activeSelf);
        }

        public void DeactivateAllButtonScene()
        {
            foreach (Transform button in scenes.transform)
            {
                button.GetComponent<ButtonClickSceneTool>().IsSelect = false;
            }
        }

        public void ActivateButtonScene(ButtonClickSceneTool clickButton)
        {
            clickButton.IsSelect = true;
        }

        private void Update()
        {
            buttonCamera.UseEventButton();
            buttonUp.UseEventButton();
            buttonDown.UseEventButton();
            buttonLeft.UseEventButton();
            buttonRight.UseEventButton();
            buttonStartReturn.UseEventButton();
            buttonMinus.UseEventButton();
            buttonPlus.UseEventButton();
            buttonSound.UseEventButton();
            if(_buttonsScene == null) return;
            foreach (var buttonScene in _buttonsScene)
            {
                buttonScene.UseEventButton();
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SceneTool))]
public class SceneToolEditor : Editor
{
    private SerializedProperty _window;
    private SerializedProperty _buttonCamera;
    private SerializedProperty _buttonUp;
    private SerializedProperty _buttonDown;
    private SerializedProperty _buttonLeft;
    private SerializedProperty _buttonRight;
    private SerializedProperty _buttonStartReturn;
    private SerializedProperty _buttonMinus;
    private SerializedProperty _buttonPlus;
    private SerializedProperty _buttonSound;
    private SerializedProperty _scenesParent;
    private List<SerializedObject> _scenes;
    private SerializedProperty _dragUI;
    private SerializedProperty _changeVolumeSound;

    private ButtonSceneToolType _activeButton;

    private Texture2D _textureCamera;
    private Texture2D _textureArrowUp;
    private Texture2D _textureArrowDown;
    private Texture2D _textureArrowLeft;
    private Texture2D _textureArrowRight;
    private Texture2D _textureStartReturn;
    private Texture2D _textureMinus;
    private Texture2D _texturePlus;
    private Texture2D _textureSound;
    private Texture2D _textureScenes;
    private Texture2D _textureCameraActive;
    private Texture2D _textureArrowUpActive;
    private Texture2D _textureArrowDownActive;
    private Texture2D _textureArrowLeftActive;
    private Texture2D _textureArrowRightActive;
    private Texture2D _textureStartReturnActive;
    private Texture2D _textureMinusActive;
    private Texture2D _texturePlusActive;
    private Texture2D _textureSoundActive;
    private Texture2D _textureScenesActive;
    private Texture2D _textureHover;

    private GUIStyle style;
    private GUILayoutOption[] sizeButton;

    private void OnEnable()
    {
        _window = serializedObject.FindProperty("window");
        _buttonCamera = serializedObject.FindProperty("buttonCamera");
        _buttonUp = serializedObject.FindProperty("buttonUp");
        _buttonDown = serializedObject.FindProperty("buttonDown");
        _buttonLeft = serializedObject.FindProperty("buttonLeft");
        _buttonRight = serializedObject.FindProperty("buttonRight");
        _buttonStartReturn = serializedObject.FindProperty("buttonStartReturn");
        _buttonMinus = serializedObject.FindProperty("buttonMinus");
        _buttonPlus = serializedObject.FindProperty("buttonPlus");
        _buttonSound = serializedObject.FindProperty("buttonSound");
        _scenesParent = serializedObject.FindProperty("scenes");
        _dragUI = serializedObject.FindProperty("dragUI");
        _changeVolumeSound = serializedObject.FindProperty("changeVolumeSound");

        _textureCamera = Resources.Load("Cam_clean") as Texture2D;
        _textureArrowUp = Resources.Load("Arrow_up_clean") as Texture2D;
        _textureArrowDown = Resources.Load("Arrow_down_clean") as Texture2D;
        _textureArrowLeft = Resources.Load("Arrow_left_clean") as Texture2D;
        _textureArrowRight = Resources.Load("Arrow_right_clean") as Texture2D;
        _textureStartReturn = Resources.Load("StartReturn_clean") as Texture2D;
        _textureMinus = Resources.Load("Minus_clean") as Texture2D;
        _texturePlus = Resources.Load("Plus_clean") as Texture2D;
        _textureSound = Resources.Load("Sound_clean") as Texture2D;
        _textureScenes = Resources.Load("Scenes") as Texture2D;
        _textureCameraActive = Resources.Load("Cam_push") as Texture2D;
        _textureArrowUpActive = Resources.Load("Arrow_up_push") as Texture2D;
        _textureArrowDownActive = Resources.Load("Arrow_down_push") as Texture2D;
        _textureArrowLeftActive = Resources.Load("Arrow_left_push") as Texture2D;
        _textureArrowRightActive = Resources.Load("Arrow_right_push") as Texture2D;
        _textureStartReturnActive = Resources.Load("StartReturn_push") as Texture2D;
        _textureMinusActive = Resources.Load("Minus_push") as Texture2D;
        _texturePlusActive = Resources.Load("Plus_push") as Texture2D;
        _textureSoundActive = Resources.Load("Sound_push") as Texture2D;
        _textureScenesActive = Resources.Load("Scenes_active") as Texture2D;
        _textureHover = Resources.Load("Hover") as Texture2D;

        style = new GUIStyle();
        style.padding = new RectOffset(3, 3, 3, 3);
        style.hover.background = _textureHover;

        sizeButton = new[] { GUILayout.Width(50f), GUILayout.Height(50f) };

        _activeButton = ButtonSceneToolType.None;

        var scenes = (GameObject)_scenesParent.objectReferenceValue;
        _scenes ??= new List<SerializedObject>();
        _scenes.Clear();
        for (var i = 0; i < scenes.transform.childCount; i++)
        {
            _scenes.Add(new SerializedObject(scenes.transform.GetChild(i).GetComponent<ButtonClickSceneTool>()));
        }
    }

    public override void OnInspectorGUI()
    {
        DrawInterfaceButtons();
        DrawOptionsActiveButton();
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawInterfaceButtons()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        _activeButton = DrawButton(ButtonSceneToolType.Plus, _texturePlus, _texturePlusActive);
        _activeButton = DrawButton(ButtonSceneToolType.Up, _textureArrowUp, _textureArrowUpActive);
        _activeButton = DrawButton(ButtonSceneToolType.Camera, _textureCamera, _textureCameraActive);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        _activeButton = DrawButton(ButtonSceneToolType.Left, _textureArrowLeft, _textureArrowLeftActive);
        _activeButton = DrawButton(ButtonSceneToolType.StartReturn, _textureStartReturn, _textureStartReturnActive);
        _activeButton = DrawButton(ButtonSceneToolType.Right, _textureArrowRight, _textureArrowRightActive);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        _activeButton = DrawButton(ButtonSceneToolType.Minus, _textureMinus, _textureMinusActive);
        _activeButton = DrawButton(ButtonSceneToolType.Down, _textureArrowDown, _textureArrowDownActive);
        _activeButton = DrawButton(ButtonSceneToolType.Sound, _textureSound, _textureSoundActive);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        _activeButton = DrawButton(ButtonSceneToolType.Scenes, _textureScenes, _textureScenesActive,
            new[] { GUILayout.Width(130f), GUILayout.Height(48f) });
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        Repaint();
    }

    private ButtonSceneToolType DrawButton(ButtonSceneToolType activeButton, Texture2D normal, Texture2D active)
    {
        var texture = IsButtonActive(activeButton) ? active : normal;
        var isActiveButton = GUILayout.Button(texture, style, sizeButton) || _activeButton == activeButton;
        return isActiveButton ? activeButton : _activeButton;
    }

    private ButtonSceneToolType DrawButton(ButtonSceneToolType activeButton, Texture2D normal, Texture2D active,
        GUILayoutOption[] options)
    {
        var texture = IsButtonActive(activeButton) ? active : normal;
        var isActiveButton = GUILayout.Button(texture, style, options) || _activeButton == activeButton;
        return isActiveButton ? activeButton : _activeButton;
    }

    private bool IsButtonActive(ButtonSceneToolType button)
    {
        return _activeButton == button;
    }

    private void DrawOptionsActiveButton()
    {
        EditorGUILayout.Space();
        switch (_activeButton)
        {
            case ButtonSceneToolType.Camera:
                OptionButton(_buttonCamera, true);
                DrawOptionsObstacles(_dragUI);
                break;
            case ButtonSceneToolType.Up:
                OptionButton(_buttonUp, false);
                break;
            case ButtonSceneToolType.Down:
                OptionButton(_buttonDown, false);
                break;
            case ButtonSceneToolType.Left:
                OptionButton(_buttonLeft, false);
                break;
            case ButtonSceneToolType.Right:
                OptionButton(_buttonRight, false);
                break;
            case ButtonSceneToolType.StartReturn:
                OptionButton(_buttonStartReturn, true);
                break;
            case ButtonSceneToolType.Minus:
                OptionButton(_buttonMinus, false);
                break;
            case ButtonSceneToolType.Plus:
                OptionButton(_buttonPlus, false);
                break;
            case ButtonSceneToolType.Sound:
                OptionButton(_buttonSound, true);
                DrawOptionAudioMixer(_changeVolumeSound);
                break;
            case ButtonSceneToolType.Scenes:
                OptionsScenes();
                break;
        }
    }

    private void OptionButton(SerializedProperty button, bool isClickButton)
    {
        EditorGUILayout.PropertyField(button);
        EditorGUILayout.Space();
        if (!button.objectReferenceValue)
        {
            EditorGUILayout.HelpBox(
                button.name + " no assigned. Please, move object " + button.name.Split("button")[1] +
                " to field above.",
                MessageType.Warning);
            return;
        }

        var objectRef = new SerializedObject(button.objectReferenceValue);
        EditorGUILayout.PropertyField(objectRef.FindProperty(isClickButton ? "onClick" : "onHold"));
        EditorGUILayout.Space();
        ShowConditionDisabled(button);
        objectRef.ApplyModifiedProperties();
    }

    private void OptionsScenes()
    {
        EditorGUILayout.PropertyField(_scenesParent);
        EditorGUILayout.Space();
        var scenes = (GameObject)_scenesParent.objectReferenceValue;
        
        if(!scenes) return;

        //Show Events
        if (_scenes.Count > 1)
            for (var i = 0; i < scenes.transform.childCount; i++)
            {
                EditorGUILayout.PropertyField(_scenes[i].FindProperty("onClick"));
                _scenes[i].ApplyModifiedProperties();
            }

        EditorGUILayout.BeginHorizontal();
        GUI.enabled = _scenes.Count > 1;
        if (GUILayout.Button("Remove Scene"))
        {
            var indexLastChild = scenes.transform.childCount - 1;
            DestroyImmediate(scenes.transform.GetChild(indexLastChild).gameObject);
            _scenes.RemoveAt(indexLastChild);
            ChangeSizeWindow(_scenes.Count);
            scenes.SetActive(_scenes.Count > 1);
        }

        GUI.enabled = true;

        if (GUILayout.Button("Add Scene"))
        {
            var child = Instantiate(scenes.transform.GetChild(0).gameObject, scenes.transform.GetChild(0).position,
                scenes.transform.GetChild(0).rotation, scenes.transform);
            var numberScene = scenes.transform.childCount;
            child.name = "Scene " + numberScene;
            child.GetComponentInChildren<TextMeshProUGUI>().text = numberScene.ToString();
            var buttonScene = child.GetComponent<ButtonClickSceneTool>();
            //buttonScene.keyboardButton = KeyCode.Alpha0 + numberScene;
            _scenes.Add(new SerializedObject(buttonScene));
            ChangeSizeWindow(_scenes.Count);
            scenes.SetActive(_scenes.Count > 1);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ChangeSizeWindow(int countScenes)
    {
        var multiply = countScenes != 1 ? Mathf.FloorToInt((countScenes - 1) / 3f) : -1;
        var rectTransform = (RectTransform)_window.objectReferenceValue;
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, -32f * multiply);
    }

    private void DrawOptionsObstacles(SerializedProperty buttonMove)
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(buttonMove);
        EditorGUILayout.Space();
        if (!buttonMove.objectReferenceValue)
        {
            EditorGUILayout.HelpBox(
                buttonMove.name + " no assigned. Please, move object with component DragUI to field above.",
                MessageType.Warning);
            return;
        }

        var objectRef = new SerializedObject(buttonMove.objectReferenceValue);
        var listProperty = objectRef.FindProperty("obstacles");
        var lenghtList = listProperty.arraySize;
        EditorGUILayout.LabelField("List Obstacles: ");
        for (var i = 0; i < lenghtList; i++)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(listProperty.GetArrayElementAtIndex(i), GUIContent.none);
            if (GUILayout.Button("Remove", GUILayout.Width(80f)))
            {
                listProperty.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(15);
        if (GUILayout.Button("Add Obstacle"))
        {
            listProperty.InsertArrayElementAtIndex(lenghtList);
        }

        EditorGUILayout.EndHorizontal();
        objectRef.ApplyModifiedProperties();
    }
    
    private void DrawOptionAudioMixer(SerializedProperty changeVolumeSound)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(changeVolumeSound);
    
                if (!changeVolumeSound.objectReferenceValue)
                {
                    EditorGUILayout.HelpBox(
                        changeVolumeSound.name +
                        " no assigned. Please, move object with component ChangeVolumeSound to field above.",
                        MessageType.Warning);
                    return;
                }
    
                var objectRef = new SerializedObject(changeVolumeSound.objectReferenceValue);
                var propertyAudioMixerMaster = objectRef.FindProperty("audioMixerMaster");
                EditorGUILayout.PropertyField(propertyAudioMixerMaster);
                var propertyMaskMixer = objectRef.FindProperty("maskMixer");
    
                if (!propertyAudioMixerMaster.objectReferenceValue)
                {
                    propertyMaskMixer.intValue = 1;
                    EditorGUILayout.HelpBox(
                        propertyAudioMixerMaster.name + " no assigned. Please, move audio mixer master to field above.",
                        MessageType.Warning);
                    objectRef.ApplyModifiedProperties();
                    return;
                }
    
                var propertyNameMixers = objectRef.FindProperty("nameMixers");
                propertyNameMixers.arraySize = 0;
    
                objectRef.FindProperty("audioMixerMaster").FindPropertyRelative("audioMixer");
                var audioMixerObj = objectRef.FindProperty("audioMixerMaster").objectReferenceValue as AudioMixerGroup;
                foreach (var mixer in audioMixerObj.audioMixer.FindMatchingGroups(""))
                {
                    propertyNameMixers.arraySize++;
                    propertyNameMixers.GetArrayElementAtIndex(propertyNameMixers.arraySize - 1).stringValue = mixer.name;
                }
    
                var namesMixers = new string[propertyNameMixers.arraySize];
                for (var i = 0; i < namesMixers.Length; i++)
                {
                    namesMixers[i] = propertyNameMixers.GetArrayElementAtIndex(i).stringValue;
                }
    
                propertyMaskMixer.intValue =
                    EditorGUILayout.MaskField("Audio Mixer", propertyMaskMixer.intValue, namesMixers);
                var indexes = new List<int>();
                for (var i = 0; i < propertyNameMixers.arraySize; i++)
                {
                    if (((1 << i) & propertyMaskMixer.intValue) != 0) indexes.Add(i);
                }
    
                var propertyIndexesMixer = objectRef.FindProperty("indexesMixer");
                propertyIndexesMixer.arraySize = indexes.Count;
    
                for (var i = 0; i < indexes.Count; i++)
                {
                    propertyIndexesMixer.GetArrayElementAtIndex(i).intValue = indexes[i];
                }
    
                objectRef.ApplyModifiedProperties();
            }

    private void ShowConditionDisabled(SerializedProperty button)
    {
        var serializedObjectButton = new SerializedObject(button.objectReferenceValue);
        var conditions = serializedObjectButton.FindProperty("disabledConditions");

        for (var j = 0; j < conditions.arraySize; j++)
        {
            var condition = conditions.GetArrayElementAtIndex(j);
            var target = condition.FindPropertyRelative("target");
            var component = condition.FindPropertyRelative("component");
            var nameProperty = condition.FindPropertyRelative("nameProperty");
            var indexComponent = condition.FindPropertyRelative("indexComponent");
            var indexProperty = condition.FindPropertyRelative("indexProperty");
            var sign = condition.FindPropertyRelative("sign");
            var requiredValue = condition.FindPropertyRelative("requiredValue");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(18f);
            EditorGUILayout.PropertyField(target, GUIContent.none);

            if (target.objectReferenceValue != null)
            {
                var GO = (GameObject)target.objectReferenceValue;

                var namesComponents = GetNamesFromComponents(GO.GetComponents<Component>());
                indexComponent.intValue = EditorGUILayout.Popup(indexComponent.intValue, namesComponents);
                indexComponent.intValue = Math.Clamp(indexComponent.intValue, 0, namesComponents.Length - 1);
                component.objectReferenceValue = GO.GetComponents<Component>()[indexComponent.intValue];

                if (component.objectReferenceValue != null)
                {
                    var properties = component.objectReferenceValue.GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    var namesProperty = GetNamesFromProperty(properties);
                    indexProperty.intValue = EditorGUILayout.Popup(indexProperty.intValue, namesProperty);
                    indexProperty.intValue = Math.Clamp(indexProperty.intValue, 0, namesProperty.Length - 1);
                    nameProperty.stringValue = namesProperty[indexProperty.intValue];

                    EditorGUILayout.PropertyField(sign, GUIContent.none, GUILayout.Width(40f));
                    EditorGUILayout.PropertyField(requiredValue, GUIContent.none);
                }
            }

            if (GUILayout.Button("X", GUILayout.Width(20f)))
            {
                conditions.DeleteArrayElementAtIndex(j);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Disabled Condition"))
        {
            conditions.arraySize++;
        }

        serializedObjectButton.ApplyModifiedProperties();
    }

    private string[] GetNamesFromComponents(Component[] array)
    {
        var names = new List<string>();
        for (int i = 0; i < array.Length; i++)
        {
            names.Add(i + " " + array[i].GetType().Name);
        }

        return names.ToArray();
    }

    private string[] GetNamesFromProperty(PropertyInfo[] array)
    {
        var names = new List<string>();
        foreach (var value in array)
        {
            names.Add(value.Name);
        }

        return names.ToArray();
    }
}
#endif
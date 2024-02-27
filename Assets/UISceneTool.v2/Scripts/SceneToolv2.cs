using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using Zenject;

public class SceneToolv2 : MonoBehaviour
{
    [Serializable]
    public struct SceneToolEvents
    {
        public UnityEvent holdZoomPlus;
        public UnityEvent holdZoomMinus;
        public UnityEvent holdUp;
        public UnityEvent holdDown;
        public UnityEvent holdLeft;
        public UnityEvent holdRight;
        public UnityEvent clickCamera;
        public UnityEvent clickReturn;
        public UnityEvent clickSound;
        public List<UnityEvent> clickScene;
    }
    
    [Serializable]
    public struct ConditionsData
    {
        public List<Condition> up;
        public List<Condition> down;
        public List<Condition> left;
        public List<Condition> right;
        public List<Condition> zoomPlus;
        public List<Condition> zoomMinus;

        public bool IsDisabledConditionUp()
        {
            return IsDisabledCondition(up);
        }
        public bool IsDisabledConditionDown()
        {
            return IsDisabledCondition(down);
        }
        public bool IsDisabledConditionLeft()
        {
            return IsDisabledCondition(left);
        }
        public bool IsDisabledConditionRight()
        {
            return IsDisabledCondition(right);
        }
        public bool IsDisabledConditionZoomPlus()
        {
            return IsDisabledCondition(zoomPlus);
        }
        public bool IsDisabledConditionZoomMinus()
        {
            return IsDisabledCondition(zoomMinus);
        }
        private bool IsDisabledCondition(List<Condition> conditions)
        {
            return conditions.Count != 0 && conditions.All(condition => condition.GetResult());
        }
    }

    [Serializable]
    public struct AudioMixerData
    {
        public AudioMixerGroup audioMixerMaster; 
        public List<string> nameMixers;
        public List<int> indexesMixer;
        public int maskMixer;
    }

    [Inject] private UIRootSceneTool _uiRoot;
    [Inject] private InputSceneTool _inputSceneTool;

    [SerializeField] private SceneToolEvents _sceneToolEvents;
    [SerializeField] private ConditionsData _conditionsData;
    [SerializeField] private AudioMixerData _audioMixerData;

    private byte _indexActiveScene;

    private void Awake()
    {
        _uiRoot.AddScenes(_sceneToolEvents.clickScene);

        _inputSceneTool.EventsHoldUp(
            (ctx) => _uiRoot.upResizeButton.OnPointerDown(null),
            (ctx) => _sceneToolEvents.holdUp?.Invoke(),
            (ctx) => _uiRoot.upResizeButton.OnPointerUp(null));
        _inputSceneTool.EventsHoldDown(
            (ctx) => _uiRoot.downResizeButton.OnPointerDown(null),
            (ctx) => _sceneToolEvents.holdDown?.Invoke(),
            (ctx) => _uiRoot.downResizeButton.OnPointerUp(null));
        _inputSceneTool.EventsHoldLeft(
            (ctx) => _uiRoot.leftResizeButton.OnPointerDown(null),
            (ctx) => _sceneToolEvents.holdLeft?.Invoke(),
            (ctx) => _uiRoot.leftResizeButton.OnPointerUp(null));
        _inputSceneTool.EventsHoldRight(
            (ctx) => _uiRoot.rightResizeButton.OnPointerDown(null),
            (ctx) => _sceneToolEvents.holdRight?.Invoke(),
            (ctx) => _uiRoot.rightResizeButton.OnPointerUp(null));
        _inputSceneTool.EventsHoldZoomIn(
            (ctx) => _uiRoot.zoomPlusResizeButton.OnPointerDown(null),
            (ctx) => _sceneToolEvents.holdZoomPlus?.Invoke(),
            (ctx) => _uiRoot.zoomPlusResizeButton.OnPointerUp(null));
        _inputSceneTool.EventsHoldZoomOut(
            (ctx) => _uiRoot.zoomMinusResizeButton.OnPointerDown(null),
            (ctx) => _sceneToolEvents.holdZoomMinus?.Invoke(),
            (ctx) => _uiRoot.zoomMinusResizeButton.OnPointerUp(null));

        _inputSceneTool.EventsClickCamera(
            (ctx) => _uiRoot.cameraResizeButton.OnPointerDown(null),
            (ctx) =>
            {
                _uiRoot.cameraResizeButton.OnPointerUp(null);
                _sceneToolEvents.clickCamera?.Invoke();
            });
        _inputSceneTool.EventsClickReturn(
            (ctx) => _uiRoot.returnResizeButton.OnPointerDown(null),
            (ctx) =>
            {
                _uiRoot.returnResizeButton.OnPointerUp(null);
                _sceneToolEvents.clickReturn?.Invoke();
            });
        _inputSceneTool.EventsClickSound(
            (ctx) => _uiRoot.soundResizeButton.OnPointerDown(null),
            (ctx) =>
            {
                _uiRoot.soundResizeButton.OnPointerUp(null);
                _sceneToolEvents.clickSound?.Invoke();
            });
        _inputSceneTool.EventsClickScene(
            (ctx) =>
            {
                _indexActiveScene = (byte)(ctx.ReadValue<float>() - 1);
                if (_indexActiveScene >= _sceneToolEvents.clickScene.Count) return;

                _uiRoot.scenesResizeButton[_indexActiveScene].OnPointerDown(null);
                _uiRoot.scenesClickButton[_indexActiveScene].OnPointerDown(null);
                _sceneToolEvents.clickScene[_indexActiveScene]?.Invoke();
            },
            (ctx) =>
            {
                if (_indexActiveScene >= _sceneToolEvents.clickScene.Count) return;

                _uiRoot.scenesResizeButton[_indexActiveScene].OnPointerUp(null);
            });
        _inputSceneTool.EventsClickNextScene(
            (ctx) =>
            {
                if (++_indexActiveScene >= _sceneToolEvents.clickScene.Count)
                    _indexActiveScene = 0;

                _uiRoot.scenesResizeButton[_indexActiveScene].OnPointerDown(null);
                _uiRoot.scenesClickButton[_indexActiveScene].OnPointerDown(null);
                _sceneToolEvents.clickScene[_indexActiveScene]?.Invoke();
            },
            (ctx) => { _uiRoot.scenesResizeButton[_indexActiveScene].OnPointerUp(null); });
        _inputSceneTool.EventsClickPrevScene(
            (ctx) =>
            {
                if (--_indexActiveScene == 255)
                    _indexActiveScene = (byte)(_sceneToolEvents.clickScene.Count - 1);

                _uiRoot.scenesResizeButton[_indexActiveScene].OnPointerDown(null);
                _uiRoot.scenesClickButton[_indexActiveScene].OnPointerDown(null);
                _sceneToolEvents.clickScene[_indexActiveScene]?.Invoke();
            },
            (ctx) => { _uiRoot.scenesResizeButton[_indexActiveScene].OnPointerUp(null); });

        if (!_audioMixerData.audioMixerMaster)
            _uiRoot.SoundDisabled();
    }

    private void OnEnable()
    {
        _inputSceneTool.Enable();
    }

    private void FixedUpdate()
    {
        _uiRoot.IsDisabledUp = _conditionsData.IsDisabledConditionUp();
        _uiRoot.IsDisabledDown = _conditionsData.IsDisabledConditionDown();
        _uiRoot.IsDisabledLeft = _conditionsData.IsDisabledConditionLeft();
        _uiRoot.IsDisabledRight = _conditionsData.IsDisabledConditionRight();
        _uiRoot.IsDisabledZoomPlus = _conditionsData.IsDisabledConditionZoomPlus();
        _uiRoot.IsDisabledZoomMinus = _conditionsData.IsDisabledConditionZoomMinus();
    }

    private void OnDisable()
    {
        _inputSceneTool.Disable();
    }

    public void Test(string text)
    {
        Debug.Log(text);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SceneToolv2))]
public class SceneToolv2Editor : Editor
{
    private enum ButtonSceneToolType
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Plus,
        Minus,
        Return,
        Camera,
        Sound,
        Scenes
    }

    private SerializedProperty _sceneToolEvents;
    private SerializedProperty _conditionsData;
    private SerializedProperty _audioMixerData;

    private ButtonSceneToolType _activeButton;

    #region TextureForEditor

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

    #endregion

    private GUIStyle _style;
    private GUILayoutOption[] _sizeButton;

    private int _selectedTab = 0;
    private string[] _tabNames = { "Init", "Main", "Input" };
    private string[] _actionNames;

    private SerializedProperty _propertyActionNames;

    private void OnEnable()
    {
        var inputSceneTool = FindObjectOfType<InputSceneToolInstaller>();
        _propertyActionNames = new SerializedObject(inputSceneTool).FindProperty("actionNames");
        if (_propertyActionNames.arraySize == 0)
        {
            _propertyActionNames.arraySize = 12;
            for (var i = 0; i < _propertyActionNames.arraySize; i++)
            {
                _propertyActionNames.GetArrayElementAtIndex(i).stringValue = "None";
            }

            _propertyActionNames.serializedObject.ApplyModifiedProperties();
        }

        var actionNames = inputSceneTool.inputActionAsset.FindActionMap("InputControlPanel")
            .Select(action => action.name).ToList();
        actionNames.Insert(0, "None");
        _actionNames = actionNames.ToArray();

        _sceneToolEvents = serializedObject.FindProperty("_sceneToolEvents");
        _conditionsData = serializedObject.FindProperty("_conditionsData");
        _audioMixerData = serializedObject.FindProperty("_audioMixerData");

        #region InitTextureForEditor

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

        #endregion

        _style = new GUIStyle();
        _style.padding = new RectOffset(3, 3, 3, 3);
        _style.hover.background = _textureHover;

        _sizeButton = new[] { GUILayout.Width(50f), GUILayout.Height(50f) };

        _selectedTab = 1;
        _activeButton = ButtonSceneToolType.None;
    }

    public override void OnInspectorGUI()
    {
        _selectedTab = GUILayout.Toolbar(_selectedTab, _tabNames);
        EditorGUILayout.Space();

        switch (_selectedTab)
        {
            case 0:
                break;
            case 1:
                DrawInterfaceButtons();
                DrawOptionsActiveButton();
                break;
            case 2:
                DrawOptionInputActions();
                break;
        }

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
        _activeButton = DrawButton(ButtonSceneToolType.Return, _textureStartReturn, _textureStartReturnActive);
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
        var isActiveButton = GUILayout.Button(texture, _style, _sizeButton) || _activeButton == activeButton;
        return isActiveButton ? activeButton : _activeButton;
    }

    private ButtonSceneToolType DrawButton(ButtonSceneToolType activeButton, Texture2D normal, Texture2D active,
        GUILayoutOption[] options)
    {
        var texture = IsButtonActive(activeButton) ? active : normal;
        var isActiveButton = GUILayout.Button(texture, _style, options) || _activeButton == activeButton;
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
                OptionButton(_sceneToolEvents.FindPropertyRelative("clickCamera"));
                //DrawOptionsObstacles(_dragUI);
                break;
            case ButtonSceneToolType.Up:
                OptionButton(_sceneToolEvents.FindPropertyRelative("holdUp"));
                OptionCondition(_conditionsData.FindPropertyRelative("up"));
                break;
            case ButtonSceneToolType.Down:
                OptionButton(_sceneToolEvents.FindPropertyRelative("holdDown"));
                OptionCondition(_conditionsData.FindPropertyRelative("down"));
                break;
            case ButtonSceneToolType.Left:
                OptionButton(_sceneToolEvents.FindPropertyRelative("holdLeft"));
                OptionCondition(_conditionsData.FindPropertyRelative("left"));
                break;
            case ButtonSceneToolType.Right:
                OptionButton(_sceneToolEvents.FindPropertyRelative("holdRight"));
                OptionCondition(_conditionsData.FindPropertyRelative("right"));
                break;
            case ButtonSceneToolType.Return:
                OptionButton(_sceneToolEvents.FindPropertyRelative("clickReturn"));
                break;
            case ButtonSceneToolType.Minus:
                OptionButton(_sceneToolEvents.FindPropertyRelative("holdZoomMinus"));
                OptionCondition(_conditionsData.FindPropertyRelative("zoomMinus"));
                break;
            case ButtonSceneToolType.Plus:
                OptionButton(_sceneToolEvents.FindPropertyRelative("holdZoomPlus"));
                OptionCondition(_conditionsData.FindPropertyRelative("zoomPlus"));
                break;
            case ButtonSceneToolType.Sound:
                OptionButton(_sceneToolEvents.FindPropertyRelative("clickSound"));
                DrawOptionAudioMixer(_audioMixerData);
                break;
            case ButtonSceneToolType.Scenes:
                OptionsScenes(_sceneToolEvents.FindPropertyRelative("clickScene"));
                break;
        }
    }

    private void OptionButton(SerializedProperty btnEvent)
    {
        EditorGUILayout.PropertyField(btnEvent);
        EditorGUILayout.Space();
    }

    private void OptionCondition(SerializedProperty btnConditions)
    {
        EditorGUILayout.Space();
        ShowConditionDisabled(btnConditions);
    }

    private void OptionsScenes(SerializedProperty listScene)
    {
        for (var i = 0; i < listScene.arraySize; i++)
        {
            EditorGUILayout.PropertyField(listScene.GetArrayElementAtIndex(i));
        }

        EditorGUILayout.BeginHorizontal();
        GUI.enabled = listScene.arraySize > 0;
        if (GUILayout.Button("Remove Scene"))
        {
            listScene.arraySize--;
        }

        GUI.enabled = true;

        if (GUILayout.Button("Add Scene"))
        {
            listScene.arraySize++;
        }

        EditorGUILayout.EndHorizontal();
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

    private void DrawOptionAudioMixer(SerializedProperty audioMixerData)
    {
        var audioMixerMaster = audioMixerData.FindPropertyRelative("audioMixerMaster");

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(audioMixerMaster);

        if (!audioMixerMaster.objectReferenceValue)
        {
            EditorGUILayout.HelpBox(
                audioMixerMaster.name + " no assigned. Please, move audio mixer master to field above.",
                MessageType.Warning);
            return;
        }

        var maskMixer = audioMixerData.FindPropertyRelative("maskMixer");
        var nameMixers = audioMixerData.FindPropertyRelative("nameMixers");
        nameMixers.arraySize = 0;

        var audioMixerObj = audioMixerMaster.objectReferenceValue as AudioMixerGroup;
        foreach (var mixer in audioMixerObj.audioMixer.FindMatchingGroups(""))
        {
            nameMixers.arraySize++;
            nameMixers.GetArrayElementAtIndex(nameMixers.arraySize - 1).stringValue = mixer.name;
        }

        var namesMixers = new string[nameMixers.arraySize];
        for (var i = 0; i < namesMixers.Length; i++)
        {
            namesMixers[i] = nameMixers.GetArrayElementAtIndex(i).stringValue;
        }

        maskMixer.intValue =
            EditorGUILayout.MaskField("Audio Mixer", maskMixer.intValue, namesMixers);
        var indexes = new List<int>();
        for (var i = 0; i < nameMixers.arraySize; i++)
        {
            if (((1 << i) & maskMixer.intValue) != 0) indexes.Add(i);
        }

        var propertyIndexesMixer = audioMixerData.FindPropertyRelative("indexesMixer");
        propertyIndexesMixer.arraySize = indexes.Count;

        for (var i = 0; i < indexes.Count; i++)
        {
            propertyIndexesMixer.GetArrayElementAtIndex(i).intValue = indexes[i];
        }
    }

    private void ShowConditionDisabled(SerializedProperty conditions)
    {
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

    private void DrawOptionInputActions()
    {
        EditorGUILayout.Space();

        _propertyActionNames.GetArrayElementAtIndex(0).stringValue =
            _actionNames[EditorGUILayout.Popup(new GUIContent("Button Up"),
                Array.IndexOf(_actionNames, _propertyActionNames.GetArrayElementAtIndex(0).stringValue), _actionNames)];
        _propertyActionNames.GetArrayElementAtIndex(1).stringValue =
            _actionNames[EditorGUILayout.Popup(new GUIContent("Button Down"),
                Array.IndexOf(_actionNames, _propertyActionNames.GetArrayElementAtIndex(1).stringValue), _actionNames)];
        _propertyActionNames.GetArrayElementAtIndex(2).stringValue =
            _actionNames[EditorGUILayout.Popup(new GUIContent("Button Left"),
                Array.IndexOf(_actionNames, _propertyActionNames.GetArrayElementAtIndex(2).stringValue), _actionNames)];
        _propertyActionNames.GetArrayElementAtIndex(3).stringValue =
            _actionNames[EditorGUILayout.Popup(new GUIContent("Button Right"),
                Array.IndexOf(_actionNames, _propertyActionNames.GetArrayElementAtIndex(3).stringValue), _actionNames)];
        _propertyActionNames.GetArrayElementAtIndex(4).stringValue =
            _actionNames[EditorGUILayout.Popup(new GUIContent("Button Zoom Plus"),
                Array.IndexOf(_actionNames, _propertyActionNames.GetArrayElementAtIndex(4).stringValue), _actionNames)];
        _propertyActionNames.GetArrayElementAtIndex(5).stringValue =
            _actionNames[EditorGUILayout.Popup(new GUIContent("Button Zoom Minus"),
                Array.IndexOf(_actionNames, _propertyActionNames.GetArrayElementAtIndex(5).stringValue), _actionNames)];
        _propertyActionNames.GetArrayElementAtIndex(6).stringValue =
            _actionNames[EditorGUILayout.Popup(new GUIContent("Button Switch Panel"),
                Array.IndexOf(_actionNames, _propertyActionNames.GetArrayElementAtIndex(6).stringValue), _actionNames)];
        _propertyActionNames.GetArrayElementAtIndex(7).stringValue =
            _actionNames[EditorGUILayout.Popup(new GUIContent("Button Switch Sound"),
                Array.IndexOf(_actionNames, _propertyActionNames.GetArrayElementAtIndex(7).stringValue), _actionNames)];
        _propertyActionNames.GetArrayElementAtIndex(8).stringValue =
            _actionNames[EditorGUILayout.Popup(new GUIContent("Button Return Back"),
                Array.IndexOf(_actionNames, _propertyActionNames.GetArrayElementAtIndex(8).stringValue), _actionNames)];
        _propertyActionNames.GetArrayElementAtIndex(9).stringValue =
            _actionNames[EditorGUILayout.Popup(new GUIContent("Button Next Location"),
                Array.IndexOf(_actionNames, _propertyActionNames.GetArrayElementAtIndex(9).stringValue), _actionNames)];
        _propertyActionNames.GetArrayElementAtIndex(10).stringValue =
            _actionNames[EditorGUILayout.Popup(new GUIContent("Button Prev Location"),
                Array.IndexOf(_actionNames, _propertyActionNames.GetArrayElementAtIndex(10).stringValue),
                _actionNames)];
        _propertyActionNames.GetArrayElementAtIndex(11).stringValue =
            _actionNames[EditorGUILayout.Popup(new GUIContent("Button Number Location"),
                Array.IndexOf(_actionNames, _propertyActionNames.GetArrayElementAtIndex(11).stringValue),
                _actionNames)];

        EditorGUILayout.Space();
        
        if (GUILayout.Button("Open Input Actions"))
        {
            AssetDatabase.OpenAsset(FindObjectOfType<InputSceneToolInstaller>().inputActionAsset);
        }

        _propertyActionNames.serializedObject.ApplyModifiedProperties();
    }
}
#endif
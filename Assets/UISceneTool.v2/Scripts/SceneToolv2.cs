using System;
using System.Collections.Generic;
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

    [Inject] private UIRootSceneTool _uiRoot;

    [SerializeField] private SceneToolEvents _sceneToolEvents;

    private void Awake()
    {
        _uiRoot.AddScenes(_sceneToolEvents.clickScene);
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
        _sceneToolEvents = serializedObject.FindProperty("_sceneToolEvents");
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

        _scenes ??= new List<SerializedObject>();
        _scenes.Clear();
        // for (var i = 0; i < scenes.transform.childCount; i++)
        // {
        //     _scenes.Add(new SerializedObject(scenes.transform.GetChild(i).GetComponent<ButtonClickSceneTool>()));
        // }
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
                OptionButton(_sceneToolEvents.FindPropertyRelative("clickCamera"));
                DrawOptionsObstacles(_dragUI);
                break;
            case ButtonSceneToolType.Up:
                OptionButton(_sceneToolEvents.FindPropertyRelative("holdUp"));
                break;
            case ButtonSceneToolType.Down:
                OptionButton(_sceneToolEvents.FindPropertyRelative("holdDown"));
                break;
            case ButtonSceneToolType.Left:
                OptionButton(_sceneToolEvents.FindPropertyRelative("holdLeft"));
                break;
            case ButtonSceneToolType.Right:
                OptionButton(_sceneToolEvents.FindPropertyRelative("holdRight"));
                break;
            case ButtonSceneToolType.Return:
                OptionButton(_sceneToolEvents.FindPropertyRelative("clickReturn"));
                break;
            case ButtonSceneToolType.Minus:
                OptionButton(_sceneToolEvents.FindPropertyRelative("holdZoomMinus"));
                break;
            case ButtonSceneToolType.Plus:
                OptionButton(_sceneToolEvents.FindPropertyRelative("holdZoomPlus"));
                break;
            case ButtonSceneToolType.Sound:
                OptionButton(_sceneToolEvents.FindPropertyRelative("clickSound"));
                DrawOptionAudioMixer(_changeVolumeSound);
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

        EditorGUILayout.Space();
        //ShowConditionDisabled(button);
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
using System;
using System.Collections.Generic;
using System.Linq;
using UISceneTool.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIRootSceneTool
{
    public struct UIButtonSceneTool
    {
        public Button Background;
        public VisualElement Frontground;
        public VisualElement Icon;
        public ResizeButtonManipulator ResizeButtonManipulator;
        public MouseHoverManipulator MouseHoverManipulator;

        public bool IsDisabled
        {
            set
            {
                if (value && !Background.ClassListContains("disabled"))
                {
                    Background.AddToClassList("disabled");
                    SwitchManipulators(false);
                }
                else if (!value && Background.ClassListContains("disabled"))
                {
                    Background.RemoveFromClassList("disabled");
                    SwitchManipulators(true);
                }
            }
        }

        public UIButtonSceneTool(Button background, ResizeButtonManipulator resizeButtonManipulator,
            MouseHoverManipulator mouseHoverManipulator)
        {
            Background = background;
            Frontground = Background.Children().First();
            Icon = Frontground.Children().First();
            ResizeButtonManipulator = resizeButtonManipulator;
            MouseHoverManipulator = mouseHoverManipulator;
        }

        public void SwitchManipulators(bool value)
        {
            if (value)
            {
                Background.AddManipulator(ResizeButtonManipulator);
                Frontground.AddManipulator(MouseHoverManipulator);
            }
            else
            {
                Background.RemoveManipulator(ResizeButtonManipulator);
                Frontground.RemoveManipulator(MouseHoverManipulator);
            }
        }
    }

    private readonly VisualElement _root;

    private VisualElement _window;
    private UIButtonSceneTool _camera;
    private UIButtonSceneTool _zoomPlus;
    private UIButtonSceneTool _zoomMinus;
    private UIButtonSceneTool _up;
    private UIButtonSceneTool _left;
    private UIButtonSceneTool _down;
    private UIButtonSceneTool _right;
    private UIButtonSceneTool _return;
    private UIButtonSceneTool _sound;
    private VisualElement _scenes;

    #region InitManipulators

    public ResizeButtonManipulator upResizeButton = new ResizeButtonManipulator();
    public ResizeButtonManipulator downResizeButton = new ResizeButtonManipulator();
    public ResizeButtonManipulator leftResizeButton = new ResizeButtonManipulator();
    public ResizeButtonManipulator rightResizeButton = new ResizeButtonManipulator();
    public ResizeButtonManipulator zoomPlusResizeButton = new ResizeButtonManipulator();
    public ResizeButtonManipulator zoomMinusResizeButton = new ResizeButtonManipulator();
    public ResizeButtonManipulator returnResizeButton = new ResizeButtonManipulator();
    public ResizeButtonManipulator soundResizeButton = new ResizeButtonManipulator();
    public ResizeButtonManipulator cameraResizeButton = new ResizeButtonManipulator();
    public List<ResizeButtonManipulator> scenesResizeButton = new List<ResizeButtonManipulator>();

    public MouseHoverManipulator upMouseHover = new MouseHoverManipulator();
    public MouseHoverManipulator downMouseHover = new MouseHoverManipulator();
    public MouseHoverManipulator leftMouseHover = new MouseHoverManipulator();
    public MouseHoverManipulator rightMouseHover = new MouseHoverManipulator();
    public MouseHoverManipulator zoomPlusMouseHover = new MouseHoverManipulator();
    public MouseHoverManipulator zoomMinusMouseHover = new MouseHoverManipulator();
    public MouseHoverManipulator returnMouseHover = new MouseHoverManipulator();
    public MouseHoverManipulator soundMouseHover = new MouseHoverManipulator();
    public MouseHoverManipulator cameraMouseHover = new MouseHoverManipulator();

    public List<ClickSceneManipulator> scenesClickButton = new List<ClickSceneManipulator>();

    private MouseScrollManipulator _mouseScrollManipulator;
    public ClickSoundManipulator clickMouseManipulator;
    private DragCameraManipulator _dragCameraManipulator;

    #endregion

    public bool IsDisabledUp { set => _up.IsDisabled = value; }

    public bool IsDisabledDown { set => _down.IsDisabled = value; }

    public bool IsDisabledLeft{ set => _left.IsDisabled = value; }

    public bool IsDisabledRight{ set => _right.IsDisabled = value; }

    public bool IsDisabledZoomPlus{ set => _zoomPlus.IsDisabled = value; }

    public bool IsDisabledZoomMinus{ set => _zoomMinus.IsDisabled = value; }

    public List<RectTransform> SetObstacles
    {
        set => _dragCameraManipulator.SetObstacle = value;
    }

    public UIRootSceneTool(UIDocument uiDocument)
    {
        _root = uiDocument.rootVisualElement;

        _window = _root.Q<VisualElement>("Window");
        _camera = new UIButtonSceneTool(_root.Q<Button>("Camera"), cameraResizeButton, cameraMouseHover);
        _zoomPlus = new UIButtonSceneTool(_root.Q<Button>("ZoomPlus"), zoomPlusResizeButton, zoomPlusMouseHover);
        _zoomMinus = new UIButtonSceneTool(_root.Q<Button>("ZoomMinus"), zoomMinusResizeButton, zoomMinusMouseHover);
        _up = new UIButtonSceneTool(_root.Q<Button>("Up"), upResizeButton, upMouseHover);
        _left = new UIButtonSceneTool(_root.Q<Button>("Left"), leftResizeButton, leftMouseHover);
        _down = new UIButtonSceneTool(_root.Q<Button>("Down"), downResizeButton, downMouseHover);
        _right = new UIButtonSceneTool(_root.Q<Button>("Right"), rightResizeButton, rightMouseHover);
        _return = new UIButtonSceneTool(_root.Q<Button>("Return"), returnResizeButton, returnMouseHover);
        _sound = new UIButtonSceneTool(_root.Q<Button>("Sound"), soundResizeButton, soundMouseHover);
        _scenes = _root.Q<VisualElement>("Scenes");

        _mouseScrollManipulator = new MouseScrollManipulator(null);
        clickMouseManipulator = new ClickSoundManipulator(SwitchActivatedOrDeactivated);
        _dragCameraManipulator = new DragCameraManipulator(null, null, null, SwitchCameraElements);

        _zoomPlus.SwitchManipulators(true);
        _zoomMinus.SwitchManipulators(true);
        _up.SwitchManipulators(true);
        _left.SwitchManipulators(true);
        _down.SwitchManipulators(true);
        _right.SwitchManipulators(true);
        _return.SwitchManipulators(true);
        _sound.SwitchManipulators(true);
        _camera.SwitchManipulators(true);

        _sound.Background.AddManipulator(_mouseScrollManipulator);
        _sound.Background.AddManipulator(clickMouseManipulator);
        _camera.Background.AddManipulator(_dragCameraManipulator);
    }
    
    #region Camera

    private bool _isCameraDisable => _camera.Background.ClassListContains("ColorWhite");
    private void CameraBtnSwitch()
    {
        _camera.Background.ToggleInClassList("ColorWhite");
        _camera.Background.ToggleInClassList("ColorGreen");
        _camera.Frontground.ToggleInClassList("ColorWhite");
        _camera.Frontground.ToggleInClassList("ColorGreen");
        _camera.Icon.ToggleInClassList("ColorWhite");
        _camera.Icon.ToggleInClassList("ColorGreen");
    }

    public void SwitchCameraElements()
    {
        CameraBtnSwitch();
        var value = !_isCameraDisable;
        _zoomPlus.Background.visible = value;
        _zoomMinus.Background.visible = value;
        _up.Background.visible = value;
        _left.Background.visible = value;
        _down.Background.visible = value;
        _right.Background.visible = value;
        _return.Background.visible = value;
        _sound.Background.visible = value;
        _scenes.visible = value;
    }

    #endregion

    #region Scenes

    public void AddScenes(List<UnityEvent> scenesAction)
    {
        var scenes = _root.Q<VisualElement>("Scenes");
        var sceneRow = new VisualElement();
        for (var i = 0; i < scenesAction.Count; i++)
        {
            if (i % 3 == 0) sceneRow = AddRowScene(scenes, (byte)(i / 3));
            AddScene(sceneRow, (byte)(i + 1), scenesAction[i]);
        }

        ChangeLeftPaddingFirstRow();

        if (scenesAction.Count > 0)
            SelectScene(_root.Q<Button>("Scene1"));
    }

    private VisualElement AddRowScene(VisualElement scenes, byte numberRow)
    {
        var scenesRow = new VisualElement();
        scenesRow.AddToClassList("SceneRow");
        scenesRow.name = "SceneRow" + numberRow;
        if (numberRow == 0)
        {
            var o = scenesRow.style.paddingLeft;
            o.value = 100f;
            scenesRow.style.paddingLeft = o;
        }

        scenes.Add(scenesRow);
        return scenesRow;
    }

    private void AddScene(VisualElement scenesRow, byte index, UnityEvent action)
    {
        var scene = new Button();
        scene.AddToClassList("Scene");
        scene.AddToClassList("ColorWhite");
        scene.name = "Scene" + index;
        scenesClickButton.Add(new ClickSceneManipulator(() =>
        {
            UnSelectScenes();
            SelectScene(scene);
        }, action));
        scene.AddManipulator(scenesClickButton.Last());

        var sceneForeground = new VisualElement();
        sceneForeground.AddToClassList("SceneForeground");
        sceneForeground.AddToClassList("ColorGreen");
        sceneForeground.name = "SceneForeground" + index;

        var sceneText = new Label();
        sceneText.AddToClassList("SceneText");
        sceneText.AddToClassList("ColorWhite");
        sceneText.text = index.ToString();
        sceneText.name = "SceneText" + sceneText.text;

        sceneForeground.Add(sceneText);
        scene.Add(sceneForeground);
        scenesRow.Add(scene);

        scenesResizeButton.Add(new ResizeButtonManipulator());
        scene.AddManipulator(scenesResizeButton.Last());
        sceneForeground.AddManipulator(new MouseHoverManipulator());
    }

    private void ChangeLeftPaddingFirstRow()
    {
        var sceneRow = _root.Q<VisualElement>(null, "SceneRow");
        var newPaddingLeft = sceneRow.style.paddingLeft;
        newPaddingLeft.value = (192 - sceneRow.childCount * 52 - (18 * (sceneRow.childCount - 1))) / 2f;
        sceneRow.style.paddingLeft = newPaddingLeft;
    }

    private void UnSelectScenes()
    {
        var scene = _root.Q<Button>(null, "Scene", "ColorGreen");

        if (scene == null) return;

        scene.RemoveFromClassList("ColorGreen");
        scene.Children().First().RemoveFromClassList("ColorWhite");
        scene.Children().First().Children().First().RemoveFromClassList("ColorGreen");
        scene.AddToClassList("ColorWhite");
        scene.Children().First().AddToClassList("ColorGreen");
        scene.Children().First().Children().First().AddToClassList("ColorWhite");
    }

    private void SelectScene(VisualElement scene)
    {
        scene.RemoveFromClassList("ColorWhite");
        scene.Children().First().RemoveFromClassList("ColorGreen");
        scene.Children().First().Children().First().RemoveFromClassList("ColorWhite");
        scene.AddToClassList("ColorGreen");
        scene.Children().First().AddToClassList("ColorWhite");
        scene.Children().First().Children().First().AddToClassList("ColorGreen");
    }

    #endregion

    #region SoundUI

    private float _soundBarValue
    {
        get => Mathf.Clamp(_root.Q<VisualElement>("SoundBar").transform.scale.x,0.05f, 1f);
        set
        {
            var scaleBar = _root.Q<VisualElement>("SoundBar").transform.scale;
            scaleBar.x = Mathf.Clamp(value, 0.05f, 1f);
            _root.Q<VisualElement>("SoundBar").transform.scale = scaleBar;
        }
    }
    public void SoundDisabled()
    {
        _sound.Background.AddToClassList("ColorDisabledWhite");
        _sound.Frontground.AddToClassList("ColorDisabledGrey");
        _sound.Icon.AddToClassList("ColorDisabledWhite");
        _root.Q<VisualElement>("SoundBar").AddToClassList("ColorDisabledWhite");
        _sound.Background.RemoveManipulator(_mouseScrollManipulator);
        _sound.Background.RemoveManipulator(clickMouseManipulator);
        _sound.Background.RemoveManipulator(soundResizeButton);
        _sound.Background.RemoveManipulator(soundMouseHover);
        _soundBarValue = 0f;
    }

    private void SwitchActivatedOrDeactivated()
    {
        if (_sound.Frontground.ClassListContains("ColorWhite"))
        {
            SoundDeactivated();
        }
        else
        {
            SoundActivated();
        }
    }
    private void SoundActivated()
    {
        _sound.Frontground.AddToClassList("ColorWhite");
        _sound.Icon.RemoveFromClassList("ColorWhite");
        _root.Q<VisualElement>("SoundBar").RemoveFromClassList("ColorWhite");
        _sound.Background.AddManipulator(_mouseScrollManipulator);
        _soundBarValue = 1f;
    }
    
    private void SoundDeactivated()
    {
        _sound.Frontground.RemoveFromClassList("ColorWhite");
        _sound.Icon.AddToClassList("ColorWhite");
        _root.Q<VisualElement>("SoundBar").AddToClassList("ColorWhite");
        _sound.Background.RemoveManipulator(_mouseScrollManipulator);
        _soundBarValue = 0f;
    }

    #endregion
}
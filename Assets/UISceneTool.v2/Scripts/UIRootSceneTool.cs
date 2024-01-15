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

        public UIButtonSceneTool(Button background)
        {
            Background = background;
            Frontground = Background.Children().First();
            Icon = Frontground.Children().First();
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

    private Vector3 _offsetOriginDrag;

    public Action<PointerMoveEvent> onCameraBeginDrag;
    public Action<PointerMoveEvent> onCameraDrag;
    public Vector3 GetOffsetOriginDrag => _offsetOriginDrag;

    public Vector3 SetPositionWindow
    {
        set => _window.transform.position = value;
    }

    public Vector2 GetSizeWindow => new Vector2(_window.resolvedStyle.width, _window.resolvedStyle.height);
    public Vector2 GetPositionWindow => new Vector2(_window.transform.position.x, _window.transform.position.y);

    public IManipulator AddManipulatorToCamera
    {
        set => _camera.Background.AddManipulator(value);
    }

    public UIRootSceneTool(UIDocument uiDocument)
    {
        _root = uiDocument.rootVisualElement;

        _window = _root.Q<VisualElement>("Window");
        _camera = new UIButtonSceneTool(_root.Q<Button>("Camera"));
        _zoomPlus = new UIButtonSceneTool(_root.Q<Button>("ZoomPlus"));
        _zoomMinus = new UIButtonSceneTool(_root.Q<Button>("ZoomMinus"));
        _up = new UIButtonSceneTool(_root.Q<Button>("Up"));
        _left = new UIButtonSceneTool(_root.Q<Button>("Left"));
        _down = new UIButtonSceneTool(_root.Q<Button>("Down"));
        _right = new UIButtonSceneTool(_root.Q<Button>("Right"));
        _return = new UIButtonSceneTool(_root.Q<Button>("Return"));
        _sound = new UIButtonSceneTool(_root.Q<Button>("Sound"));
        _scenes = _root.Q<VisualElement>("Scenes");

        _zoomPlus.Background.AddManipulator(new ResizeButtonManipulator());
        _zoomMinus.Background.AddManipulator(new ResizeButtonManipulator());
        _up.Background.AddManipulator(new ResizeButtonManipulator());
        _left.Background.AddManipulator(new ResizeButtonManipulator());
        _down.Background.AddManipulator(new ResizeButtonManipulator());
        _right.Background.AddManipulator(new ResizeButtonManipulator());
        _return.Background.AddManipulator(new ResizeButtonManipulator());
        _sound.Background.AddManipulator(new ResizeButtonManipulator());
        _camera.Background.AddManipulator(new ResizeButtonManipulator());
        _camera.Background.AddManipulator(new DragCameraManipulator(null, null, null, SwitchCameraElements));
    }

    private bool _isCameraDisable => _camera.Background.ClassListContains("CameraBackgroundClicked");

    private void CameraBtnSwitch()
    {
        _camera.Background.ToggleInClassList("CameraBackgroundClicked");
        _camera.Frontground.ToggleInClassList("CameraFrontgroundClicked");
        _camera.Icon.ToggleInClassList("CameraIconClicked");
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

    public void OnClickCamera(Action action)
    {
        _root.Q<Button>("Camera").clicked += () => { action?.Invoke(); };
    }

    public void OnCameraDrag(Action<PointerMoveEvent> action)
    {
        _camera.Background.RegisterCallback<PointerMoveEvent>(evt => { action?.Invoke(evt); });
    }

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
        scene.name = "Scene" + index;
        scene.AddManipulator(new EventClickManipulation(() =>
        {
            UnSelectScenes();
            SelectScene(scene);
        }, action));

        var sceneFrontground = new VisualElement();
        sceneFrontground.AddToClassList("SceneFrontground");
        sceneFrontground.name = "SceneFrontground" + index;

        var sceneText = new Label();
        sceneText.AddToClassList("SceneText");
        sceneText.text = index.ToString();
        sceneText.name = "SceneText" + sceneText.text;

        sceneFrontground.Add(sceneText);
        scene.Add(sceneFrontground);
        scenesRow.Add(scene);
    }

    private void ChangeLeftPaddingFirstRow()
    {
        var sceneRow = _root.Q<VisualElement>(null,"SceneRow");
        var newPaddingLeft = sceneRow.style.paddingLeft;
        newPaddingLeft.value = (192 - sceneRow.childCount * 52 - (18 * (sceneRow.childCount - 1))) / 2f;
        sceneRow.style.paddingLeft = newPaddingLeft;
    }

    private void UnSelectScenes()
    {
        var scene = _root.Q<Button>(null, "SceneSelected");

        scene.RemoveFromClassList("SceneSelected");
        scene.Children().First().RemoveFromClassList("SceneFrontgroundSelected");
        scene.Children().First().Children().First().RemoveFromClassList("SceneTextSelected");
    }

    private void SelectScene(VisualElement scene)
    {
        scene.AddToClassList("SceneSelected");
        scene.Children().First().AddToClassList("SceneFrontgroundSelected");
        scene.Children().First().Children().First().AddToClassList("SceneTextSelected");
    }
}
using System;
using System.Linq;
using UISceneTool.Scripts;
using UnityEngine;
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

        public void OnPointerDown(Action<PointerDownEvent> action)
        {
            Background.RegisterCallback<PointerDownEvent>(evt => { action?.Invoke(evt); }, TrickleDown.TrickleDown);
        }

        public void OnPointerUp(Action<PointerUpEvent> action)
        {
            Background.RegisterCallback<PointerUpEvent>(evt => { action?.Invoke(evt); });
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
    }

    public void OnClickCamera(Action action)
    {
        _root.Q<Button>("Camera").clicked += () => { action?.Invoke(); };
    }

    public void OnCameraDrag(Action<PointerMoveEvent> action)
    {
        _camera.Background.RegisterCallback<PointerMoveEvent>(evt => { action?.Invoke(evt); });
    }
}
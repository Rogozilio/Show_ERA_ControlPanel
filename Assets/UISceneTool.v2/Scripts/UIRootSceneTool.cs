using System.Collections.Generic;
using System.Linq;
using UISceneTool.Scripts;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIRootSceneTool
{
    public struct UIButtonSceneTool
    {
        public Button Background;
        public VisualElement Foreground;
        public VisualElement Icon;

        public UIButtonSceneTool(Button background)
        {
            Background = background;
            Foreground = Background.Children().First();
            Icon = Foreground.Children().First();
        }
    }

    public VisualElement GetCamera => _camera.Background;

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
    
    private MouseScrollManipulation _mouseScrollManipulation;
    private DragCameraManipulator _dragCameraManipulator;

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

        _mouseScrollManipulation = new MouseScrollManipulation(null);
        _dragCameraManipulator = new DragCameraManipulator(null, null, null, SwitchCameraElements);
        
        _zoomPlus.Background.AddManipulator(new ResizeButtonManipulator());
        _zoomMinus.Background.AddManipulator(new ResizeButtonManipulator());
        _up.Background.AddManipulator(new ResizeButtonManipulator());
        _left.Background.AddManipulator(new ResizeButtonManipulator());
        _down.Background.AddManipulator(new ResizeButtonManipulator());
        _right.Background.AddManipulator(new ResizeButtonManipulator());
        _return.Background.AddManipulator(new ResizeButtonManipulator());
        _sound.Background.AddManipulator(new ResizeButtonManipulator());
        _sound.Background.AddManipulator(_mouseScrollManipulation);
        _camera.Background.AddManipulator(new ResizeButtonManipulator());
        _camera.Background.AddManipulator(_dragCameraManipulator);
        
        _zoomPlus.Foreground.AddManipulator(new MouseHoverManipulation());
        _zoomMinus.Foreground.AddManipulator(new MouseHoverManipulation());
        _up.Foreground.AddManipulator(new MouseHoverManipulation());
        _left.Foreground.AddManipulator(new MouseHoverManipulation());
        _down.Foreground.AddManipulator(new MouseHoverManipulation());
        _right.Foreground.AddManipulator(new MouseHoverManipulation());
        _return.Foreground.AddManipulator(new MouseHoverManipulation());
        _sound.Foreground.AddManipulator(new MouseHoverManipulation());
        _camera.Foreground.AddManipulator(new MouseHoverManipulation());
    }

    private bool _isCameraDisable => _camera.Background.ClassListContains("ColorWhite");

    #region Camera

    private void CameraBtnSwitch()
        {
            _camera.Background.ToggleInClassList("ColorWhite");
            _camera.Background.ToggleInClassList("ColorGreen");
            _camera.Foreground.ToggleInClassList("ColorWhite");
            _camera.Foreground.ToggleInClassList("ColorGreen");
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
        scene.AddManipulator(new EventClickManipulation(() =>
        {
            UnSelectScenes();
            SelectScene(scene);
        }, action));

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
        
        scene.AddManipulator(new ResizeButtonManipulator());
        sceneForeground.AddManipulator(new MouseHoverManipulation());
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
        
        if(scene == null) return;
        
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

    public void SoundDisabled()
    {
        _root.Q<Button>("Sound").AddToClassList("SoundDisabledWhite");
        _root.Q<VisualElement>("SoundForeground").AddToClassList("SoundDisabledGrey");
        _root.Q<VisualElement>("SoundIcon").AddToClassList("SoundDisabledWhite");
        _root.Q<VisualElement>("SoundBar").AddToClassList("SoundDisabledWhite");
        _root.Q<Button>("Sound").RemoveManipulator(_mouseScrollManipulation);
    }

    private void SoundDeactivated()
    {
        _root.Q<Button>("Sound").AddToClassList("SoundDisabledWhite");
        _root.Q<VisualElement>("SoundForeground").AddToClassList("DeactivateForeground");
        _root.Q<VisualElement>("SoundIcon").AddToClassList("SoundDisabledWhite");
        _root.Q<VisualElement>("SoundBar").AddToClassList("SoundDisabledWhite");
        _root.Q<Button>("Sound").RemoveManipulator(_mouseScrollManipulation);
    }

    #endregion
}
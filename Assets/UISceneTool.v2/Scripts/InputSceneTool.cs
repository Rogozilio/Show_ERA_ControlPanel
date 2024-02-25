using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSceneTool
{
    public class InputHoldData
    {
        public bool isPressed;
        public Action<InputAction.CallbackContext> onStartHold;
        public Action<InputAction.CallbackContext> onHold;
        public Action<InputAction.CallbackContext> onEndHold;
    }

    public class InputClickData
    {
        public Action<InputAction.CallbackContext> onStartClick;
        public Action<InputAction.CallbackContext> onEndClick;
    }

    private InputActionAsset _inputActionAsset;

    private readonly InputAction _up;
    private readonly InputAction _down;
    private readonly InputAction _left;
    private readonly InputAction _right;
    private readonly InputAction _zoomIn;
    private readonly InputAction _zoomOut;
    private readonly InputAction _switchPanel;
    private readonly InputAction _switchSound;
    private readonly InputAction _returnBack;
    private readonly InputAction _nextLocation;
    private readonly InputAction _prevLocation;
    private readonly InputAction _numberLocation;

    private readonly InputHoldData _holdUp = new InputHoldData();
    private readonly InputHoldData _holdDown = new InputHoldData();
    private readonly InputHoldData _holdLeft = new InputHoldData();
    private readonly InputHoldData _holdRight = new InputHoldData();
    private readonly InputHoldData _holdZoomIn = new InputHoldData();
    private readonly InputHoldData _holdZoomOut = new InputHoldData();
    private readonly InputClickData _clickReturn = new InputClickData();
    private readonly InputClickData _clickCamera = new InputClickData();
    private readonly InputClickData _clickSound = new InputClickData();
    private readonly InputClickData _clickScene = new InputClickData();
    private readonly InputClickData _clickNextScene = new InputClickData();
    private readonly InputClickData _clickPrevScene = new InputClickData();

    public InputSceneTool(InputActionAsset inputActionAsset)
    {
        _inputActionAsset = inputActionAsset;

        _up = _inputActionAsset.FindAction("Up");
        _down = _inputActionAsset.FindAction("Down");
        _left = _inputActionAsset.FindAction("Left");
        _right = _inputActionAsset.FindAction("Right");
        _zoomIn = _inputActionAsset.FindAction("ZoomIn");
        _zoomOut = _inputActionAsset.FindAction("ZoomOut");
        _switchPanel = _inputActionAsset.FindAction("SwitchPanel");
        _switchSound = _inputActionAsset.FindAction("SwitchSound");
        _returnBack = _inputActionAsset.FindAction("ReturnBack");
        _nextLocation = _inputActionAsset.FindAction("NextLocation");
        _prevLocation = _inputActionAsset.FindAction("PrevLocation");
        _numberLocation = _inputActionAsset.FindAction("NumberLocation");

        #region InitHold

        _up.started += (ctx) =>
        {
            _holdUp.isPressed = true;
            _holdUp.onStartHold?.Invoke(ctx);
        };
        _down.started += (ctx) =>
        {
            _holdDown.isPressed = true;
            _holdDown.onStartHold?.Invoke(ctx);
        };
        _left.started += (ctx) =>
        {
            _holdLeft.isPressed = true;
            _holdLeft.onStartHold?.Invoke(ctx);
        };
        _right.started += (ctx) =>
        {
            _holdRight.isPressed = true;
            _holdRight.onStartHold?.Invoke(ctx);
        };
        _zoomIn.started += (ctx) =>
        {
            _holdZoomIn.isPressed = true;
            _holdZoomIn.onStartHold?.Invoke(ctx);
        };
        _zoomOut.started += (ctx) =>
        {
            _holdZoomOut.isPressed = true;
            _holdZoomOut.onStartHold?.Invoke(ctx);
        };

        _up.performed += async (ctx) => { await HoldButton(_holdUp, ctx); };
        _down.performed += async (ctx) => { await HoldButton(_holdDown, ctx); };
        _left.performed += async (ctx) => { await HoldButton(_holdLeft, ctx); };
        _right.performed += async (ctx) => { await HoldButton(_holdRight, ctx); };
        _zoomIn.performed += async (ctx) => { await HoldButton(_holdZoomIn, ctx); };
        _zoomOut.performed += async (ctx) => { await HoldButton(_holdZoomOut, ctx); };

        _up.canceled += (ctx) =>
        {
            _holdUp.isPressed = false;
            _holdUp.onEndHold?.Invoke(ctx);
        };
        _down.canceled += (ctx) =>
        {
            _holdDown.isPressed = false;
            _holdDown.onEndHold?.Invoke(ctx);
        };
        _left.canceled += (ctx) =>
        {
            _holdLeft.isPressed = false;
            _holdLeft.onEndHold?.Invoke(ctx);
        };
        _right.canceled += (ctx) =>
        {
            _holdRight.isPressed = false;
            _holdRight.onEndHold?.Invoke(ctx);
        };
        _zoomIn.canceled += (ctx) =>
        {
            _holdZoomIn.isPressed = false;
            _holdZoomIn.onEndHold?.Invoke(ctx);
        };
        _zoomOut.canceled += (ctx) =>
        {
            _holdZoomOut.isPressed = false;
            _holdZoomOut.onEndHold?.Invoke(ctx);
        };

        #endregion

        #region InitClick

        _switchPanel.started += (ctx) => { _clickCamera.onStartClick?.Invoke(ctx); };
        _returnBack.started += (ctx) => { _clickReturn.onStartClick?.Invoke(ctx); };
        _switchSound.started += (ctx) => { _clickSound.onStartClick?.Invoke(ctx); };
        _numberLocation.started += (ctx) => { _clickScene.onStartClick?.Invoke(ctx); };
        _nextLocation.started += (ctx) => { _clickNextScene.onStartClick?.Invoke(ctx); };
        _prevLocation.started += (ctx) => { _clickPrevScene.onStartClick?.Invoke(ctx); };

        _switchPanel.canceled += (ctx) => { _clickCamera.onEndClick?.Invoke(ctx); };
        _returnBack.canceled += (ctx) => { _clickReturn.onEndClick?.Invoke(ctx); };
        _switchSound.canceled += (ctx) => { _clickSound.onEndClick?.Invoke(ctx); };
        _numberLocation.canceled += (ctx) => { _clickScene.onEndClick?.Invoke(ctx); };
        _nextLocation.canceled += (ctx) => { _clickNextScene.onEndClick?.Invoke(ctx); };
        _prevLocation.canceled += (ctx) => { _clickPrevScene.onEndClick?.Invoke(ctx); };

        #endregion
    }

    #region EventsHold

    public void EventsHoldUp(Action<InputAction.CallbackContext> onStart
        , Action<InputAction.CallbackContext> onHold
        , Action<InputAction.CallbackContext> onEnd)
    {
        _holdUp.onStartHold += onStart;
        _holdUp.onHold += onHold;
        _holdUp.onEndHold += onEnd;
    }

    public void EventsHoldDown(Action<InputAction.CallbackContext> onStart
        , Action<InputAction.CallbackContext> onHold
        , Action<InputAction.CallbackContext> onEnd)
    {
        _holdDown.onStartHold += onStart;
        _holdDown.onHold += onHold;
        _holdDown.onEndHold += onEnd;
    }

    public void EventsHoldLeft(Action<InputAction.CallbackContext> onStart
        , Action<InputAction.CallbackContext> onHold
        , Action<InputAction.CallbackContext> onEnd)
    {
        _holdLeft.onStartHold += onStart;
        _holdLeft.onHold += onHold;
        _holdLeft.onEndHold += onEnd;
    }

    public void EventsHoldRight(Action<InputAction.CallbackContext> onStart
        , Action<InputAction.CallbackContext> onHold
        , Action<InputAction.CallbackContext> onEnd)
    {
        _holdRight.onStartHold += onStart;
        _holdRight.onHold += onHold;
        _holdRight.onEndHold += onEnd;
    }

    public void EventsHoldZoomIn(Action<InputAction.CallbackContext> onStart
        , Action<InputAction.CallbackContext> onHold
        , Action<InputAction.CallbackContext> onEnd)
    {
        _holdZoomIn.onStartHold += onStart;
        _holdZoomIn.onHold += onHold;
        _holdZoomIn.onEndHold += onEnd;
    }

    public void EventsHoldZoomOut(Action<InputAction.CallbackContext> onStart
        , Action<InputAction.CallbackContext> onHold
        , Action<InputAction.CallbackContext> onEnd)
    {
        _holdZoomOut.onStartHold += onStart;
        _holdZoomOut.onHold += onHold;
        _holdZoomOut.onEndHold += onEnd;
    }

    private async UniTask HoldButton(InputHoldData holdData, InputAction.CallbackContext ctx)
    {
        while (holdData.isPressed)
        {
            holdData.onHold?.Invoke(ctx);
            await UniTask.WaitForFixedUpdate();
        }
    }

    #endregion
    
    #region EventsClick

    public void EventsClickCamera(Action<InputAction.CallbackContext> onStart
        , Action<InputAction.CallbackContext> onEnd)
    {
        _clickCamera.onStartClick += onStart;
        _clickCamera.onEndClick += onEnd;
    }

    public void EventsClickReturn(Action<InputAction.CallbackContext> onStart
        , Action<InputAction.CallbackContext> onEnd)
    {
        _clickReturn.onStartClick += onStart;
        _clickReturn.onEndClick += onEnd;
    }

    public void EventsClickSound(Action<InputAction.CallbackContext> onStart
        , Action<InputAction.CallbackContext> onEnd)
    {
        _clickSound.onStartClick += onStart;
        _clickSound.onEndClick += onEnd;
    }
    
    public void EventsClickScene(Action<InputAction.CallbackContext> onStart
        , Action<InputAction.CallbackContext> onEnd)
    {
        _clickScene.onStartClick += onStart;
        _clickScene.onEndClick += onEnd;
    }
    
    public void EventsClickNextScene(Action<InputAction.CallbackContext> onStart
        , Action<InputAction.CallbackContext> onEnd)
    {
        _clickNextScene.onStartClick += onStart;
        _clickNextScene.onEndClick += onEnd;
    }
    
    public void EventsClickPrevScene(Action<InputAction.CallbackContext> onStart
        , Action<InputAction.CallbackContext> onEnd)
    {
        _clickPrevScene.onStartClick += onStart;
        _clickPrevScene.onEndClick += onEnd;
    }

    #endregion

    public void Enable()
    {
        _inputActionAsset.Enable();
    }

    public void Disable()
    {
        _inputActionAsset.Disable();
    }
}
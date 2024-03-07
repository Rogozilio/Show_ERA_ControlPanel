using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DragCameraManipulator : PointerManipulator
{
    public struct ObstacleData
    {
        public RectTransform Transform;
        private Vector2 _size => Transform.rect.size * Transform.lossyScale;

        public Vector2 TopLeft => new Vector2(Transform.position.x + _size.x * (-Transform.pivot.x),
            Transform.position.y + _size.y * (1f - Transform.pivot.y));

        public Vector2 TopCenter => new Vector2(Transform.position.x + _size.x * (0.5f - Transform.pivot.x),
            Transform.position.y + _size.y * (1f - Transform.pivot.y));

        public Vector2 TopRight => new Vector2(Transform.position.x + _size.x * (1f - Transform.pivot.x),
            Transform.position.y + _size.y * (1f - Transform.pivot.y));

        public Vector2 MiddleLeft => new Vector2(Transform.position.x + _size.x * (-Transform.pivot.x),
            Transform.position.y + _size.y * (0.5f - Transform.pivot.y));

        public Vector2 MiddleCenter => new Vector2(Transform.position.x + _size.x * (0.5f - Transform.pivot.x),
            Transform.position.y + _size.y * (0.5f - Transform.pivot.y));

        public Vector2 MiddleRight => new Vector2(Transform.position.x + _size.x * (1f - Transform.pivot.x),
            Transform.position.y + _size.y * (0.5f - Transform.pivot.y));

        public Vector2 BottomLeft => new Vector2(Transform.position.x + _size.x * (-Transform.pivot.x),
            Transform.position.y + _size.y * (-Transform.pivot.y));

        public Vector2 BottomCenter => new Vector2(Transform.position.x + _size.x * (0.5f - Transform.pivot.x),
            Transform.position.y + _size.y * (-Transform.pivot.y));

        public Vector2 BottomRight => new Vector2(Transform.position.x + _size.x * (1f - Transform.pivot.x),
            Transform.position.y + _size.y * (-Transform.pivot.y));

        public ObstacleData(RectTransform transform)
        {
            Transform = transform;
        }
    }

    public struct TargetData
    {
        public VisualElement Window;
        public VisualElement Camera;

        public float multiplyResolution;
        private Vector3 _mousePosition;
        private Vector2 _sizeWindow;
        private Vector2 _sizeLayout;
        private Vector2 _halfSizeWindow;
        public Vector2 offset;

        public Vector2 Position =>
            new Vector2(Window.transform.position.x,
                _sizeLayout.y - Window.resolvedStyle.height - Window.transform.position.y) * multiplyResolution;

        public Vector2 SizeWindow => _sizeWindow;
        public Vector2 SizeLayout => _sizeLayout;
        public Vector2 CenterWindow => Position + _halfSizeWindow;
        public Vector2 CenterLeftWindow => new Vector2(CenterWindow.x - _halfSizeWindow.x, CenterWindow.y);
        public Vector2 CenterRightWindow => new Vector2(CenterWindow.x + _halfSizeWindow.x, CenterWindow.y);
        public Vector2 CenterTopWindow => new Vector2(CenterWindow.x, CenterWindow.y + _halfSizeWindow.y);
        public Vector2 CenterBottomWindow => new Vector2(CenterWindow.x, CenterWindow.y - _halfSizeWindow.y);

        public Vector2 TopLeftWindow =>
            new Vector2(CenterWindow.x - _halfSizeWindow.x, CenterWindow.y + _halfSizeWindow.y);

        public Vector2 TopRightWindow =>
            new Vector2(CenterWindow.x + _halfSizeWindow.x, CenterWindow.y + _halfSizeWindow.y);

        public Vector2 BottomLeftWindow =>
            new Vector2(CenterWindow.x - _halfSizeWindow.x, CenterWindow.y - _halfSizeWindow.y);

        public Vector2 BottomRightWindow =>
            new Vector2(CenterWindow.x + _halfSizeWindow.x, CenterWindow.y - _halfSizeWindow.y);

        public TargetData(VisualElement target, Vector3 mousePosition)
        {
            Camera = target;
            Window = target.parent.parent;
            _mousePosition = mousePosition;
            multiplyResolution = Screen.width / 1920f;
            _sizeWindow = Window.worldBound.size * multiplyResolution;
            _halfSizeWindow = _sizeWindow / 2f;
            _sizeLayout = new Vector2(Window.parent.resolvedStyle.width, Window.parent.resolvedStyle.height);
            offset = Window.transform.position - _mousePosition;
        }

        public Vector2 ClosestCorner(Vector2 obstacleCenter)
        {
            var fromCenterToCenter = obstacleCenter - CenterWindow;
            var corners = new List<Vector2>();
            corners.Add(TopLeftWindow - CenterWindow);
            corners.Add(CenterTopWindow - CenterWindow);
            corners.Add(TopRightWindow - CenterWindow);
            corners.Add(CenterLeftWindow - CenterWindow);
            corners.Add(CenterRightWindow - CenterWindow);
            corners.Add(BottomLeftWindow - CenterWindow);
            corners.Add(CenterBottomWindow - CenterWindow);
            corners.Add(BottomRightWindow - CenterWindow);

            Vector2 result = default;
            var prevDot = 0f;
            foreach (var corner in corners)
            {
                var dot = Vector2.Dot(fromCenterToCenter.normalized, corner.normalized);
                if (dot > prevDot)
                {
                    prevDot = dot;
                    result = corner + CenterWindow;
                }
            }

            return result;
        }
    }

    private Vector2 _directionMove;
    private Vector2 _offset;

    private float _offsetTop;
    private float _offsetBottom;
    private float _offsetLeft;
    private float _offsetRight;

    private bool _isDrag;

    private Action _onDown;
    private Action _onDrag;
    private Action _onUp;
    private Action _onUpWithoutDrag;

    private TargetData _target;
    private List<RectTransform> _obstacles = new List<RectTransform>();
    private List<ObstacleData> _obstacleDatas = new List<ObstacleData>();

    public List<RectTransform> SetObstacle
    {
        set
        {
            foreach (var obstacle in value)
            {
                _obstacleDatas.Add(new ObstacleData(obstacle));
            }
        }
    }

    public DragCameraManipulator(Action onDown = null, Action onDrag = null, Action onUp = null,
        Action onUpWithoutDrag = null)
    {
        _onDown = onDown;
        _onDrag = onDrag;
        _onUp = onUp;
        _onUpWithoutDrag = onUpWithoutDrag;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
    }

    private Vector3 offsetStartPosition { get; set; }

    private void OnPointerDown(PointerDownEvent evt)
    {
        _onDown?.Invoke();
        _target = new TargetData(target, evt.position);
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (target.HasPointerCapture(evt.pointerId))
        {
            _isDrag = true;
            _onDrag?.Invoke();

            _target.Window.transform.position = Calculate(evt.position);
            _target.Window.transform.position = new Vector2(
                Mathf.Clamp(_target.Window.transform.position.x, 0,
                    _target.SizeLayout.x - _target.Window.resolvedStyle.width),
                Mathf.Clamp(_target.Window.transform.position.y, 0,
                    _target.SizeLayout.y - _target.Window.resolvedStyle.height));

            ShowOrHideEdgeWindow();
        }
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (_isDrag) _onUp?.Invoke();
        else _onUpWithoutDrag?.Invoke();
        _isDrag = false;
        ShowOrHideEdgeWindow(false);
    }

    private void ShowOrHideEdgeWindow(bool isShow = true)
    {
        if (isShow && !target.parent.parent.ClassListContains("WindowShow"))
        {
            target.parent.parent.AddToClassList("WindowShow");
        }
        else if (!isShow && target.parent.parent.ClassListContains("WindowShow"))
        {
            target.parent.parent.RemoveFromClassList("WindowShow");
        }
    }

    public Vector3 Calculate(Vector3 mousePosition)
    {
        var nextXPosition = mousePosition.x  + _target.offset.x;
        var nextYPosition = mousePosition.y + _target.offset.y;
        GizmoDrawer.Instance.ClearDrawLine();
        foreach (var obstacle in _obstacleDatas)
        {
            GizmoDrawer.Instance.AddDrawLine(_target.CenterTopWindow, _target.CenterBottomWindow, Color.green);
            GizmoDrawer.Instance.AddDrawLine(_target.CenterLeftWindow, _target.CenterRightWindow, Color.green);
            GizmoDrawer.Instance.AddDrawLine(obstacle.MiddleCenter, _target.ClosestCorner(obstacle.MiddleCenter),
                Color.green);
            GizmoDrawer.Instance.AddDrawLine(obstacle.BottomLeft, obstacle.BottomRight, Color.red);
            GizmoDrawer.Instance.AddDrawLine(obstacle.TopLeft, obstacle.TopRight, Color.blue);


            if (IsLineIntersection(obstacle.TopLeft, obstacle.BottomLeft, obstacle.MiddleCenter,
                    _target.ClosestCorner(obstacle.MiddleCenter)))
            {
                nextXPosition = Mathf.Clamp(nextXPosition, 0,
                    (obstacle.TopLeft.x - _target.SizeWindow.x) / _target.multiplyResolution);
                Debug.Log("Left");
                Debug.Log("X " + nextXPosition + " Next " +
                          (obstacle.TopLeft.x - _target.SizeWindow.x));
                continue;
            }

            if (IsLineIntersection(obstacle.TopRight, obstacle.BottomRight,
                    obstacle.MiddleCenter, _target.ClosestCorner(obstacle.MiddleCenter)))
            {
                nextXPosition = Mathf.Clamp(nextXPosition, obstacle.TopRight.x / _target.multiplyResolution,
                    nextXPosition);
                Debug.Log("Right");
                continue;
            }

            if (IsLineIntersection(obstacle.BottomLeft, obstacle.BottomRight,
                    obstacle.MiddleCenter, _target.ClosestCorner(obstacle.MiddleCenter)))
            {
                nextYPosition = Mathf.Clamp(nextYPosition, _target.SizeLayout.y - obstacle.BottomLeft.y / _target.multiplyResolution,
                    nextYPosition);
                Debug.Log("Top");
               
                continue;
            }

            if (IsLineIntersection(obstacle.TopLeft, obstacle.TopRight, obstacle.MiddleCenter,
                    _target.ClosestCorner(obstacle.MiddleCenter)))
            {
                nextYPosition = Mathf.Clamp(nextYPosition, nextYPosition,
                    _target.SizeLayout.y + (- obstacle.TopRight.y - _target.SizeWindow.y ) /  _target.multiplyResolution);
                Debug.Log("Bottom");
                Debug.Log("Y " + nextYPosition + " Next " + (obstacle.TopLeft.y));
                continue;
            }
        }

        return new Vector3(nextXPosition, nextYPosition);
    }
    private bool IsLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        Vector2 a = p2 - p1;
        Vector2 b = p3 - p4;
        Vector2 c = p1 - p3;

        float alphaNumerator = b.y * c.x - b.x * c.y;
        float alphaDenominator = a.y * b.x - a.x * b.y;
        float betaNumerator = a.x * c.y - a.y * c.x;
        float betaDenominator = a.y * b.x - a.x * b.y;

        bool doIntersect = true;

        if (alphaDenominator == 0 || betaDenominator == 0)
        {
            doIntersect = false;
        }
        else
        {
            if (alphaDenominator > 0)
            {
                if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
                {
                    doIntersect = false;
                }
            }
            else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
            {
                doIntersect = false;
            }

            if (doIntersect && betaDenominator > 0)
            {
                if (betaNumerator < 0 || betaNumerator > betaDenominator)
                {
                    doIntersect = false;
                }
            }
            else if (betaNumerator > 0 || betaNumerator < betaDenominator)
            {
                doIntersect = false;
            }
        }

        return doIntersect;
    }
}
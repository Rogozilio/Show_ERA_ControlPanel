using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DragCameraManipulator : PointerManipulator
{
    public bool isStaticOffsetTop;
    public bool isStaticOffsetBottom;
    public bool isStaticOffsetLeft;
    public bool isStaticOffsetRight;

    private Vector2 _directionMove;
    private Vector2 _offset;
    private Vector2 _centerLeft;
    private Vector2 _centerRight;
    private Vector2 _centerTop;
    private Vector2 _centerBottom;
    private Vector2 _sizeWindow;

    private Vector2 _center;
    private Vector2 _halfSize;

    private float _offsetTop;
    private float _offsetBottom;
    private float _offsetLeft;
    private float _offsetRight;

    private bool _isDrag;

    private Action _onDown;
    private Action _onDrag;
    private Action _onUp;
    private Action _onUpWithoutDrag;

    public DragCameraManipulator(Action onDown = null, Action onDrag = null, Action onUp = null, Action onUpWithoutDrag = null)
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
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
    }

    private Vector3 offsetStartPosition { get; set; }

    private void OnPointerDown(PointerDownEvent evt)
    {
        _onDown?.Invoke();
        offsetStartPosition = (Vector3)target.worldBound.position - evt.position;
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (target.HasPointerCapture(evt.pointerId))
        {
            _isDrag = true;
            _onDrag?.Invoke();
            
            Vector3 pointerDelta = evt.position - (Vector3)target.worldBound.position + offsetStartPosition;

            var window = target.parent.parent;
            var root = window.parent;

            window.transform.position = new Vector2(
                Mathf.Clamp(window.transform.position.x + pointerDelta.x, 0,
                    root.resolvedStyle.width - window.resolvedStyle.width),
                Mathf.Clamp(window.transform.position.y + pointerDelta.y, 0,
                    root.resolvedStyle.height - window.resolvedStyle.height));
            
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

    public Vector3 Calculate(Vector3 windowPosition, Vector3 mousePosition, Vector3 deltaPosition)
    {
        _directionMove = deltaPosition;

        _halfSize = HalfSize(_sizeWindow);
        _center = Center(mousePosition, _sizeWindow);
        RefreshOffset(windowPosition, mousePosition);
        //RefreshOffsetValue();
        _centerTop = new Vector2(_center.x, _center.y + _halfSize.y /* + offsetTop*/);
        _centerBottom = new Vector2(_center.x, _center.y - _halfSize.y /* + offsetBottom*/);
        _centerLeft = new Vector2(_center.x - _halfSize.x /* + offsetLeft*/, _center.y);
        _centerRight = new Vector2(_center.x + _halfSize.x /* + offsetRight*/, _center.y);

        var nextXPosition = float.NaN;
        var nextYPosition = float.NaN;

        // foreach (var obstacle in obstacles)
        // {
        //     var topRight = Center(obstacle) + new Vector2(HalfSize(obstacle).x, HalfSize(obstacle).y);
        //     var bottomRight = Center(obstacle) +
        //                       new Vector2(HalfSize(obstacle).x, -HalfSize(obstacle).y);
        //     var topLeft = Center(obstacle) + new Vector2(-HalfSize(obstacle).x, HalfSize(obstacle).y);
        //     var bottomLeft = Center(obstacle) +
        //                      new Vector2(-HalfSize(obstacle).x, -HalfSize(obstacle).y);
        //
        //     if (LeftCollision(obstacle) && IsLineIntersection(topRight, bottomRight, Center(obstacle), _getCenter))
        //     {
        //         nextXPosition = Center(obstacle).x + HalfSize(obstacle).x + _getHalfSize.x;// - offsetLeft;
        //         continue;
        //     }
        //
        //     if (RightCollision(obstacle) && IsLineIntersection(topLeft, bottomLeft, Center(obstacle), _getCenter))
        //     {
        //         nextXPosition = Center(obstacle).x - HalfSize(obstacle).x - _getHalfSize.x;// - offsetRight;
        //         continue;
        //     }
        //
        //     if (TopCollision(obstacle) && IsLineIntersection(bottomLeft, bottomRight, Center(obstacle), _getCenter))
        //     {
        //         nextYPosition = Center(obstacle).y - HalfSize(obstacle).y - _getHalfSize.y;// - offsetTop;
        //         continue;
        //     }
        //
        //     if (BottomCollision(obstacle) && IsLineIntersection(topLeft, topRight, Center(obstacle), _getCenter))
        //     {
        //         nextYPosition = Center(obstacle).y + HalfSize(obstacle).y + _getHalfSize.y;// - offsetBottom;
        //         continue;
        //     }
        // }

        nextXPosition = CheckXInsideScreen(nextXPosition);
        nextYPosition = CheckYInsideScreen(nextYPosition);

        Debug.Log(nextXPosition);

        return new Vector3(
            float.IsNaN(nextXPosition)
                ? CheckXInsideScreen(mousePosition.x /* - _directionMove.x*/)
                : nextXPosition,
            float.IsNaN(nextYPosition)
                ? CheckYInsideScreen(mousePosition.y /* - _directionMove.y*/)
                : nextYPosition);
    }

    private Vector2 Center(Vector3 position, Vector2 size)
    {
        return new Vector2(position.x + size.x * 0.5f, position.y + size.y * 0.5f);
    }

    private Vector2 HalfSize(Vector2 size)
    {
        return new Vector2(size.x / 2f, size.y / 2f);
    }

    private void RefreshOffset(Vector3 windowPosition, Vector3 mousePosition)
    {
        _offsetLeft = mousePosition.x - windowPosition.x;
        _offsetRight = _sizeWindow.x - _offsetLeft;
        _offsetBottom = mousePosition.y - windowPosition.y;
        _offsetTop = _sizeWindow.y - _offsetBottom;
        Debug.Log(_offsetLeft);
    }

    private float CheckXInsideScreen(float posX)
    {
        var minX = 0; //_offsetLeft;
        var maxX = Screen.width; // - _offsetRight;

        return Mathf.Clamp(posX, minX, maxX);
    }

    private float CheckYInsideScreen(float posY)
    {
        var minY = 0; //_offsetBottom;
        var maxY = Screen.height; // - _offsetTop;

        return Mathf.Clamp(posY, minY, maxY);
    }

    private bool IsInsideObstacle(Vector2 point, RectTransform obstacle)
    {
        var xMin = obstacle.position.x - obstacle.rect.width * obstacle.pivot.x;
        var yMin = obstacle.position.y - obstacle.rect.height * obstacle.pivot.y;
        var xMax = xMin + obstacle.rect.width;
        var yMax = yMin + obstacle.rect.height;
        return point.x - _directionMove.normalized.x > xMin && point.x - _directionMove.normalized.x < xMax &&
               point.y - _directionMove.normalized.y > yMin && point.y - _directionMove.normalized.y < yMax;
    }

    private bool LeftCollision(RectTransform obstacle)
    {
        var leftTop = _centerLeft + new Vector2(0, _halfSize.y);
        var leftDown = _centerLeft - new Vector2(0, _halfSize.y);

        return IsInsideObstacle(leftTop, obstacle)
               || IsInsideObstacle(_centerLeft, obstacle)
               || IsInsideObstacle(leftDown, obstacle);
    }

    private bool RightCollision(RectTransform obstacle)
    {
        var rightTop = _centerRight + new Vector2(0, _halfSize.y);
        var rightDown = _centerRight - new Vector2(0, _halfSize.y);

        return IsInsideObstacle(rightTop, obstacle)
               || IsInsideObstacle(_centerRight, obstacle)
               || IsInsideObstacle(rightDown, obstacle);
    }

    private bool TopCollision(RectTransform obstacle)
    {
        var topLeft = _centerTop - new Vector2(_halfSize.x, 0);
        var topRight = _centerTop + new Vector2(_halfSize.x, 0);

        return IsInsideObstacle(topLeft, obstacle)
               || IsInsideObstacle(_centerTop, obstacle)
               || IsInsideObstacle(topRight, obstacle);
    }

    private bool BottomCollision(RectTransform obstacle)
    {
        var bottomLeft = _centerBottom - new Vector2(_halfSize.x, 0);
        var bottomRight = _centerBottom + new Vector2(_halfSize.x, 0);

        return IsInsideObstacle(bottomLeft, obstacle)
               || IsInsideObstacle(_centerBottom, obstacle)
               || IsInsideObstacle(bottomRight, obstacle);
    }

    // private void RefreshOffsetValue()
    // {
    //     if (!isStaticOffsetTop)
    //         offsetTop = offsetRectTransform.offsetMax.y;
    //     if (!isStaticOffsetBottom)
    //         offsetBottom = offsetRectTransform.offsetMin.y;
    //     if (!isStaticOffsetLeft)
    //         offsetLeft = offsetRectTransform.offsetMin.x;
    //     if (!isStaticOffsetRight)
    //         offsetRight = offsetRectTransform.offsetMax.x;
    // }

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
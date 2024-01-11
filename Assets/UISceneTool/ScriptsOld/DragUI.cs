using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ERA
{
    public class DragUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public enum MoveRect
        {
            Self,
            Parent,
            Another
        }

        public MoveRect moveRect;
        public RectTransform moveRectTransform;
        public RectTransform[] obstacles;
        public UnityEvent onBeginDrag;
        public UnityEvent onDrag;
        public UnityEvent onEndDrag;
        public RectTransform offsetRectTransform;
        public float offsetTop;
        public float offsetBottom;
        public float offsetLeft;
        public float offsetRight;
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

        private Vector2 _getCenter => Center(moveRectTransform);
        private Vector2 _getHalfSize => HalfSize(moveRectTransform);

        private void Awake()
        {
            switch (moveRect)
            {
                case MoveRect.Self:
                    moveRectTransform = (RectTransform)transform;
                    break;
                case MoveRect.Parent:
                    moveRectTransform = (RectTransform)transform.parent;
                    break;
                case MoveRect.Another:
                    if (!moveRectTransform)
                        Debug.LogError("MoveRectTransform is not set. Set value in editor");
                    break;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _offset = (Vector2)moveRectTransform.position - eventData.position;
            onBeginDrag?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!eventData.dragging) return;

            _directionMove = (Vector2)moveRectTransform.position - eventData.position - _offset;
            RefreshOffsetValue();
            _centerTop = new Vector2(_getCenter.x, _getCenter.y + _getHalfSize.y + offsetTop);
            _centerBottom = new Vector2(_getCenter.x, _getCenter.y - _getHalfSize.y + offsetBottom);
            _centerLeft = new Vector2(_getCenter.x - _getHalfSize.x + offsetLeft, _getCenter.y);
            _centerRight = new Vector2(_getCenter.x + _getHalfSize.x + offsetRight, _getCenter.y);

            var nextXPosition = float.NaN;
            var nextYPosition = float.NaN;

            foreach (var obstacle in obstacles)
            {
                var topRight = Center(obstacle) + new Vector2(HalfSize(obstacle).x, HalfSize(obstacle).y);
                var bottomRight = Center(obstacle) +
                                  new Vector2(HalfSize(obstacle).x, -HalfSize(obstacle).y);
                var topLeft = Center(obstacle) + new Vector2(-HalfSize(obstacle).x, HalfSize(obstacle).y);
                var bottomLeft = Center(obstacle) +
                                 new Vector2(-HalfSize(obstacle).x, -HalfSize(obstacle).y);

                if (LeftCollision(obstacle) && IsLineIntersection(topRight, bottomRight, Center(obstacle), _getCenter))
                {
                    nextXPosition = Center(obstacle).x + HalfSize(obstacle).x + _getHalfSize.x - offsetLeft;
                    continue;
                }

                if (RightCollision(obstacle) && IsLineIntersection(topLeft, bottomLeft, Center(obstacle), _getCenter))
                {
                    nextXPosition = Center(obstacle).x - HalfSize(obstacle).x - _getHalfSize.x - offsetRight;
                    continue;
                }

                if (TopCollision(obstacle) && IsLineIntersection(bottomLeft, bottomRight, Center(obstacle), _getCenter))
                {
                    nextYPosition = Center(obstacle).y - HalfSize(obstacle).y - _getHalfSize.y - offsetTop;
                    continue;
                }

                if (BottomCollision(obstacle) && IsLineIntersection(topLeft, topRight, Center(obstacle), _getCenter))
                {
                    nextYPosition = Center(obstacle).y + HalfSize(obstacle).y + _getHalfSize.y - offsetBottom;
                    continue;
                }
            }

            nextXPosition = CheckXInsideScreen(nextXPosition);
            nextYPosition = CheckYInsideScreen(nextYPosition);

            moveRectTransform.position = new Vector3(
                float.IsNaN(nextXPosition) ? CheckXInsideScreen(moveRectTransform.position.x - _directionMove.x) : nextXPosition,
                float.IsNaN(nextYPosition) ? CheckYInsideScreen(moveRectTransform.position.y - _directionMove.y) : nextYPosition,
                moveRectTransform.position.z);
            onDrag?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.Invoke();
        }

        private Vector2 Center(RectTransform rectTransform)
        {
            return new Vector2(rectTransform.position.x + rectTransform.rect.width * (0.5f - rectTransform.pivot.x),
                rectTransform.position.y + rectTransform.rect.height * (0.5f - rectTransform.pivot.y));
        }

        private Vector2 HalfSize(RectTransform rectTransform)
        {
            return new Vector2(rectTransform.rect.width / 2f, rectTransform.rect.height / 2f);
        }
        
        private float CheckXInsideScreen(float posX)
        {
            var minX = _getHalfSize.x - offsetLeft;
            var maxX = Screen.width - _getHalfSize.x - offsetRight;
            
            return Mathf.Clamp(posX, minX, maxX);
        }
        
        private float CheckYInsideScreen(float posY)
        {
            var minY = _getHalfSize.y - offsetBottom;
            var maxY = Screen.height - _getHalfSize.y - offsetTop;
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
            var leftTop = _centerLeft + new Vector2(0, _getHalfSize.y);
            var leftDown = _centerLeft - new Vector2(0, _getHalfSize.y);

            return IsInsideObstacle(leftTop, obstacle)
                   || IsInsideObstacle(_centerLeft, obstacle)
                   || IsInsideObstacle(leftDown, obstacle);
        }

        private bool RightCollision(RectTransform obstacle)
        {
            var rightTop = _centerRight + new Vector2(0, _getHalfSize.y);
            var rightDown = _centerRight - new Vector2(0, _getHalfSize.y);

            return IsInsideObstacle(rightTop, obstacle)
                   || IsInsideObstacle(_centerRight, obstacle)
                   || IsInsideObstacle(rightDown, obstacle);
        }

        private bool TopCollision(RectTransform obstacle)
        {
            var topLeft = _centerTop - new Vector2(_getHalfSize.x, 0);
            var topRight = _centerTop + new Vector2(_getHalfSize.x, 0);

            return IsInsideObstacle(topLeft, obstacle)
                   || IsInsideObstacle(_centerTop, obstacle)
                   || IsInsideObstacle(topRight, obstacle);
        }

        private bool BottomCollision(RectTransform obstacle)
        {
            var bottomLeft = _centerBottom - new Vector2(_getHalfSize.x, 0);
            var bottomRight = _centerBottom + new Vector2(_getHalfSize.x, 0);

            return IsInsideObstacle(bottomLeft, obstacle)
                   || IsInsideObstacle(_centerBottom, obstacle)
                   || IsInsideObstacle(bottomRight, obstacle);
        }

        private void RefreshOffsetValue()
        {
            if (!isStaticOffsetTop)
                offsetTop = offsetRectTransform.offsetMax.y;
            if (!isStaticOffsetBottom)
                offsetBottom = offsetRectTransform.offsetMin.y;
            if (!isStaticOffsetLeft)
                offsetLeft = offsetRectTransform.offsetMin.x;
            if (!isStaticOffsetRight)
                offsetRight = offsetRectTransform.offsetMax.x;
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

#if UNITY_EDITOR
[CustomEditor(typeof(DragUI))]
public class DragPanelEditor : Editor
{
    private SerializedProperty _moveRect;
    private SerializedProperty _moveRectTransform;
    private SerializedProperty _obstacles;
    private SerializedProperty _onBeginDrag;
    private SerializedProperty _onDrag;
    private SerializedProperty _onEndDrag;
    private SerializedProperty _offsetRectTransform;
    private SerializedProperty _offsetTop;
    private SerializedProperty _offsetBottom;
    private SerializedProperty _offsetLeft;
    private SerializedProperty _offsetRight;
    private SerializedProperty _isStaticOffsetTop;
    private SerializedProperty _isStaticOffsetBottom;
    private SerializedProperty _isStaticOffsetLeft;
    private SerializedProperty _isStaticOffsetRight;

    private bool _isFoldoutOffset;

    private void OnEnable()
    {
        _moveRect = serializedObject.FindProperty("moveRect");
        _moveRectTransform = serializedObject.FindProperty("moveRectTransform");
        _obstacles = serializedObject.FindProperty("obstacles");
        _onBeginDrag = serializedObject.FindProperty("onBeginDrag");
        _onDrag = serializedObject.FindProperty("onDrag");
        _onEndDrag = serializedObject.FindProperty("onEndDrag");
        _offsetRectTransform = serializedObject.FindProperty("offsetRectTransform");
        _offsetTop = serializedObject.FindProperty("offsetTop");
        _offsetBottom = serializedObject.FindProperty("offsetBottom");
        _offsetLeft = serializedObject.FindProperty("offsetLeft");
        _offsetRight = serializedObject.FindProperty("offsetRight");
        _isStaticOffsetTop = serializedObject.FindProperty("isStaticOffsetTop");
        _isStaticOffsetBottom = serializedObject.FindProperty("isStaticOffsetBottom");
        _isStaticOffsetLeft = serializedObject.FindProperty("isStaticOffsetLeft");
        _isStaticOffsetRight = serializedObject.FindProperty("isStaticOffsetRight");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(_moveRect);
        if (_moveRect.enumValueIndex == 2)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_moveRectTransform);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(_obstacles);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_onBeginDrag);
        EditorGUILayout.PropertyField(_onDrag);
        EditorGUILayout.PropertyField(_onEndDrag);

        _isFoldoutOffset = EditorGUILayout.Foldout(_isFoldoutOffset, "Set offset (Top/Bottom/Left/Right)", true);
        if (_isFoldoutOffset)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(_offsetRectTransform);
            if (_offsetRectTransform.objectReferenceValue != null)
            {
                var offsetRectTransform = ((RectTransform)_offsetRectTransform.objectReferenceValue);
                EditorGUI.indentLevel++;
                SetOffsetValue(_offsetTop, _isStaticOffsetTop, offsetRectTransform.offsetMax.y);
                SetOffsetValue(_offsetBottom, _isStaticOffsetBottom, offsetRectTransform.offsetMin.y);
                SetOffsetValue(_offsetLeft, _isStaticOffsetLeft, offsetRectTransform.offsetMin.x);
                SetOffsetValue(_offsetRight, _isStaticOffsetRight, offsetRectTransform.offsetMax.x);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void SetOffsetValue(SerializedProperty offsetValue, SerializedProperty isStaticValue, float value)
    {
        EditorGUILayout.BeginHorizontal();

        GUI.enabled = isStaticValue.boolValue;
        if (!isStaticValue.boolValue)
        {
            offsetValue.floatValue = value;
        }

        EditorGUILayout.PropertyField(offsetValue);
        GUI.enabled = true;
        isStaticValue.boolValue = EditorGUILayout.Toggle("IsStaticValue", isStaticValue.boolValue);
        EditorGUILayout.EndHorizontal();
    }
}
#endif
}
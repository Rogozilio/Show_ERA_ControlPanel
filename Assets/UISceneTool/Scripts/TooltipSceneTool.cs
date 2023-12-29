using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ERA
{
    public class TooltipSceneTool : MonoBehaviour
    {
        public Vector2 padding;

        private RectTransform _rectTransform;
        private TextMeshProUGUI _textMeshPro;
        private Image _image;
        private Color32 _colorImageVisible;
        private Color32 _colorImageInvisible;
        private Color32 _colorTextVisible;
        private Color32 _colorTextInvisible;
        private Coroutine _showTooltip;
        private Coroutine _hideTooltip;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
            _image = GetComponent<Image>();

            _colorImageInvisible = _image.color;
            _colorTextInvisible = _textMeshPro.color;
            _colorImageVisible = _colorImageInvisible;
            _colorTextVisible = _colorTextInvisible;
            _colorImageVisible.a = 255;
            _colorTextVisible.a = 255;
        }

        private void Update()
        {
            transform.position = Input.mousePosition;
            var textSize = _textMeshPro.GetRenderedValues(false);
            _rectTransform.sizeDelta = textSize + padding;
        }

        private void ImmediatelyHideTooltip()
        {
            _image.color = _colorImageInvisible;
            _textMeshPro.color = _colorTextInvisible;
        }

        private IEnumerator SmoothChangeColor(bool isShowTooltip = true)
        {
            float step;
            float deltaLerp;
            int finishAlpha;
            if (isShowTooltip)
            {
                ImmediatelyHideTooltip();
                yield return new WaitForSeconds(1f);
                step = 0.04f;
                deltaLerp = 0f;
                finishAlpha = 255;
            }
            else
            {
                step = -0.04f;
                deltaLerp = 1f;
                finishAlpha = 0;
            }

            Color32 colorImage = _image.color;
            Color32 colorText = _textMeshPro.color;
            while (colorImage.a != finishAlpha && colorText.a != finishAlpha)
            {
                colorImage = _image.color;
                colorText = _textMeshPro.color;
                deltaLerp = Mathf.Clamp(deltaLerp + step, 0f, 1f);
                _image.color = Color32.Lerp(_colorImageInvisible, _colorImageVisible, deltaLerp);
                _textMeshPro.color = Color32.Lerp(_colorTextInvisible, _colorTextVisible, deltaLerp);
                yield return null;
            }
        }

        public void Show(string text)
        {
            _textMeshPro.text = text;
            if (_textMeshPro.text == string.Empty) return;
            if (_hideTooltip != null) StopCoroutine(_hideTooltip);
            _showTooltip = StartCoroutine(SmoothChangeColor());
        }

        public void Hide()
        {
            if (_textMeshPro.text == string.Empty) return;
            if (_showTooltip != null) StopCoroutine(_showTooltip);
            _hideTooltip = StartCoroutine(SmoothChangeColor(false));
        }
    }
}
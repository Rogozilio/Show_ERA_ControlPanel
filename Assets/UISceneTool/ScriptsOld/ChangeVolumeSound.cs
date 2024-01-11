using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace ERA
{
    public class ChangeVolumeSound : MonoBehaviour
    {
        public AudioMixerGroup audioMixerMaster;
        public List<string> nameMixers;
        public List<int> indexesMixer;
        public int maskMixer = 0;

        private ButtonClickSceneTool _buttonClick;
        private AudioMixerGroup[] _audioMixers;

        private Image _leftBar;
        private Image _rightBar;

        private float _delta = 0.1f;
        private float _scrollValue = 1f;

        private bool _isCanScroll => _buttonClick.clickButtonState == ButtonClickSceneTool.ClickButtonState.Activate &&
                                     _buttonClick.IsMouseHover;

        private void Awake()
        {
            _buttonClick = GetComponent<ButtonClickSceneTool>();
            _leftBar = GetComponentsInChildren<Image>()[1];
            _rightBar = GetComponentsInChildren<Image>()[2];

            nameMixers = new List<string>();

            if (!audioMixerMaster) return;

            _audioMixers = audioMixerMaster.audioMixer.FindMatchingGroups("");

            foreach (var mixer in _audioMixers)
            {
                nameMixers.Add(mixer.name);
            }
        }

        private void Start()
        {
            _buttonClick.AddOnActivate = ActivateVolumeBar;
            _buttonClick.AddOnDeactivate = DeactivateVolumeBar;

            if (!audioMixerMaster || _buttonClick.isDisabled)
                _buttonClick.Deactivate();
        }

        private void Update()
        {
            ChangeScrollValue();
        }

        private void ChangeScrollValue()
        {
            if (!_isCanScroll) return;

            var scrollStep = Input.mouseScrollDelta.y * _delta;
            _scrollValue = Mathf.Clamp(_scrollValue + scrollStep, 0f, 1f);
            _leftBar.fillAmount = _scrollValue + (0.1f - 0.1f * _scrollValue);
            _rightBar.fillAmount = _scrollValue + (0.1f - 0.1f * _scrollValue);
            ChangeVolumeMixers(_scrollValue);
        }

        private void ChangeVolumeMixers(float value)
        {
            if (_audioMixers == null) return;

            foreach (var index in indexesMixer)
            {
                var hasMixer = _audioMixers[index].audioMixer
                    .SetFloat(nameMixers[index], Mathf.Log(Mathf.Clamp(value, 0.001f, 1f)) * 20);

                if (!hasMixer)
                    Debug.LogError("Parameter " + nameMixers[index] +
                                   " not found. Please, add parameter in Audio Mixed for " + nameMixers[index] +
                                   " group and set name " + nameMixers[index]);
            }
        }

        public void SwitchVolumeBar()
        {
            if (_buttonClick.clickButtonState == ButtonClickSceneTool.ClickButtonState.Activate)
            {
                ActivateVolumeBar();
            }
            else
            {
                DeactivateVolumeBar();
            }
        }

        private void ActivateVolumeBar()
        {
            _leftBar.gameObject.SetActive(true);
            _rightBar.gameObject.SetActive(true);
            ChangeVolumeMixers(_scrollValue);
        }

        private void DeactivateVolumeBar()
        {
            _leftBar.gameObject.SetActive(false);
            _rightBar.gameObject.SetActive(false);
            ChangeVolumeMixers(0f);
        }
    }

#if(UNITY_EDITOR)
    [CustomEditor(typeof(ChangeVolumeSound))]
    public class ChangeVolumeSoundEditor : Editor
    {
        private ChangeVolumeSound _main;

        private SerializedProperty _audioMixerMaster;
        private SerializedProperty _indexesMixer;
        private SerializedProperty _maskMixer;

        private void OnEnable()
        {
            _main = (ChangeVolumeSound)target;

            _audioMixerMaster = serializedObject.FindProperty("audioMixerMaster");
            _indexesMixer = serializedObject.FindProperty("indexesMixer");
            _maskMixer = serializedObject.FindProperty("maskMixer");

            _main.nameMixers = new List<string>();

            if (!_main.audioMixerMaster) return;

            foreach (var mixer in _main.audioMixerMaster.audioMixer.FindMatchingGroups(""))
            {
                _main.nameMixers.Add(mixer.name);
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_audioMixerMaster);
            if (!_audioMixerMaster.objectReferenceValue) return;
            _maskMixer.intValue =
                EditorGUILayout.MaskField("Audio Mixer", _maskMixer.intValue, _main.nameMixers.ToArray());
            var indexes = GetIndexes();
            _indexesMixer.arraySize = indexes.Count;

            for (var i = 0; i < indexes.Count; i++)
            {
                _indexesMixer.GetArrayElementAtIndex(i).intValue = indexes[i];
            }

            serializedObject.ApplyModifiedProperties();
        }

        private List<int> GetIndexes()
        {
            var indexes = new List<int>();
            for (var i = 0; i < _main.nameMixers.Count; i++)
            {
                if (((1 << i) & _maskMixer.intValue) != 0) indexes.Add(i);
            }

            return indexes;
        }
    }
#endif
}
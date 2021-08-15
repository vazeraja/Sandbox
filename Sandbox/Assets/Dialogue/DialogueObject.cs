using System;
using CharTweener.Assets.CharTween.Scripts;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Dialogue {
    [CreateAssetMenu(fileName = "NewDialogueObject", menuName = "Text Animator/Dialogue Object")]
    public class DialogueObject : ScriptableObject {
        [TextArea(10, 5)]
        public string text;
        public TextAnimationGraph textAnimationGraph;

        public void Animate() {
            var tmpText = textAnimationGraph.m_TMPText;
            tmpText.text = text;
            var start = 0;
            var end = text.Length-1;
            textAnimationGraph.Animate(start, end);
            Debug.Log(start + " " + end);
        }
    }

    [Serializable]
    public class TextAnimationGraph {
        public TMP_Text m_TMPText;
        private CharTweener.Assets.CharTween.Scripts.CharTweener m_Tweener;

        public TextAnimationGraph(TMP_Text tmpText) {
            m_TMPText = tmpText;
            // m_Tweener = m_TMPText.GetCharTweener();
        }

        public void Animate(int start, int end) {
            m_Tweener = m_TMPText.GetCharTweener();
            for (var i = start; i <= end; ++i) {
                var timeOffset = Mathf.Lerp(0, 1, (i - start) / (float)(end - start + 1));
                var rotationTween = m_Tweener
                    .DOLocalRotate(i, UnityEngine.Random.onUnitSphere * 360, 2, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental);
                rotationTween.fullPosition = timeOffset;
            }
        }
    }
}
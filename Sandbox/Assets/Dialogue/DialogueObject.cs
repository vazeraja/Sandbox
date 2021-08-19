using System;
using System.Threading;
using CharTweener.Assets.CharTween.Scripts;
using TMPro;
using UnityEngine;

namespace Dialogue {
    [CreateAssetMenu(fileName = "NewDialogueObject", menuName = "Text Animator/Dialogue Object")]
    public class DialogueObject : ScriptableObject {
        [TextArea(10, 5)] public string text;
        public TextAnimationGraph textAnimationGraph;

        public void Animate(int start, int end) {
            if (textAnimationGraph == null) {
                Debug.LogError("Text Animation Graph is null");
                return;
            }

            textAnimationGraph.text = text;
            textAnimationGraph.Animate(start, end);
        }
    }

    [Serializable]
    public class TextAnimationGraph {
        private TMP_Text m_TMPText;
        private Action<CharTweener.Assets.CharTween.Scripts.CharTweener, int, int> m_AnimationMethod;
        private CharTweener.Assets.CharTween.Scripts.CharTweener m_Tweener;

        public string text {
            set { m_TMPText.text = value; }
        }

        public TextAnimationGraph(TMP_Text tmpText,
            Action<CharTweener.Assets.CharTween.Scripts.CharTweener, int, int> animationMethod) {
            m_TMPText = tmpText;
            m_AnimationMethod = animationMethod;
            m_Tweener = m_TMPText.GetCharTweener();
        }

        public void Animate(int start, int end) {
            m_AnimationMethod(m_Tweener, start, end);
        }
    }
}
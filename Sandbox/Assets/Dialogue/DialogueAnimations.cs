using DG.Tweening;
using UnityEngine;

namespace Dialogue {
    public static class DialogueAnimations {
        public static void MatrixTextAnimation(CharTweener.Assets.CharTween.Scripts.CharTweener tweener, int start,
            int end) {
            for (var i = start; i <= end; ++i) {
                var timeOffset = Mathf.Lerp(0, 1, (i - start) / (float)(end - start + 1));
                var rotationTween = tweener
                    .DOLocalRotate(i, UnityEngine.Random.onUnitSphere * 360, 2, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental);
                rotationTween.fullPosition = timeOffset;
            }
        }
    }
}
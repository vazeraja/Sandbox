using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CharTweener.Assets.CharTween.Scripts
{
    public partial class CharTweener : MonoBehaviour
    {
        // Extra goodies
        public Tweener DOCircle(int charIndex, float radius, float duration, int pathPoints = 8, PathType pathType = PathType.CatmullRom,
            PathMode pathMode = PathMode.Full3D, int resolution = 10, Color? gizmoColor = null)
        {
            var tweenPath = new Vector3[pathPoints];
            for (var i = 0; i < tweenPath.Length; ++i)
            {
                var theta = Mathf.Lerp(0, 2 * Mathf.PI, i / (float)(tweenPath.Length - 1));
                tweenPath[i] = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0);
            }

            //The first point of the path is the transform itself
            GetProxyTransform(charIndex).localPosition = tweenPath[0];

            SetPositionOffset(charIndex, tweenPath[0]);
            return DOLocalPath(charIndex, tweenPath, duration, pathType, pathMode, resolution, gizmoColor, true);
        }

        // Color tweens
        public Tweener DOFade(int charIndex, float endValue, float duration)
        {
            return MonitorColorTween(GetProxyColor(charIndex).DOFade(endValue, duration));
        }

        public Tweener DOColor(int charIndex, Color endValue, float duration)
        {
            return MonitorColorTween(GetProxyColor(charIndex).DOColor(endValue, duration));
        }

        public Tweener DOGradient(int charIndex, VertexGradient endValue, float duration)
        {
            return MonitorColorTween(GetProxyColor(charIndex).DOVertexGradient(endValue, duration));
        }

        // Transform tweens
        public Tweener DOMove(int charIndex, Vector3 endValue, float duration, bool snapping = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOMove(GetProxyTransform(charIndex), endValue, duration, snapping));
        }

        public Tweener DOMoveX(int charIndex, float endValue, float duration, bool snapping = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOMoveX(GetProxyTransform(charIndex), endValue, duration, snapping));
        }

        public Tweener DOMoveY(int charIndex, float endValue, float duration, bool snapping = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOMoveY(GetProxyTransform(charIndex), endValue, duration, snapping));
        }

        public Tweener DOMoveZ(int charIndex, float endValue, float duration, bool snapping = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOMoveZ(GetProxyTransform(charIndex), endValue, duration, snapping));
        }

        public Tweener DOLocalMove(int charIndex, Vector3 endValue, float duration, bool snapping = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOLocalMove(GetProxyTransform(charIndex), endValue, duration, snapping));
        }

        public Tweener DOLocalMoveX(int charIndex, float endValue, float duration, bool snapping = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOLocalMoveX(GetProxyTransform(charIndex), endValue, duration, snapping));
        }

        public Tweener DOLocalMoveY(int charIndex, float endValue, float duration, bool snapping = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOLocalMoveY(GetProxyTransform(charIndex), endValue, duration, snapping));
        }

        public Tweener DOLocalMoveZ(int charIndex, float endValue, float duration, bool snapping = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOLocalMoveZ(GetProxyTransform(charIndex), endValue, duration, snapping));
        }

        public Sequence DOJump(int charIndex, Vector3 endValue, float jumpPower, int numJumps, float duration, bool snapping = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOJump(GetProxyTransform(charIndex), endValue, jumpPower, numJumps, duration, snapping));
        }

        public Tweener DORotate(int charIndex, Vector3 endValue, float duration, RotateMode mode)
        {
            return MonitorTransformTween(ShortcutExtensions.DORotate(GetProxyTransform(charIndex), endValue, duration, mode));
        }

        public Tweener DORotateQuaternion(int charIndex, Quaternion endValue, float duration)
        {
            return MonitorTransformTween(ShortcutExtensions.DORotateQuaternion(GetProxyTransform(charIndex), endValue, duration));
        }

        public Tweener DOLocalRotate(int charIndex, Vector3 endValue, float duration, RotateMode mode = RotateMode.Fast)
        {
            return MonitorTransformTween(ShortcutExtensions.DOLocalRotate(GetProxyTransform(charIndex), endValue, duration, mode));
        }

        public Tweener DOLocalRotateQuaternion(int charIndex, Quaternion endValue, float duration)
        {
            return MonitorTransformTween(ShortcutExtensions.DOLocalRotateQuaternion(GetProxyTransform(charIndex), endValue, duration));
        }

        public Tweener DOLookAt(int charIndex, Vector3 towards, float duration, AxisConstraint axisConstraint = AxisConstraint.None,
            Vector3? up = null)
        {
            return MonitorTransformTween(ShortcutExtensions.DOLookAt(GetProxyTransform(charIndex), towards, duration, axisConstraint, up));
        }

        public Tweener DOScale(int charIndex, float endValue, float duration)
        {
            return MonitorTransformTween(ShortcutExtensions.DOScale(GetProxyTransform(charIndex), endValue, duration));
        }

        public Tweener DOScale(int charIndex, Vector3 endValue, float duration)
        {
            return MonitorTransformTween(ShortcutExtensions.DOScale(GetProxyTransform(charIndex), endValue, duration));
        }

        public Tweener DOScaleX(int charIndex, float endValue, float duration)
        {
            return MonitorTransformTween(ShortcutExtensions.DOScaleX(GetProxyTransform(charIndex), endValue, duration));
        }

        public Tweener DOScaleY(int charIndex, float endValue, float duration)
        {
            return MonitorTransformTween(ShortcutExtensions.DOScaleY(GetProxyTransform(charIndex), endValue, duration));
        }

        public Tweener DOScaleZ(int charIndex, float endValue, float duration)
        {
            return MonitorTransformTween(ShortcutExtensions.DOScaleZ(GetProxyTransform(charIndex), endValue, duration));
        }

        public Tweener DOPunchPosition(int charIndex, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1, bool snapping = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOPunchPosition(GetProxyTransform(charIndex), punch, duration, vibrato, elasticity, snapping));
        }

        public Tweener DOPunchRotation(int charIndex, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            return MonitorTransformTween(ShortcutExtensions.DOPunchRotation(GetProxyTransform(charIndex), punch, duration, vibrato, elasticity));
        }

        public Tweener DOPunchScale(int charIndex, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            return MonitorTransformTween(ShortcutExtensions.DOPunchScale(GetProxyTransform(charIndex), punch, duration, vibrato, elasticity));
        }

        public Tweener DOShakePosition(int charIndex, float duration, float strength = 1, int vibrato = 10, float randomness = 90,
            bool snapping = false, bool fadeOut = true)
        {
            return MonitorTransformTween(ShortcutExtensions.DOShakePosition(GetProxyTransform(charIndex), duration, strength, vibrato, randomness, snapping, fadeOut));
        }

        public Tweener DOShakePosition(int charIndex, float duration, Vector3 strength, int vibrato = 10, float randomness = 90,
            bool snapping = false, bool fadeOut = true)
        {
            return MonitorTransformTween(ShortcutExtensions.DOShakePosition(GetProxyTransform(charIndex), duration, strength, vibrato, randomness, snapping, fadeOut));
        }

        public Tweener DOShakeRotation(int charIndex, float duration, float strength = 1, int vibrato = 10, float randomness = 90,
            bool fadeOut = true)
        {
            return MonitorTransformTween(ShortcutExtensions.DOShakeRotation((Transform)GetProxyTransform(charIndex), duration, strength, vibrato, randomness, fadeOut));
        }

        public Tweener DOShakeRotation(int charIndex, float duration, Vector3 strength, int vibrato = 10, float randomness = 90,
            bool fadeOut = true)
        {
            return MonitorTransformTween(ShortcutExtensions.DOShakeRotation((Transform)GetProxyTransform(charIndex), duration, strength, vibrato, randomness, fadeOut));
        }
        public Tweener DOShakeScale(int charIndex, float duration, float strength = 1, int vibrato = 10, float randomness = 90,
            bool fadeOut = true)
        {
            return MonitorTransformTween(ShortcutExtensions.DOShakeScale(GetProxyTransform(charIndex), duration, strength, vibrato, randomness, fadeOut));
        }

        public Tweener DOShakeScale(int charIndex, float duration, Vector3 strength, int vibrato = 10, float randomness = 90,
            bool fadeOut = true)
        {
            return MonitorTransformTween(ShortcutExtensions.DOShakeScale(GetProxyTransform(charIndex), duration, strength, vibrato, randomness, fadeOut));
        }

        public Tweener DOPath(int charIndex, Vector3[] path, float duration, PathType pathType = PathType.Linear,
            PathMode pathMode = PathMode.Sidescroller2D, int resolution = 10, Color? gizmoColor = null, bool closePath = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOPath(GetProxyTransform(charIndex), path, duration, pathType, pathMode, resolution, gizmoColor).SetOptions(closePath));
        }

        public Tweener DOLocalPath(int charIndex, Vector3[] path, float duration, PathType pathType = PathType.Linear,
            PathMode pathMode = PathMode.Full3D, int resolution = 10, Color? gizmoColor = null, bool closePath = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOLocalPath(GetProxyTransform(charIndex), path, duration, pathType, pathMode, resolution, gizmoColor).SetOptions(closePath));
        }

        public Tweener DOBlendableMoveBy(int charIndex, Vector3 byValue, float duration, bool snapping = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOBlendableMoveBy(GetProxyTransform(charIndex), byValue, duration, snapping));
        }

        public Tweener DOBlendableLocalMoveBy(int charIndex, Vector3 byValue, float duration, bool snapping = false)
        {
            return MonitorTransformTween(ShortcutExtensions.DOBlendableLocalMoveBy(GetProxyTransform(charIndex), byValue, duration, snapping));
        }

        public Tweener DOBlendableRotateBy(int charIndex, Vector3 byValue, float duration,
            RotateMode rotateMode = RotateMode.Fast)
        {
            return MonitorTransformTween(ShortcutExtensions.DOBlendableRotateBy(GetProxyTransform(charIndex), byValue, duration, rotateMode));
        }

        public Tweener DOBlendableLocalRotateBy(int charIndex, Vector3 byValue, float duration,
            RotateMode rotateMode = RotateMode.Fast)
        {
            return MonitorTransformTween(ShortcutExtensions.DOBlendableLocalRotateBy(GetProxyTransform(charIndex), byValue, duration, rotateMode));
        }

        public Tweener DOBlendableScaleBy(int charIndex, Vector3 byValue, float duration)
        {
            return MonitorTransformTween(ShortcutExtensions.DOBlendableScaleBy(GetProxyTransform(charIndex), byValue, duration));
        }
    }
}

using System.Collections.Generic;
using TMPro;

namespace CharTweener.Assets.CharTween.Scripts
{
    public static class CharTweenerUtility
    {
        private static readonly Dictionary<TMP_Text, CharTweener> CharModifiers = new Dictionary<TMP_Text, CharTweener>();

        /// <summary>
        /// Returns a <see cref="CharTweener.Assets.CharTween.Scripts.CharTweener"/> guaranteeing the same instance is used for the same text.
        /// </summary>
        public static CharTweener GetCharTweener(this TMP_Text text)
        {
            if (CharModifiers.ContainsKey(text))
                return CharModifiers[text];

            var modifier = CharModifiers[text] = text.gameObject.AddComponent<CharTweener>();
            modifier.Text = text;
            modifier.Initialize();
            return modifier;
        }
    }
}

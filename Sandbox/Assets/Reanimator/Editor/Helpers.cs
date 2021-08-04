using System;
using System.Collections.Generic;
using System.Linq;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor
{
    internal static class Helpers
    {
        /// <summary>
        /// Shamelessly stolen from <a href="https://forum.unity.com/threads/drawing-a-sprite-in-editor-window.419199/#post-3059891">Woofy</a>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="sprite"></param>
        internal static void DrawTexturePreview(Rect position, Sprite sprite)
        {
            var fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
            var size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

            var coords = sprite.textureRect;
            coords.x /= fullSize.x;
            coords.width /= fullSize.x;
            coords.y /= fullSize.y;
            coords.height /= fullSize.y;

            Vector2 ratio;
            ratio.x = position.width / size.x;
            ratio.y = position.height / size.y;
            float minRatio = Mathf.Min(ratio.x, ratio.y);

            var center = position.center;
            position.width = size.x * minRatio;
            position.height = size.y * minRatio;
            position.center = center;

            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }

        internal static void SetDirty(IEnumerable<UnityEngine.Object> targets) => targets.ForEach(EditorUtility.SetDirty);

        private static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        internal static List<T> LoadAssetsOfType<T>() where T : UnityEngine.Object {
            //List<string> assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}").ToList();
            return AssetDatabase.FindAssets($"t:{typeof(T).Name}").ToList().Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<T>).ToList();
        }
        internal static List<ReanimatorNode> GetChildren(ReanimatorNode parent)
        {
            List<ReanimatorNode> children = new List<ReanimatorNode>();
            
            switch (parent) {
                case BaseNode rootNode when rootNode.root != null:
                    children.Add(rootNode.root);
                    break;
                case OverrideNode overrideNode when overrideNode.next != null:
                    children.Add(overrideNode.next);
                    break;
                case SwitchNode switchNode:
                    return switchNode.nodes;
            }

            return children;
        }

        internal static void Traverse(ReanimatorNode node, Action<ReanimatorNode> visitor)
        {
            if (!node) return;
            visitor.Invoke(node);
            var children = GetChildren(node);
            children.ForEach(n => Traverse(n, visitor));
        }
    }
}
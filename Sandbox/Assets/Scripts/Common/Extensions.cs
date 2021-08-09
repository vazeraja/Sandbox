using System;
using System.Collections.Generic;

namespace TN.Common {
    public static class Extensions {
        /// <summary>
        /// Allows one line ForEach statement on Arrays and other IEnumerables
        /// </summary>
        /// <example>
        /// This sample shows how to call the method.
        /// <code>
        /// int[] array = new int[] {1, 2, 3};
        /// array.ForEach(x => Debug.Log(x));
        /// </code>
        /// </example>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}
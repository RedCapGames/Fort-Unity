using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Fort.GamePlay
{
    public static class TransformExtensions
    {
        #region  Public Methods

        public static T FindComponentRecursive<T>(this Transform transform)
        {
            T component = transform.GetComponent<T>();
            if (component != null)
                return component;

            for (int i = 0; i < transform.childCount; i++)
            {
                T result = FindComponentRecursive<T>(transform.GetChild(i));
                if (result != null)
                    return result;
            }
            return default(T);
        }

        public static Transform FindChildRecursive(this Transform transform, string name)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (string.Equals(transform.GetChild(i).name, name, StringComparison.OrdinalIgnoreCase))
                    return transform.GetChild(i);
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform childTransform = FindChildRecursive(transform.GetChild(i), name);
                if (childTransform != null)
                    return childTransform;
            }

            return null;
        }

        public static Transform FindChildStartWithRecursive(this Transform transform, string name)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
                    return transform.GetChild(i);
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform childTransform = FindChildStartWithRecursive(transform.GetChild(i), name);
                if (childTransform != null)
                    return childTransform;
            }

            return null;
        }

        public static Transform FindChildRecursive(this Transform transform, Regex pattern)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (pattern.IsMatch(transform.GetChild(i).name))
                    return transform.GetChild(i);
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform childTransform = FindChildRecursive(transform.GetChild(i), pattern);
                if (childTransform != null)
                    return childTransform;
            }

            return null;
        }

        public static Transform[] FindChildsRecursive(this Transform transform, Regex pattern)
        {
            List<Transform> result = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (pattern.IsMatch(transform.GetChild(i).name))
                    result.Add(transform.GetChild(i));
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform[] childTransforms = FindChildsRecursive(transform.GetChild(i), pattern);
                if (childTransforms.Any())
                    result.AddRange(childTransforms);
            }
            return result.ToArray();
        }

        public static IEnumerable<Transform> Children(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                yield return transform.GetChild(i);
            }
        }

        #endregion
    }
}

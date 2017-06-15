using System;
using UnityEditor;

namespace Fort.Inspector
{
    class NumberPresentation : Presentation
    {
        #region Overrides of PresentationFieldInfo
        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            if (parameter.DataType == typeof (byte))
            {
                byte result = (byte)EditorGUILayout.IntField(parameter.Title, (byte)parameter.Instance);
                return new PresentationResult
                {
                    Result = result,
                    Change = new Change { IsDataChanged = result != (byte)parameter.Instance }
                };
            }
            if (parameter.DataType == typeof(sbyte))
            {
                sbyte result = (sbyte)EditorGUILayout.IntField(parameter.Title, (sbyte)parameter.Instance);
                return new PresentationResult
                {
                    Result = result,
                    Change = new Change { IsDataChanged = result != (sbyte)parameter.Instance }                    
                };

            }
            if (parameter.DataType == typeof(short))
            {
                short result = (short)EditorGUILayout.IntField(parameter.Title, (short)parameter.Instance);
                return new PresentationResult
                {
                    Result = result,
                    Change = new Change { IsDataChanged = result != (short)parameter.Instance }
                };

            }
            if (parameter.DataType == typeof(ushort))
            {
                ushort result = (ushort)EditorGUILayout.IntField(parameter.Title, (ushort)parameter.Instance);
                return new PresentationResult
                {
                    Result = result,
                    Change = new Change { IsDataChanged = result != (ushort)parameter.Instance }
                };

            }
            if (parameter.DataType == typeof(int))
            {
                int result = EditorGUILayout.IntField(parameter.Title, (int)parameter.Instance);
                return new PresentationResult
                {
                    Result = result,
                    Change = new Change { IsDataChanged = result != (int)parameter.Instance }                    
                };

            }
            if (parameter.DataType == typeof(uint))
            {
                uint result = (uint)EditorGUILayout.IntField(parameter.Title, (int)(uint)parameter.Instance);
                return new PresentationResult
                {
                    Result = result,
                    Change = new Change { IsDataChanged = result != (uint)parameter.Instance }                    
                };

            }
            if (parameter.DataType == typeof(long))
            {
                long result = EditorGUILayout.LongField(parameter.Title, (long)parameter.Instance);
                return new PresentationResult
                {
                    Result = result,
                    Change = new Change { IsDataChanged = result != (long)parameter.Instance }
                };

            }
            if (parameter.DataType == typeof(ulong))
            {
                ulong result = (ulong)EditorGUILayout.LongField(parameter.Title, (long)(ulong)parameter.Instance);
                return new PresentationResult
                {
                    Result = result,
                    Change = new Change { IsDataChanged = result != (ulong)parameter.Instance }
                };

            }
            if (parameter.DataType == typeof(float))
            {
                float result = EditorGUILayout.FloatField(parameter.Title, (float)parameter.Instance);
                return new PresentationResult
                {
                    Result = result,
                    Change = new Change { IsDataChanged = result != (float)parameter.Instance }
                };

            }
            if (parameter.DataType == typeof(double))
            {
                double result = EditorGUILayout.DoubleField(parameter.Title, (double)parameter.Instance);
                return new PresentationResult
                {
                    Result = result,
                    Change = new Change { IsDataChanged = result != (double)parameter.Instance }
                };

            }
            if (parameter.DataType == typeof(decimal))
            {
                decimal result = (decimal)EditorGUILayout.DoubleField(parameter.Title, (double)(decimal)parameter.Instance);
                return new PresentationResult
                {
                    Result = result,
                    Change = new Change { IsDataChanged = result != (decimal)parameter.Instance }
                };

            }
            throw new Exception("Invalid numeric type");

        }

        #endregion
    }
}
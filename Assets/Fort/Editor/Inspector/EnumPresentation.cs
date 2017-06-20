using System;
using System.Linq;
using UnityEditor;

namespace Fort.Inspector
{
    class EnumPresentation : Presentation
    {

        #region Overrides of PresentationFieldInfo

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            string[] presentationItems = Enum.GetValues(parameter.DataType).Cast<Enum>().Select(val =>
            {
                EnumPresentationAttribute attribute = val.GetAttribute<EnumPresentationAttribute>();
                return attribute != null ? attribute.DisplayName : val.ToString();
            }).ToArray();

            object result = Enum.GetValues(parameter.DataType)
                .GetValue(EditorGUILayout.Popup(parameter.Title,
                    Enum.GetValues(parameter.DataType).Cast<Enum>().ToList().IndexOf((Enum)parameter.Instance),
                    presentationItems));
            return new PresentationResult
            {
                Result = result,
                Change = new Change { IsDataChanged = !Equals((Enum)result, (Enum)parameter.Instance) }                
            };
        }

        #endregion
    }

    internal static class EnumExtensions
    {

        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            if (attributes.Length == 0)
                return null;
            return (T)attributes[0];
        }
    }
}
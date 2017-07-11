using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Fort.Inspector
{
    public class CurvePresentation:Presentation
    {
        #region Overrides of Presentation

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            AnimationCurve animationCurve = parameter.Instance as AnimationCurve?? new AnimationCurve();
            EditorGUI.BeginChangeCheck();
            AnimationCurve newAnimationCurve = EditorGUILayout.CurveField(parameter.Title,animationCurve);

            Change change = new Change();
            change.IsDataChanged = EditorGUI.EndChangeCheck();
/*            if (newAnimationCurve.length != animationCurve.length||newAnimationCurve.postWrapMode != animationCurve.postWrapMode || newAnimationCurve.preWrapMode != animationCurve.preWrapMode)
                change.IsDataChanged = true;
            else
            {
                for (int i = 0; i < newAnimationCurve.length; i++)
                {
                    Keyframe keyFrame = animationCurve.keys[i];
                    Keyframe newKeyFrame = newAnimationCurve.keys[i];
                    change.IsDataChanged |= keyFrame.value != newKeyFrame.value;
                    change.IsDataChanged |= keyFrame.inTangent != newKeyFrame.inTangent;
                    change.IsDataChanged |= keyFrame.outTangent != newKeyFrame.outTangent;
                    change.IsDataChanged |= keyFrame.tangentMode != newKeyFrame.tangentMode;
                    change.IsDataChanged |= keyFrame.time != newKeyFrame.time;
                }
            }*/
            return new PresentationResult
            {
                Change = change,
                Result = newAnimationCurve
            };
        }

        #endregion
    }
}

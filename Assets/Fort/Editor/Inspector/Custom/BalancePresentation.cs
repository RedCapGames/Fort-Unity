using System.Collections.Generic;
using System.Linq;
using Fort.Info;
using Fort.Inspector;
using UnityEditor;
using UnityEngine;


namespace Fort.CustomEditor
{
    public class BalancePresentation:Presentation
    {
        private NumberPresentation[] _numberPresentations = new NumberPresentation[0];
        #region Overrides of Presentation
        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            GUIStyle guiStyle = new GUIStyle();
            EditorGUILayout.BeginVertical(guiStyle);
            Change change = new Change();
            BalancePresentationData balancePresentationData = parameter.PresentationData as BalancePresentationData??new BalancePresentationData();

            bool isFoldout = balancePresentationData.IsFoldout;
            balancePresentationData.IsFoldout = EditorGUILayout.Foldout(balancePresentationData.IsFoldout, parameter.Title);
            change.IsPresentationChanged = isFoldout != balancePresentationData.IsFoldout;

            Balance balance = (Balance)parameter.Instance;
            if(balance == null)
                balance = new Balance();
            balance.SyncValues();
            Dictionary<string, int> values = balance.Values;
            NumberPresentation[] numberPresentations = _numberPresentations;
            _numberPresentations = new NumberPresentation[values.Count];
            KeyValuePair<string, int>[] pairs = values.ToArray();

            if (balancePresentationData.IsFoldout)
            {
                change.ChildrenChange = new Change[_numberPresentations.Length];
                for (int i = 0; i < _numberPresentations.Length; i++)
                {

                    if (i < numberPresentations.Length)
                        _numberPresentations[i] = numberPresentations[i];
                    PresentationParamater presentationParamater = new PresentationParamater(pairs[i].Value, null,
                        pairs[i].Key, typeof(int),
                        new PresentationSite
                        {
                            Base = parameter.Instance,
                            BaseSite = parameter.PresentationSite,
                            BasePresentation = this,
                            SiteType = PresentationSiteType.None
                        }, parameter.FortInspector);
                    if (_numberPresentations[i] == null)
                    {
                        _numberPresentations[i] = new NumberPresentation();
                    }
                    EditorGUILayout.BeginHorizontal(guiStyle);
                    GUILayout.Space(FortInspector.ItemSpacing);
                    EditorGUILayout.BeginVertical(guiStyle);
                    PresentationResult presentationResult = _numberPresentations[i].OnInspectorGui(presentationParamater);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    change.ChildrenChange[i] = presentationResult.Change;

                    values[pairs[i].Key] = (int)presentationResult.Result;
                }
            }
            EditorGUILayout.EndVertical();
            return new PresentationResult
            {
                Change = change,
                PresentationData = balancePresentationData,
                Result = balance
            };
        }

        #endregion

        class BalancePresentationData
        {
            public bool IsFoldout { get; set; }
            public object[] ChildrenPrenetation { get; set; } 
        }
    }

}

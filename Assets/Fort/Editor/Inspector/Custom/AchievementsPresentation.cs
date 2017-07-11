using Fort.Info.Achievement;
using Fort.Inspector;
using UnityEditor;
using UnityEngine;

namespace Fort.CustomEditor
{
    public class AchievementsPresentation : Presentation
    {
        private ConcretePresentation[] _concretePresentations = new ConcretePresentation[0];
        private bool _isFoldout;
        #region Overrides of Presentation

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            GUIStyle guiStyle = new GUIStyle();
            Change change = new Change();
            object presentationData = parameter.PresentationData;
            AchievementsPresentationData achievementPresentationData;
            if (presentationData != null && !((presentationData) is AchievementsPresentationData))
                presentationData = null;
            if (presentationData != null)
            {
                achievementPresentationData = (AchievementsPresentationData)presentationData;
                _isFoldout = achievementPresentationData.IsFoldout;
            }
            else
            {
                achievementPresentationData = new AchievementsPresentationData { IsFoldout = _isFoldout };
            }
            achievementPresentationData.IsFoldout = EditorGUILayout.Foldout(_isFoldout, "Achievements");
            change.IsPresentationChanged |= _isFoldout != achievementPresentationData.IsFoldout;
            _isFoldout = achievementPresentationData.IsFoldout;
            AchievementInfo[] achievementInfos = (AchievementInfo[])parameter.Instance;
            if(achievementInfos == null)
                achievementInfos = new AchievementInfo[0];
            EditorGUILayout.BeginHorizontal(guiStyle);
            GUILayout.Space(FortInspector.ItemSpacing);
            EditorGUILayout.BeginVertical(guiStyle);
            if (_isFoldout)
            {
                ConcretePresentation[] concretePresentations = _concretePresentations;
                _concretePresentations = new ConcretePresentation[achievementInfos.Length];
                object[] innerPresentationData = achievementPresentationData.InnerPresentationData;
                achievementPresentationData.InnerPresentationData = new object[achievementInfos.Length];
                for (int i = 0; i < achievementInfos.Length; i++)
                {
                    if (i < innerPresentationData.Length)
                    {
                        achievementPresentationData.InnerPresentationData[i] = innerPresentationData[i];
                    }
                }

                for (int i = 0; i < _concretePresentations.Length; i++)
                {
                    if (i < concretePresentations.Length)
                    {
                        _concretePresentations[i] = concretePresentations[i];
                    }
                    if (_concretePresentations[i] == null)
                    {
                        _concretePresentations[i] = new ConcretePresentation();
                    }
                }

                change.ChildrenChange = new Change[achievementInfos.Length];
                for (int i = 0; i < achievementInfos.Length; i++)
                {
                    PresentationParamater concreteParamater = new PresentationParamater(achievementInfos[i], achievementPresentationData.InnerPresentationData[i], achievementInfos[i].GetType().Name, typeof(AchievementInfo), new PresentationSite { BaseSite = parameter.PresentationSite, Base = parameter.Instance, BasePresentation = this, SiteType = PresentationSiteType.None }, parameter.FortInspector);
                    PresentationResult presentationResult = _concretePresentations[i].OnInspectorGui(concreteParamater);
                    change.ChildrenChange[i] = presentationResult.Change;
                    achievementPresentationData.InnerPresentationData[i] = presentationResult.PresentationData;
                    achievementInfos[i] = (AchievementInfo)presentationResult.Result;
                }

            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            return new PresentationResult
            {
                Change = change,
                PresentationData = achievementPresentationData,
                Result = achievementInfos
            };
        }
        class AchievementsPresentationData
        {
            public AchievementsPresentationData()
            {
                InnerPresentationData = new object[0];
            }
            public bool IsFoldout { get; set; }
            public object[] InnerPresentationData { get; set; }
        }
        #endregion
    }


}

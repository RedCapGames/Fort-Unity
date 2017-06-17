using Fort.Info.Achievement;
using Fort.Info.PurchasableItem;
using Fort.Inspector;
using UnityEditor;
using UnityEngine;

namespace Fort.CustomEditor
{
    public class PurchasableItemPresentation : Presentation
    {
        private ConcretePresentation[] _concretePresentations = new ConcretePresentation[0];
        private bool _isFoldout;
        #region Overrides of Presentation
        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            Change change = new Change();
            object presentationData = parameter.PresentationData;
            PurchasableItemPresentationData purchasableItemPresentationData;
            if (presentationData != null && !((presentationData) is PurchasableItemPresentationData))
                presentationData = null;
            if (presentationData != null)
            {
                purchasableItemPresentationData = (PurchasableItemPresentationData)presentationData;
                _isFoldout = purchasableItemPresentationData.IsFoldout;
            }
            else
            {
                purchasableItemPresentationData = new PurchasableItemPresentationData { IsFoldout = _isFoldout };
            }
            purchasableItemPresentationData.IsFoldout = EditorGUILayout.Foldout(_isFoldout, "PurchasableItems");
            change.IsPresentationChanged |= _isFoldout != purchasableItemPresentationData.IsFoldout;
            _isFoldout = purchasableItemPresentationData.IsFoldout;

            PurchasableItemInfo[] purchasableItemInfos = (PurchasableItemInfo[])parameter.Instance;
            if (purchasableItemInfos == null)
                purchasableItemInfos = new PurchasableItemInfo[0];
            EditorGUILayout.BeginHorizontal();
            GUILayoutUtility.GetRect(3f, 6f);
            EditorGUILayout.BeginVertical();
            if (_isFoldout)
            {
                ConcretePresentation[] concretePresentations = _concretePresentations;
                _concretePresentations = new ConcretePresentation[purchasableItemInfos.Length];
                object[] innerPresentationData = purchasableItemPresentationData.InnerPresentationData;
                purchasableItemPresentationData.InnerPresentationData = new object[purchasableItemInfos.Length];
                for (int i = 0; i < purchasableItemInfos.Length; i++)
                {
                    if (i < innerPresentationData.Length)
                    {
                        purchasableItemPresentationData.InnerPresentationData[i] = innerPresentationData[i];
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

                change.ChildrenChange = new Change[purchasableItemInfos.Length];
                for (int i = 0; i < purchasableItemInfos.Length; i++)
                {
                    PresentationParamater concreteParamater = new PresentationParamater(purchasableItemInfos[i], purchasableItemPresentationData.InnerPresentationData[i], purchasableItemInfos[i].GetType().Name, typeof(AchievementInfo), new PresentationSite { BaseSite = parameter.PresentationSite, Base = parameter.Instance, BasePresentation = this, SiteType = PresentationSiteType.None }, parameter.FortInspector);
                    PresentationResult presentationResult = _concretePresentations[i].OnInspectorGui(concreteParamater);
                    change.ChildrenChange[i] = presentationResult.Change;
                    purchasableItemPresentationData.InnerPresentationData[i] = presentationResult.PresentationData;
                    purchasableItemInfos[i] = (PurchasableItemInfo)presentationResult.Result;
                }

            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            return new PresentationResult
            {
                Change = change,
                PresentationData = purchasableItemPresentationData,
                Result = purchasableItemInfos
            };
        }

        #endregion
        class PurchasableItemPresentationData
        {
            public PurchasableItemPresentationData()
            {
                InnerPresentationData = new object[0];
            }
            public bool IsFoldout { get; set; }
            public object[] InnerPresentationData { get; set; }
        }
    }
}

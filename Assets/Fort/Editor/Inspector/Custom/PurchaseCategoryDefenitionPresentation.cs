using System.Linq;
using Fort.Info;
using Fort.Inspector;

namespace Fort.CustomEditor
{
    class PurchaseCategoryDefenitionPresentation: ArrayPresentation
    {
        #region Overrides of ArrayPresentation

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            string[] catrgoryDefenitions = (((string[])parameter.Instance)??new string[0]).ToArray();
            PresentationResult presentationResult = base.OnInspectorGui(parameter);
            string[] newCategoryDefenitions = (string[])presentationResult.Result;
            if (!presentationResult.Change.IsDataChanged && presentationResult.Change.ChildrenChange.Any(change => change.IsDataChanged))
            {
                PurchasableItemInfo[] purchasableItemInfos = TypeFinder.FindType(parameter.FortInspector.Targer, typeof(PurchasableItemInfo)).Cast<PurchasableItemInfo>().ToArray();
                for (int i = 0; i < presentationResult.Change.ChildrenChange.Length; i++)
                {
                    if (presentationResult.Change.ChildrenChange[i].IsDataChanged)
                    {
                        foreach (PurchasableItemInfo purchasableItemInfo in purchasableItemInfos)
                        {
                            if (purchasableItemInfo.Category == catrgoryDefenitions[i])
                                purchasableItemInfo.Category = newCategoryDefenitions[i];                            
                        }
                    }
                }
            }
            return presentationResult;
        }

        #endregion
    }
}

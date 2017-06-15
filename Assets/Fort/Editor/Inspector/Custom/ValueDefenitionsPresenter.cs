using System.Linq;
using Fort.Info;
using Fort.Inspector;

namespace Fort.CustomEditor
{
    public class ValueDefenitionsPresenter:ArrayPresentation
    {
        #region Overrides of ArrayPresentation

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            string[] valueDefenitions = (((string[])parameter.Instance) ?? new string[0]).ToArray();
            PresentationResult presentationResult = base.OnInspectorGui(parameter);
            string[] newValueDefenitions = (string[]) presentationResult.Result;
            if (!presentationResult.Change.IsDataChanged && presentationResult.Change.ChildrenChange.Any(change => change.IsDataChanged))
            {
                Balance[] balances = TypeFinder.FindType(parameter.FortInspector.Targer,typeof(Balance)).Cast<Balance>().ToArray();
                for (int i = 0; i < presentationResult.Change.ChildrenChange.Length; i++)
                {
                    if (presentationResult.Change.ChildrenChange[i].IsDataChanged)
                    {
                        foreach (Balance balance in balances)
                        {
                            balance.SyncValueKey(valueDefenitions[i],newValueDefenitions[i]);
                        }
                    }
                }
            }
            return presentationResult;
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Linq;
using Fort.Info;
using Fort.Inspector;


namespace Fort.CustomEditor
{
    public class BalancePresentation:Presentation
    {
        private NumberPresentation[] _numberPresentations = new NumberPresentation[0];
        #region Overrides of Presentation
        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            Balance balance = (Balance)parameter.Instance;
            if(balance == null)
                balance = new Balance();
            balance.SyncValues();
            Dictionary<string, int> values = balance.Values;
            NumberPresentation[] numberPresentations = _numberPresentations;
            _numberPresentations = new NumberPresentation[values.Count];
            KeyValuePair<string, int>[] pairs = values.ToArray();
            Change change = new Change();
            change.ChildrenChange = new Change[_numberPresentations.Length];
            for (int i = 0; i < _numberPresentations.Length; i++)
            {

                if (i < numberPresentations.Length)
                    _numberPresentations[i] = numberPresentations[i];
                PresentationParamater presentationParamater = new PresentationParamater(pairs[i].Value, null,
                    pairs[i].Key, typeof (int),
                    new PresentationSite
                    {
                        Base = parameter.Instance,
                        BaseSite = parameter.PresentationSite,
                        BasePresentation = this,
                        SiteType = PresentationSiteType.None
                    }, parameter.FortInspector);
                if (_numberPresentations[i] ==null)
                {
                    _numberPresentations[i] = new NumberPresentation();
                }
                PresentationResult presentationResult = _numberPresentations[i].OnInspectorGui(presentationParamater);
                change.ChildrenChange[i] = presentationResult.Change;
                
                values[pairs[i].Key] = (int) presentationResult.Result;
            }
            return new PresentationResult
            {
                Change = change,
                PresentationData = null,
                Result = balance
            };
        }

        #endregion
    }
}

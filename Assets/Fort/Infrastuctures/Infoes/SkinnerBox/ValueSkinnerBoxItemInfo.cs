namespace Fort.Info.SkinnerBox
{
    public abstract class ValueSkinnerBoxItemInfo : SkinnerBoxItemInfo
    {
        protected ValueSkinnerBoxItemInfo()
        {
            Value = new Balance();
        }
        public Balance Value { get; set; }
    }
}
namespace Fort.Info.Market.Iap
{
    public class IapPackage
    {
        public IapPackage()
        {
            Packages = new IapPackageInfo[0];
        }
        public IapPackageInfo[] Packages { get; set; }
    }
}
namespace Fort.Advertisement
{
    public interface IAdvertisementProvider
    {
        string Name { get; }
        bool IsVideoSupported { get; }
        bool IsStandardBannerSupported { get; }
        bool IsInterstitialBannerSupported { get; }        
        ErrorPromise<ShowVideoFailed> ShowVideo(string zone, bool skipable);

        void ChangeStandardBannerPosition(StandardBannerVerticalAlignment verticalAlignment,
            StandardBannerHorizantalAlignment horizantalAlignment);
        void ShowStandardBanner();
        void HideStandardBanner();
        void ShowInterstiatialBanner();
    }
}

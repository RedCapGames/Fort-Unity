namespace Fort
{
    public interface IAdvertisementService
    {
        bool IsVideoSupported { get; }
        bool IsStandardBannerSupported { get; }
        bool IsInterstitialBannerSupported { get; }
        ErrorPromise<ShowVideoFailed> ShowVideo(int zone, bool skipable,bool removabale);

        void ChangeStandardBannerPosition(StandardBannerVerticalAlignment verticalAlignment,
            StandardBannerHorizantalAlignment horizantalAlignment);
        void ShowStandardBanner();
        void HideStandardBanner();
        void ShowInterstiatialBanner();
    }

    public enum StandardBannerVerticalAlignment
    {
        Top,
        Bottom
    }

    public enum StandardBannerHorizantalAlignment
    {
        Left,
        Center,
        Right
    }
    public enum ShowVideoFailed
    {
        Cancel,
        NoVideoAvilable,
        ProviderError,
        AdvertisementRemoved
    }
}
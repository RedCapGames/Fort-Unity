namespace Fort
{
    /// <summary>
    /// This Service is used to manage advertisement. Ability such as adding advertisement providers and prioritize them or optimization of video fill rate 
    /// </summary>
    public interface IAdvertisementService
    {
        /// <summary>
        /// Is any added video provider support Video
        /// </summary>
        bool IsVideoSupported { get; }
        /// <summary>
        /// Is any added video provider support Standard Banner ad
        /// </summary>
        bool IsStandardBannerSupported { get; }
        /// <summary>
        /// Is any added video provider support Interstitial Banner
        /// </summary>
        bool IsInterstitialBannerSupported { get; }
        /// <summary>
        /// Show video
        /// </summary>
        /// <param name="zone">zone is used in video providers to classifying advertisement</param>
        /// <param name="skipable">Is video skipable</param>
        /// <param name="removabale">Is showing this video can be removed by purchasing RemoveAdIapPackage</param>
        /// <returns>Return a promise</returns>
        ErrorPromise<ShowVideoFailed> ShowVideo(int zone, bool skipable,bool removabale);

        /// <summary>
        /// Changing standard banner Position
        /// </summary>
        /// <param name="verticalAlignment">Vertical Alignment</param>
        /// <param name="horizantalAlignment">Horizantal Alignment</param>
        void ChangeStandardBannerPosition(StandardBannerVerticalAlignment verticalAlignment,
            StandardBannerHorizantalAlignment horizantalAlignment);
        /// <summary>
        /// Shownig standard banner
        /// </summary>
        void ShowStandardBanner();
        /// <summary>
        /// Hiding standard banner
        /// </summary>
        void HideStandardBanner();
        /// <summary>
        /// Showing interstiatial banner
        /// </summary>
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
        AdvertisementRemoved,
        AlreadyShowingVideo
    }
}
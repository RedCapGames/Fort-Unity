using System;
using Fort.Info.SkinnerBox;

namespace Fort
{
    /// <summary>
    /// Service to manage skinner box
    /// </summary>
    public interface ISkinnerBoxService
    {
        /// <summary>
        /// Return if skinner box available
        /// </summary>
        /// <param name="boxInfo">Free box info</param>
        /// <returns>Is skinner box available</returns>
        bool IsFreeSkinnerBoxAvailable(FreeSkinnerBoxInfo boxInfo);
        /// <summary>
        /// Resolve the count of purchased box
        /// </summary>
        /// <param name="boxInfo"></param>
        /// <returns>count of purchased box</returns>
        int GetPurchableskinnerBoxCount(PurchableSkinnerBoxInfo boxInfo);
        /// <summary>
        /// Opening a box
        /// </summary>
        /// <param name="boxInfo">Corresponding skinner box</param>
        /// <returns>Returning item that is picked from skinner box</returns>
        SkinnerBoxItemInfo OpenBox(SkinnerBoxInfo boxInfo);
        /// <summary>
        /// Returning the availability delay of free skinner box
        /// </summary>
        /// <param name="boxInfo">Corresponding free skinner box</param>
        /// <returns>The availability delay of free skinner box</returns>
        TimeSpan GetFreeSkinnerBoxAvailabiltyDuration(FreeSkinnerBoxInfo boxInfo);
    }

}

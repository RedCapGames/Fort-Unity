﻿using System;
using Fort.Info;

namespace Fort
{
    public interface ISkinnerBoxService
    {
        bool IsFreeSkinnerBoxAvailable(FreeSkinnerBoxInfo boxInfo);
        int GetPurchableskinnerBoxCount(PurchableSkinnerBoxInfo boxInfo);
        SkinnerBoxItemInfo OpenBox(SkinnerBoxInfo boxInfo);
        TimeSpan GetFreeSkinnerBoxAvailabiltyDuration(FreeSkinnerBoxInfo boxInfo);
    }

}

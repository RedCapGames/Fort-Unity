using System;
using Fort.Inspector;

namespace Fort.Info
{
    public class PurchasableLevelInfo
    {
        public PurchasableLevelInfo()
        {
            Id = Guid.NewGuid().ToString();
        }
        [IgnoreProperty]
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool DefaultBought { get; set; }
        public Balance Cost { get; set; }

    }
}
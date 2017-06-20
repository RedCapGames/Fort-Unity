namespace Fort.Info.PurchasableItem
{
    public class ItemCosts
    {
        public ItemCosts()
        {
            Purchase = new Balance();
            Rent = new Balance();
        }
        public Balance Purchase { get; set; }
        public Balance Rent { get; set; }
    }
}
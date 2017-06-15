using Fort.Info;

public class CarPurchasableItem:NoneLevelBasePurchasableItemInfo
{
    public string CarType { get; set; }
}

public class EnginePurchasableItem : LevelBasePurchasableItemInfo<EnginePurchasableLevelInfo>
{
    
}

public class EnginePurchasableLevelInfo: PurchasableLevelInfo
{
    public int EnginePower { get; set; }
}
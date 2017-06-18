using System.IO;
using Fort.Info;
using Fort.Info.Achievement;
using Fort.Info.PurchasableItem;
using UnityEditor;

namespace Fort.Export
{
    public static class PurchaseExporter
    {
        [MenuItem("Fort/Import-Export/Purchase/Export Purchase Items")]
        public static void ExportPurchaseItems()
        {
            string path = EditorUtility.SaveFilePanel("Export Purchase Items", "", "", "csv");
            if (string.IsNullOrEmpty(path))
                return;
            using (StreamWriter writer = new StreamWriter(path))
            {
                ExportData exportData = new ExportData();
                foreach (PurchasableItemInfo purchasableItemInfo in InfoResolver.FortInfo.Purchase.GetAllPurchasableItemInfos())
                {
                    ExportRow exportRow = new ExportRow();
                    exportRow.AddParameter("Id", new Parameter
                    {
                        Value = purchasableItemInfo.Id,
                        Type = typeof(string)
                    });
                    exportRow.AddParameter("Name", new Parameter
                    {
                        Value = purchasableItemInfo.Name,
                        Type = typeof(string)
                    });
                    exportRow.AddParameter("DisplayName", new Parameter
                    {
                        Value = purchasableItemInfo.DisplayName,
                        Type = typeof(string)
                    });
                    exportRow.AddParameter("Description", new Parameter
                    {
                        Value = purchasableItemInfo.Description,
                        Type = typeof(string)
                    });
                    exportRow.AddParameter("DefaultBought", new Parameter
                    {
                        Value = purchasableItemInfo.DefaultBought,
                        Type = typeof(bool)
                    });
                    NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo = purchasableItemInfo as NoneLevelBasePurchasableItemInfo;
                    if (noneLevelBasePurchasableItemInfo != null)
                    {


                        foreach (string valueDefenition in InfoResolver.FortInfo.ValueDefenitions)
                        {
                            exportRow.AddParameter(string.Format("PurchaseCost-{0}", valueDefenition), new Parameter
                            {
                                Value = noneLevelBasePurchasableItemInfo.Costs == null || noneLevelBasePurchasableItemInfo.Costs.Purchase==null ||
                                        noneLevelBasePurchasableItemInfo.Costs.Purchase.Values == null
                                    ? 0
                                    : noneLevelBasePurchasableItemInfo.Costs.Purchase[valueDefenition],
                                Type = typeof(int)
                            });
                        }
                        foreach (string valueDefenition in InfoResolver.FortInfo.ValueDefenitions)
                        {
                            exportRow.AddParameter(string.Format("RentCost-{0}", valueDefenition), new Parameter
                            {
                                Value = noneLevelBasePurchasableItemInfo.Costs == null || noneLevelBasePurchasableItemInfo.Costs.Rent == null ||
                                        noneLevelBasePurchasableItemInfo.Costs.Rent.Values == null
                                    ? 0
                                    : noneLevelBasePurchasableItemInfo.Costs.Rent[valueDefenition],
                                Type = typeof(int)
                            });
                        }
                        exportRow.AddCustomExportParameter(purchasableItemInfo);
                        exportData.AddRow(exportRow);
                    }
                    else
                    {
                        exportRow.AddCustomExportParameter(purchasableItemInfo);
                        exportData.AddRow(exportRow);
                        LevelBasePurchasableItemInfo levelBasePurchasableItemInfo = purchasableItemInfo as LevelBasePurchasableItemInfo;
                        if (levelBasePurchasableItemInfo != null)
                        {
                            PurchasableLevelInfo[] purchasableLevelInfos = levelBasePurchasableItemInfo.GetPurchasableLevelInfos();
                            int index = 0;
                            foreach (PurchasableLevelInfo purchasableLevelInfo in purchasableLevelInfos)
                            {
                                ExportRow levelExportRow = new ExportRow();
                                levelExportRow.AddParameter("Id", new Parameter
                                {
                                    Value = purchasableLevelInfo.Id,
                                    Type = typeof(string)
                                });
                                levelExportRow.AddParameter("Name", new Parameter
                                {
                                    Value = string.Format("{0}_{1}", purchasableItemInfo.Name,index++),
                                    Type = typeof(string)
                                });
                                levelExportRow.AddParameter("DisplayName", new Parameter
                                {
                                    Value = purchasableLevelInfo.DisplayName,
                                    Type = typeof(string)
                                });
                                levelExportRow.AddParameter("Description", new Parameter
                                {
                                    Value = purchasableLevelInfo.Description,
                                    Type = typeof(string)
                                });
                                levelExportRow.AddParameter("DefaultBought", new Parameter
                                {
                                    Value = purchasableLevelInfo.DefaultBought,
                                    Type = typeof(bool)
                                });

                                foreach (string valueDefenition in InfoResolver.FortInfo.ValueDefenitions)
                                {
                                    levelExportRow.AddParameter(string.Format("PurchaseCost-{0}", valueDefenition), new Parameter
                                    {
                                        Value = purchasableLevelInfo.Costs == null || purchasableLevelInfo.Costs.Purchase == null ||
                                                purchasableLevelInfo.Costs.Purchase.Values == null
                                            ? 0
                                            : purchasableLevelInfo.Costs.Purchase[valueDefenition],
                                        Type = typeof(int)
                                    });
                                }
                                foreach (string valueDefenition in InfoResolver.FortInfo.ValueDefenitions)
                                {
                                    levelExportRow.AddParameter(string.Format("RentCost-{0}", valueDefenition), new Parameter
                                    {
                                        Value = purchasableLevelInfo.Costs == null || purchasableLevelInfo.Costs.Rent == null ||
                                                purchasableLevelInfo.Costs.Rent.Values == null
                                            ? 0
                                            : purchasableLevelInfo.Costs.Rent[valueDefenition],
                                        Type = typeof(int)
                                    });
                                }
                                levelExportRow.AddCustomExportParameter(purchasableLevelInfo);
                                exportData.AddRow(levelExportRow);
                            }
                        }
                    }
                }

                writer.Write(exportData.SerializeToCsv());
            }
        }
    }
}

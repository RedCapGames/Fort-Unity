using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Fort.Info;
using Fort.Info.PurchasableItem;
using NPOI.HSSF.UserModel;
using UnityEditor;

namespace Fort.Export
{
    public static class PurchaseExporter
    {
        [MenuItem("Fort/Import-Export/Purchase/Export Purchase Items")]
        public static void ExportPurchaseItems()
        {
            string path = EditorUtility.SaveFilePanel("Export Purchase Items", "", "", "xls");
            if (string.IsNullOrEmpty(path))
                return;
            using (Stream writer = File.Create(path))
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
/*                    exportRow.AddParameter("DisplayName", new Parameter
                    {
                        Value = purchasableItemInfo.DisplayName,
                        Type = typeof(string)
                    });*/
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
/*                                levelExportRow.AddParameter("DisplayName", new Parameter
                                {
                                    Value = purchasableLevelInfo.DisplayName,
                                    Type = typeof(string)
                                });*/
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

                HSSFWorkbook workbook = new HSSFWorkbook();

                HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet("Purchasable Items");
                exportData.SerializeToSheet(sheet);
                workbook.Write(writer);
            }
        }
        [MenuItem("Fort/Import-Export/Purchase/Import Purchase Items")]
        public static void ImportAchievements()
        {

            string path = EditorUtility.OpenFilePanel("Import Purchase Items", "", "xls");
            if (string.IsNullOrEmpty(path))
                return;
            using (Stream reader = File.OpenRead(path))
            {
                IDictionary<string, PropertyInfo> customPossibleProperties =
                    ExportData.GetCustomPossibleProperties(
                        TypeHelper.GetAllTypes(AllTypeCategory.Game)
                            .Where(type => typeof(NoneLevelBasePurchasableItemInfo).IsAssignableFrom(type))
                            .Concat(
                                TypeHelper.GetAllTypes(AllTypeCategory.Game)
                                    .Where(type => typeof(LevelBasePurchasableItemInfo).IsAssignableFrom(type)))
                            .Concat(
                                TypeHelper.GetAllTypes(AllTypeCategory.Game)
                                    .Where(type => typeof(PurchasableLevelInfo).IsAssignableFrom(type)))
                            .ToArray());
                Dictionary<string, Type> parameters = new Dictionary<string, Type>();
                parameters["Id"] = typeof(string);
                parameters["Name"] = typeof(string);
                //parameters["DisplayName"] = typeof(string);
                parameters["Description"] = typeof(string);
                parameters["DefaultBought"] = typeof(bool);
                foreach (string valueDefenition in InfoResolver.FortInfo.ValueDefenitions)
                {
                    parameters[string.Format("PurchaseCost-{0}", valueDefenition)] = typeof(int);
                    parameters[string.Format("RentCost-{0}", valueDefenition)] = typeof(int);
                }
                foreach (KeyValuePair<string, PropertyInfo> pair in customPossibleProperties)
                {
                    parameters[pair.Key] = pair.Value.PropertyType;
                }
                HSSFWorkbook workbook = new HSSFWorkbook(reader);
                ExportData exportData = ExportData.DeserializeFromSheet(parameters, workbook.GetSheetAt(0));
                foreach (ExportRow exportRow in exportData.ExportRows)
                {
                    if (!exportRow.ContainsParameter("Id"))
                        continue;
                    string id = (string)exportRow.GetValue("Id").Value;
                    if (!InfoResolver.FortInfo.Purchase.PurchasableTokens.ContainsKey(id))
                        continue;
                    PurchasableToken purchasableToken = InfoResolver.FortInfo.Purchase.PurchasableTokens[id];
                    if (purchasableToken.NoneLevelBase)
                    {
                        NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo = (NoneLevelBasePurchasableItemInfo)purchasableToken.PurchasableItemInfo;
                        if (exportRow.ContainsParameter("Name"))
                        {
                            noneLevelBasePurchasableItemInfo.Name = (string) exportRow.GetValue("Name").Value;
                        }
/*                        if (exportRow.ContainsParameter("DisplayName"))
                        {
                            noneLevelBasePurchasableItemInfo.DisplayName = (string)exportRow.GetValue("DisplayName").Value;
                        }*/
                        if (exportRow.ContainsParameter("Description"))
                        {
                            noneLevelBasePurchasableItemInfo.Description = (string)exportRow.GetValue("Description").Value;
                        }
                        if (exportRow.ContainsParameter("DefaultBought"))
                        {
                            noneLevelBasePurchasableItemInfo.DefaultBought = (bool)exportRow.GetValue("DefaultBought").Value;
                        }
                        if(noneLevelBasePurchasableItemInfo.Costs == null)
                            noneLevelBasePurchasableItemInfo.Costs = new ItemCosts();
                        
                        foreach (string valueDefenition in InfoResolver.FortInfo.ValueDefenitions)
                        {
                            string purchaseValueDefenition = string.Format("PurchaseCost-{0}", valueDefenition);
                            if (exportRow.ContainsParameter(purchaseValueDefenition))
                            {
                                noneLevelBasePurchasableItemInfo.Costs.Purchase[valueDefenition] =
                                    (int)exportRow.GetValue(purchaseValueDefenition).Value;
                            }
                            string rentValueDefenition = string.Format("RentCost-{0}", valueDefenition);
                            if (exportRow.ContainsParameter(rentValueDefenition))
                            {
                                noneLevelBasePurchasableItemInfo.Costs.Purchase[valueDefenition] =
                                    (int)exportRow.GetValue(rentValueDefenition).Value;
                            }
                        }
                        exportRow.FillCustomExportParameter(noneLevelBasePurchasableItemInfo);
                    }
                    else
                    {
                        if (purchasableToken.PurchasableLevelInfo == null)
                        {
                            LevelBasePurchasableItemInfo levelBasePurchasableItemInfo = (LevelBasePurchasableItemInfo)purchasableToken.PurchasableItemInfo;
                            if (exportRow.ContainsParameter("Name"))
                            {
                                levelBasePurchasableItemInfo.Name = (string)exportRow.GetValue("Name").Value;
                            }
/*                            if (exportRow.ContainsParameter("DisplayName"))
                            {
                                levelBasePurchasableItemInfo.DisplayName = (string)exportRow.GetValue("DisplayName").Value;
                            }*/
                            if (exportRow.ContainsParameter("Description"))
                            {
                                levelBasePurchasableItemInfo.Description = (string)exportRow.GetValue("Description").Value;
                            }
                            if (exportRow.ContainsParameter("DefaultBought"))
                            {
                                levelBasePurchasableItemInfo.DefaultBought = (bool)exportRow.GetValue("DefaultBought").Value;
                            }
                            exportRow.FillCustomExportParameter(levelBasePurchasableItemInfo);
                        }
                        else
                        {
                            PurchasableLevelInfo purchasableLevelInfo = purchasableToken.PurchasableLevelInfo;
/*                            if (exportRow.ContainsParameter("DisplayName"))
                            {
                                purchasableLevelInfo.DisplayName = (string)exportRow.GetValue("DisplayName").Value;
                            }*/
                            if (exportRow.ContainsParameter("Description"))
                            {
                                purchasableLevelInfo.Description = (string)exportRow.GetValue("Description").Value;
                            }
                            if (exportRow.ContainsParameter("DefaultBought"))
                            {
                                purchasableLevelInfo.DefaultBought = (bool)exportRow.GetValue("DefaultBought").Value;
                            }
                            foreach (string valueDefenition in InfoResolver.FortInfo.ValueDefenitions)
                            {
                                string purchaseValueDefenition = string.Format("PurchaseCost-{0}", valueDefenition);
                                if (exportRow.ContainsParameter(purchaseValueDefenition))
                                {
                                    purchasableLevelInfo.Costs.Purchase[valueDefenition] =
                                        (int)exportRow.GetValue(purchaseValueDefenition).Value;
                                }
                                string rentValueDefenition = string.Format("RentCost-{0}", valueDefenition);
                                if (exportRow.ContainsParameter(rentValueDefenition))
                                {
                                    purchasableLevelInfo.Costs.Purchase[valueDefenition] =
                                        (int)exportRow.GetValue(rentValueDefenition).Value;
                                }
                            }
                            exportRow.FillCustomExportParameter(purchasableLevelInfo);
                        }


                    }
                }
            }
            InfoResolver.FortInfo.Save();
        }
    }
}

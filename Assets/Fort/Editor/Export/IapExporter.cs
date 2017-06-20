using System.IO;
using Fort;
using Fort.Export;
using Fort.Info;
using Fort.Info.Language;
using Fort.Info.Market.Iap;
using NPOI.HSSF.UserModel;
using UnityEditor;

namespace Assets.Fort.Editor.Export
{
    public static class IapExporter
    {
        [MenuItem("Fort/Import-Export/Iap Package/Export")]
        public static void ExportIapPackages()
        {
            string path = EditorUtility.SaveFilePanel("Export Iap packages", "", "", "xls");
            if (string.IsNullOrEmpty(path))
                return;
            using (Stream writer = File.Create(path))
            {
                ExportData exportData = new ExportData();
                foreach (IapPackageInfo iapPackageInfo in InfoResolver.FortInfo.Package.Packages)
                {
                    ExportRow exportRow = new ExportRow();
                    exportRow.AddParameter("Sku", new Parameter
                    {
                        Value = iapPackageInfo.Sku,
                        Type = typeof(string)
                    });
                    LanguageInfo[] languageInfos = LanguageInfoResolver.LanguageEditorInfo.Languages;
                    if (languageInfos.Length == 1)
                    {
                        if (iapPackageInfo.DisplayName != null)
                        {
                            if (languageInfos[0].LanguageDatas.ContainsKey(iapPackageInfo.DisplayName.Id))
                            {
                                exportRow.AddParameter("DisplayName", new Parameter
                                {
                                    Value = languageInfos[0].LanguageDatas[iapPackageInfo.DisplayName.Id],
                                    Type = typeof(string)
                                });

                            }

                        }

                    }
                    else
                    {
                        foreach (LanguageInfo languageInfo in LanguageInfoResolver.LanguageEditorInfo.Languages)
                        {
                            if (iapPackageInfo.DisplayName != null)
                            {
                                if (languageInfo.LanguageDatas.ContainsKey(iapPackageInfo.DisplayName.Id))
                                {
                                    exportRow.AddParameter(string.Format("DisplayName-{0}", languageInfo.Name), new Parameter
                                    {
                                        Value = languageInfo.LanguageDatas[iapPackageInfo.DisplayName.Id],
                                        Type = typeof(string)
                                    });

                                }

                            }
                        }

                    }
                    exportRow.AddParameter("Price", new Parameter
                    {
                        Value = iapPackageInfo.Price,
                        Type = typeof(int)
                    });
                    exportRow.AddCustomExportParameter(iapPackageInfo);
                    exportData.AddRow(exportRow);
                }
                HSSFWorkbook workbook = new HSSFWorkbook();

                HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet("Iap packages");
                exportData.SerializeToSheet(sheet);
                workbook.Write(writer);
            }
        }
    }
}

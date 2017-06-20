using System.IO;
using Fort.Export;
using Fort.Info;
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
                    exportRow.AddParameter("DisplayName", new Parameter
                    {
                        Value = iapPackageInfo.DisplayName,
                        Type = typeof(string)
                    });
                    exportRow.AddParameter("Price", new Parameter
                    {
                        Value = iapPackageInfo.DisplayName,
                        Type = typeof(string)
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

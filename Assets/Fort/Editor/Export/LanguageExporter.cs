using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fort;
using Fort.Info;
using Fort.Info.Language;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using UnityEditor;

namespace Assets.Fort.Editor.Export
{
    public static class LanguageExporter
    {
        [MenuItem("Fort/Import-Export/Language/Export")]
        public static void ExportLanguages()
        {
            string path = EditorUtility.SaveFilePanel("Export Languages", "", "", "xls");
            if (string.IsNullOrEmpty(path))
                return;
            using (Stream writer = File.Create(path))
            {
                LanguageItem[] allLanguageItems = TypeHelper.FindType(InfoResolver.FortInfo,typeof(LanguageItem<string>)).Cast<LanguageItem>().ToArray();
                LanguageEditorInfo languageEditorInfo = LanguageInfoResolver.LanguageEditorInfo;
                string[] languageNames = languageEditorInfo.Languages.Select(info => info.Name).ToArray();

                HSSFWorkbook workbook = new HSSFWorkbook();

                HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet("Languages");
                HSSFRow header = (HSSFRow)sheet.CreateRow(0);
                HSSFCell hssfCell = (HSSFCell)header.CreateCell(0);
                hssfCell.SetCellValue("Item Id");
                for (int i = 0; i < languageNames.Length; i++)
                {
                    HSSFCell cell = (HSSFCell)header.CreateCell(i+1);
                    cell.SetCellValue(languageNames[i]);
                }
                for (int i = 0; i < allLanguageItems.Length; i++)
                {
                    
                    HSSFRow row = (HSSFRow)sheet.CreateRow(i+1);
                    HSSFCell itemIdcell = (HSSFCell)row.CreateCell(0);
                    itemIdcell.SetCellValue(allLanguageItems[i].Id);
                    for (int j = 0; j < languageNames.Length; j++)
                    {
                        
                        HSSFCell cell = (HSSFCell)row.CreateCell(j + 1);
                        if (languageEditorInfo.Languages[j].LanguageDatas.ContainsKey(allLanguageItems[i].Id))
                        {
                            object languageData = languageEditorInfo.Languages[j].LanguageDatas[allLanguageItems[i].Id];
                            cell.SetCellValue(languageData== null?string.Empty:languageData.ToString());
                        }
                        else
                        {
                            cell.SetCellValue(string.Empty);
                        }
                        
                    }
                }
                for (int i = 0; i < languageNames.Length+1; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
                workbook.Write(writer);
            }
        }

        [MenuItem("Fort/Import-Export/Language/Import")]
        public static void ImportLanguages()
        {

            string path = EditorUtility.OpenFilePanel("Import Languages", "", "xls");
            if (string.IsNullOrEmpty(path))
                return;
            using (Stream reader = File.OpenRead(path))
            {
                HSSFWorkbook workbook = new HSSFWorkbook(reader);
                ISheet sheet = workbook.GetSheetAt(0);
                int index = 0;
                List<string> languageNames = new List<string>();
                LanguageEditorInfo languageEditorInfo = LanguageInfoResolver.LanguageEditorInfo;
                LanguageItem[] allLanguageItems = TypeHelper.FindType(InfoResolver.FortInfo, typeof(LanguageItem<string>)).Cast<LanguageItem>().ToArray();
                while (true)
                {
                    IRow row = sheet.GetRow(index);
                    if(row == null)
                        break;
                    if (index == 0)
                    {
                        List<string> headers = new List<string>();
                        int cellIndex = 0;
                        while (true)
                        {
                            ICell cell = row.GetCell(cellIndex++);
                            if (cell == null)
                                break;
                            headers.Add(cell.ToString()); 
                        }
                        for (int i = 1; i < headers.Count; i++)
                        {
                            languageNames.Add(headers[i]);
                        }
                    }
                    else
                    {
                        string itemId = string.Empty;
                        for (int i = 0; i < languageNames.Count+1; i++)
                        {
                            if (i == 0)
                            {
                                ICell cell = row.GetCell(i);
                                if(cell == null)
                                    break;
                                itemId = cell.ToString();
                                if(allLanguageItems.All(item => item.Id != itemId))
                                    break;
                            }
                            else
                            {
                                ICell cell = row.GetCell(i);
                                if (cell == null)
                                    break;
                                LanguageInfo languageInfo = languageEditorInfo.Languages.FirstOrDefault(info => info.Name == languageNames[i-1]);
                                if(languageInfo==null)
                                    continue;
                                languageInfo.LanguageDatas[itemId] = cell.ToString();
                            }
                        }
                    }
                    index++;
                }
                languageEditorInfo.SyncFortAndSave(true);

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Fort;
using Fort.Export;
using Fort.Info;
using Fort.Info.GameLevel;
using NPOI.HSSF.UserModel;
using UnityEditor;

namespace Assets.Fort.Editor.Export
{
    public static class GameLevelExporter
    {
        [MenuItem("Fort/Import-Export/GameLevel/Export Game Categories")]
        public static void ExportGameCategories()
        {
            string path = EditorUtility.SaveFilePanel("Export Game Categories", "", "", "xls");
            if (string.IsNullOrEmpty(path))
                return;
            using (Stream writer = File.Create(path))
            {
                ExportData exportData = new ExportData();
                foreach (GameLevelCategory gameLevelCategory in InfoResolver.FortInfo.GameLevel.LevelCategories.Select(pair => pair.Value))
                {
                    ExportRow exportRow = new ExportRow();
                    exportRow.AddParameter("Id", new Parameter
                    {
                        Value = gameLevelCategory.Id,
                        Type = typeof(string)
                    });
/*                    exportRow.AddParameter("DefaultScene", new Parameter
                    {
                        Value = gameLevelCategory.DefaultScene == null?null: gameLevelCategory.DefaultScene.Value.SceneName,
                        Type = typeof(string)
                    });*/
/*                    exportRow.AddParameter("DisplayName", new Parameter
                    {
                        Value = gameLevelCategory.DisplayName,
                        Type = typeof(string)
                    });*/
                    exportRow.AddParameter("Name", new Parameter
                    {
                        Value = gameLevelCategory.Name,
                        Type = typeof(string)
                    });
                    exportRow.AddCustomExportParameter(gameLevelCategory);
                    exportData.AddRow(exportRow);
                }
                HSSFWorkbook workbook = new HSSFWorkbook();

                HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet("Game Categories");
                exportData.SerializeToSheet(sheet);
                workbook.Write(writer);
            }
        }
        [MenuItem("Fort/Import-Export/GameLevel/Export Game Levels")]
        public static void ExportGameLevels()
        {
            string path = EditorUtility.SaveFilePanel("Export Game Levels", "", "", "xls");
            if (string.IsNullOrEmpty(path))
                return;
            using (Stream writer = File.Create(path))
            {
                ExportData exportData = new ExportData();
                foreach (GameLevelInfo gameLevel in InfoResolver.FortInfo.GameLevel.GameLevelInfos.Select(pair => pair.Value))
                {
                    ExportRow exportRow = new ExportRow();
                    exportRow.AddParameter("Id", new Parameter
                    {
                        Value = gameLevel.Id,
                        Type = typeof(string)
                    });
/*                    exportRow.AddParameter("Scene", new Parameter
                    {
                        Value = gameLevel.Scene == null?null: gameLevel.Scene.Value.SceneName,
                        Type = typeof(string)
                    });*/
/*                    exportRow.AddParameter("DisplayName", new Parameter
                    {
                        Value = gameLevel.DisplayName,
                        Type = typeof(string)
                    });*/
                    exportRow.AddParameter("Name", new Parameter
                    {
                        Value = gameLevel.Name,
                        Type = typeof(string)
                    });
                    exportRow.AddCustomExportParameter(gameLevel);
                    exportData.AddRow(exportRow);
                }
                HSSFWorkbook workbook = new HSSFWorkbook();

                HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet("Game Levels");
                exportData.SerializeToSheet(sheet);
                workbook.Write(writer);
            }
        }

        [MenuItem("Fort/Import-Export/GameLevel/Import Game Categories")]
        public static void ImportGameCategories()
        {

            string path = EditorUtility.OpenFilePanel("Import Game Categories", "", "xls");
            if (string.IsNullOrEmpty(path))
                return;
            using (Stream reader = File.OpenRead(path))
            {
                IDictionary<string, PropertyInfo> customPossibleProperties =
                    ExportData.GetCustomPossibleProperties(
                        TypeHelper.GetAllTypes(AllTypeCategory.Game)
                            .Where(type => typeof(GameLevelCategory).IsAssignableFrom(type))
                            .ToArray());
                Dictionary<string, Type> parameters = new Dictionary<string, Type>();
                parameters["Id"] = typeof(string);
                //parameters["DefaultScene"] = typeof(string);
                //parameters["DisplayName"] = typeof(string);
                parameters["Name"] = typeof(string);
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
                    if (!InfoResolver.FortInfo.GameLevel.LevelCategories.ContainsKey(id))
                        continue;
                    GameLevelCategory gameLevelCategory = InfoResolver.FortInfo.GameLevel.LevelCategories[id];
                    if (exportRow.ContainsParameter("DefaultScene"))
                    {
/*                        gameLevelCategory.DefaultScene = new FortScene();
                        gameLevelCategory.DefaultScene.Value.SceneName = (string)exportRow.GetValue("DefaultScene").Value;*/
                    }
/*                    if (exportRow.ContainsParameter("DisplayName"))
                    {                        
                        gameLevelCategory.DisplayName = (string)exportRow.GetValue("DisplayName").Value;
                    }*/
                    if (exportRow.ContainsParameter("Name"))
                    {
                        gameLevelCategory.Name = (string)exportRow.GetValue("Name").Value;
                    }
                    exportRow.FillCustomExportParameter(gameLevelCategory);

                }
            }
            InfoResolver.FortInfo.Save();
        }

        [MenuItem("Fort/Import-Export/GameLevel/Import Game Levels")]
        public static void ImportGameLevels()
        {

            string path = EditorUtility.OpenFilePanel("Import Game Levels", "", "xls");
            if (string.IsNullOrEmpty(path))
                return;
            using (Stream reader = File.OpenRead(path))
            {
                IDictionary<string, PropertyInfo> customPossibleProperties =
                    ExportData.GetCustomPossibleProperties(
                        TypeHelper.GetAllTypes(AllTypeCategory.Game)
                            .Where(type => typeof(GameLevelCategory).IsAssignableFrom(type))
                            .ToArray());
                Dictionary<string, Type> parameters = new Dictionary<string, Type>();
                parameters["Id"] = typeof(string);
                //parameters["Scene"] = typeof(string);
                //parameters["DisplayName"] = typeof(string);
                parameters["Name"] = typeof(string);
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
                    if (!InfoResolver.FortInfo.GameLevel.GameLevelInfos.ContainsKey(id))
                        continue;
                    GameLevelInfo gameLevelInfo = InfoResolver.FortInfo.GameLevel.GameLevelInfos[id];
                    if (exportRow.ContainsParameter("Scene"))
                    {
/*                        gameLevelInfo.Scene = new FortScene();
                        gameLevelInfo.Scene.SceneName = (string)exportRow.GetValue("Scene").Value;*/
                    }
/*                    if (exportRow.ContainsParameter("DisplayName"))
                    {
                        gameLevelInfo.DisplayName = (string)exportRow.GetValue("DisplayName").Value;
                    }*/
                    if (exportRow.ContainsParameter("Name"))
                    {
                        gameLevelInfo.Name = (string)exportRow.GetValue("Name").Value;
                    }
                    exportRow.FillCustomExportParameter(gameLevelInfo);
                }
            }
            InfoResolver.FortInfo.Save();
        }
    }
}

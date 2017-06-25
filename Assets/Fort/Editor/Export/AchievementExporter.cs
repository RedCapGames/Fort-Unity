using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Fort.Info;
using Fort.Info.Achievement;
using NPOI.HSSF.UserModel;
using UnityEditor;

namespace Fort.Export
{

    public static class AchievementExporter
    {

        [MenuItem("Fort/Import-Export/Achievement/Export Achievements")]
        public static void ExportAchievements()
        {
            string path = EditorUtility.SaveFilePanel("Export Achievements", "", "", "xls");
            if (string.IsNullOrEmpty(path))
                return;
            using (Stream writer = File.Create(path))
            {
                ExportData exportData = new ExportData();
                foreach (AchievementInfo achievementInfo in InfoResolver.Resolve<FortInfo>().Achievement.AchievementInfos)
                {

                    NoneLevelBaseAchievementInfo noneLevelBaseAchievementInfo =
                        achievementInfo as NoneLevelBaseAchievementInfo;
                    if (noneLevelBaseAchievementInfo != null)
                    {
                        ExportRow exportRow = new ExportRow();
                        exportRow.AddParameter("Id", new Parameter
                        {
                            Value = noneLevelBaseAchievementInfo.Id,
                            Type = typeof (string)
                        });
                        exportRow.AddParameter("Name", new Parameter
                        {
                            Value = achievementInfo.GetType().Name,
                            Type = typeof (string)
                        });
                        exportRow.AddParameter("Score", new Parameter
                        {
                            Value = noneLevelBaseAchievementInfo.Score,
                            Type = typeof(int)
                        });
                        foreach (string valueDefenition in InfoResolver.Resolve<FortInfo>().ValueDefenitions)
                        {
                            exportRow.AddParameter(valueDefenition, new Parameter
                            {
                                Value = noneLevelBaseAchievementInfo.Balance == null ||
                                        noneLevelBaseAchievementInfo.Balance.Values == null
                                    ? 0
                                    : noneLevelBaseAchievementInfo.Balance[valueDefenition],
                                Type = typeof (int)
                            });

                        }
                        exportRow.AddCustomExportParameter(noneLevelBaseAchievementInfo);
                        exportData.AddRow(exportRow);
                    }
                    else
                    {
                        LevelBaseAchievementInfo levelBaseAchievementInfo = (LevelBaseAchievementInfo) achievementInfo;
                        AchievementLevelInfo[] achievementLevelInfos =
                            levelBaseAchievementInfo.GetAchievementLevelInfos();
                        int index = 0;
                        foreach (AchievementLevelInfo achievementLevelInfo in achievementLevelInfos)
                        {
                            ExportRow exportRow = new ExportRow();
                            exportRow.AddParameter("Id", new Parameter
                            {
                                Value = achievementLevelInfo.Id,
                                Type = typeof (string)
                            });
                            exportRow.AddParameter("Name", new Parameter
                            {
                                Value = string.Format("{0}_{1}", achievementInfo.GetType().Name, index++),
                                Type = typeof (string)
                            });
                            exportRow.AddParameter("Score", new Parameter
                            {
                                Value = achievementLevelInfo.Score,
                                Type = typeof(int)
                            });
                            foreach (string valueDefenition in InfoResolver.Resolve<FortInfo>().ValueDefenitions)
                            {
                                exportRow.AddParameter(valueDefenition, new Parameter
                                {
                                    Value = achievementLevelInfo.Balance == null ||
                                            achievementLevelInfo.Balance.Values == null
                                        ? 0
                                        : achievementLevelInfo.Balance[valueDefenition],
                                    Type = typeof (int)
                                });

                            }
                            exportRow.AddCustomExportParameter(achievementLevelInfo);
                            exportData.AddRow(exportRow);

                        }
                    }
                }
                HSSFWorkbook workbook = new HSSFWorkbook();
                
                HSSFSheet sheet = (HSSFSheet) workbook.CreateSheet("Achievements");
                exportData.SerializeToSheet(sheet);
                workbook.Write(writer);                
            }
        }

        [MenuItem("Fort/Import-Export/Achievement/Import Achievements")]
        public static void ImportAchievements()
        {
            
            string path = EditorUtility.OpenFilePanel("Import Achievements", "", "xls");
            if (string.IsNullOrEmpty(path))
                return;
            using (Stream reader = File.OpenRead(path))
            {
                IDictionary<string, PropertyInfo> customPossibleProperties =
                    ExportData.GetCustomPossibleProperties(
                        TypeHelper.GetAllTypes(AllTypeCategory.Game)
                            .Where(type => typeof (NoneLevelBaseAchievementInfo).IsAssignableFrom(type))
                            .Concat(
                                TypeHelper.GetAllTypes(AllTypeCategory.Game)
                                    .Where(type => typeof (AchievementLevelInfo).IsAssignableFrom(type)))
                            .ToArray());
                Dictionary<string,Type> parameters = new Dictionary<string, Type>();
                parameters["Id"] = typeof (string);
                parameters["Name"] = typeof (string);
                foreach (string valueDefenition in InfoResolver.Resolve<FortInfo>().ValueDefenitions)
                {
                    parameters[valueDefenition] = typeof (int);
                }
                foreach (KeyValuePair<string, PropertyInfo> pair in customPossibleProperties)
                {
                    parameters[pair.Key] = pair.Value.PropertyType;
                }
                HSSFWorkbook workbook = new HSSFWorkbook(reader);
                ExportData exportData = ExportData.DeserializeFromSheet(parameters, workbook.GetSheetAt(0));
                foreach (ExportRow exportRow in exportData.ExportRows)
                {
                    if(!exportRow.ContainsParameter("Id"))
                        continue;
                    string id = (string)exportRow.GetValue("Id").Value;
                    if(!InfoResolver.Resolve<FortInfo>().Achievement.AchievementTokens.ContainsKey(id))
                        continue;
                    AchievementToken achievementToken = InfoResolver.Resolve<FortInfo>().Achievement.AchievementTokens[id];
                    if (achievementToken.NoneLevelBase)
                    {
                        NoneLevelBaseAchievementInfo noneLevelBaseAchievementInfo = (NoneLevelBaseAchievementInfo)achievementToken.AchievementInfo;
                        if (exportRow.ContainsParameter("Score"))
                        {
                            noneLevelBaseAchievementInfo.Score = (int) exportRow.GetValue("Score").Value;
                        }
                        foreach (string valueDefenition in InfoResolver.Resolve<FortInfo>().ValueDefenitions)
                        {
                            if (exportRow.ContainsParameter(valueDefenition))
                            {
                                noneLevelBaseAchievementInfo.Balance[valueDefenition] =
                                    (int) exportRow.GetValue(valueDefenition).Value;
                            }
                        }
                        exportRow.FillCustomExportParameter(noneLevelBaseAchievementInfo);
                    }
                    else
                    {
                        AchievementLevelInfo achievementLevelInfo = achievementToken.AchievementLevelInfo;
                        if (exportRow.ContainsParameter("Score"))
                        {
                            achievementLevelInfo.Score = (int)exportRow.GetValue("Score").Value;
                        }
                        foreach (string valueDefenition in InfoResolver.Resolve<FortInfo>().ValueDefenitions)
                        {
                            if (exportRow.ContainsParameter(valueDefenition))
                            {
                                achievementLevelInfo.Balance[valueDefenition] =
                                    (int)exportRow.GetValue(valueDefenition).Value;
                            }
                        }
                        exportRow.FillCustomExportParameter(achievementLevelInfo);

                    }
                }
            }
            InfoResolver.Resolve<FortInfo>().Save();
        }
    }
}

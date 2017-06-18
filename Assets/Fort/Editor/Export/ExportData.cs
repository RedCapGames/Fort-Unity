using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Fort.Info;
using Newtonsoft.Json;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Fort.Export
{
    public class ExportRow
    {
        #region Fields

        private readonly Dictionary<string, Parameter> _parameters = new Dictionary<string, Parameter>();

        #endregion

        #region Properties

        public string[] ParameterNames
        {
            get { return _parameters.Keys.ToArray(); }
        }

        #endregion

        #region  Public Methods

        public void AddParameter(string parameter, Parameter value)
        {
            _parameters[parameter] = value;
        }

        public Parameter GetValue(string parameterName)
        {
            return _parameters[parameterName];
        }

        public bool ContainsParameter(string parameterName)
        {
            return _parameters.ContainsKey(parameterName);
        }
        public void AddCustomExportParameter(object obj)
        {
            PropertyInfo[] propertyInfos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                CustomExportAttribute customExportAttribute = propertyInfo.GetCustomAttribute<CustomExportAttribute>();
                if (customExportAttribute != null)
                {
                    string displayName = string.IsNullOrEmpty(customExportAttribute.DisplayName)
                        ? propertyInfo.Name
                        : customExportAttribute.DisplayName;
                    if (customExportAttribute.AddType)
                        displayName = string.Format("{0}({1})", obj.GetType().Name, displayName);
                    AddParameter(displayName,new Parameter
                    {
                        Type = propertyInfo.PropertyType,
                        Value = propertyInfo.GetValue(obj,new object[0])
                    });
                }
            }
        }

        public void FillCustomExportParameter(object obj)
        {
            PropertyInfo[] propertyInfos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                CustomExportAttribute customExportAttribute = propertyInfo.GetCustomAttribute<CustomExportAttribute>();
                if (customExportAttribute != null)
                {
                    string displayName = string.IsNullOrEmpty(customExportAttribute.DisplayName)
                        ? propertyInfo.Name
                        : customExportAttribute.DisplayName;
                    if (customExportAttribute.AddType)
                        displayName = string.Format("{0}({1})", obj.GetType().Name, displayName);
                    if (ContainsParameter(displayName))
                    {

                        try
                        {
                            propertyInfo.SetValue(obj, GetValue(displayName).Value, new object[0]);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                }
            }
        }
        #endregion
    }

    public class ExportData
    {
        #region Fields

        private readonly List<ExportRow> _exports = new List<ExportRow>();

        #endregion

        #region Properties

        public ExportRow[] ExportRows
        {
            get { return _exports.ToArray(); }
        }

        #endregion

        #region  Public Methods

        public void AddRow(ExportRow exportRow)
        {
            _exports.Add(exportRow);
        }

        public string SerializeToCsv()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            foreach (ExportRow exportRow in _exports)
            {
                foreach (string parameterName in exportRow.ParameterNames)
                {
                    parameters[parameterName] = parameterName;
                }
            }
            string[] param = parameters.Select(pair => pair.Key).ToArray();
            string header = string.Empty;
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < param.Length; i++)
            {
                if (i < param.Length - 1)
                    header += string.Format("{0},", param[i]);
                else
                    header += string.Format("{0}", param[i]);
            }
            builder.AppendLine(header);

            foreach (ExportRow exportRow in _exports)
            {
                string row = string.Empty;
                for (int i = 0; i < param.Length; i++)
                {
                    string format;
                    if (i < param.Length - 1)
                        format = "{0},";
                    else
                        format = "{0},";
                    row += string.Format(format,
                        exportRow.ContainsParameter(param[i])
                            ? SerializeObject(exportRow.GetValue(param[i]))
                            : string.Empty);
                }
                builder.AppendLine(row);
            }

            return builder.ToString();
        }

        public static ExportData DeserializeFromCsv(IDictionary<string, Type> parameters, TextReader reader)
        {
            //Read header
            Dictionary<string, int> parameterIndecies = new Dictionary<string, int>();
            string header = reader.ReadLine();
            if (string.IsNullOrEmpty(header))
                return new ExportData();
            string[] headers = header.Split(',');
            for (int i = 0; i < headers.Length; i++)
            {
                if (!string.IsNullOrEmpty(headers[i]) && parameters.ContainsKey(headers[i]))
                    parameterIndecies[headers[i]] = i;
            }
            ExportData exportData = new ExportData();
            while (true)
            {
                string row = reader.ReadLine();
                if (string.IsNullOrEmpty(row))
                    return exportData;
                string[] values = row.Split(',');
                ExportRow exportRow = new ExportRow();
                foreach (KeyValuePair<string, int> pair in parameterIndecies)
                {
                    if (pair.Value < values.Length)
                    {
                        exportRow.AddParameter(pair.Key, new Parameter
                        {
                            Type = parameters[pair.Key],
                            Value = DeserializeObject(values[pair.Value], parameters[pair.Key])
                        });
                    }
                }
                exportData._exports.Add(exportRow);
            }
        }

        public static IDictionary<string, PropertyInfo> GetCustomPossibleProperties(Type[] possibleTypes)
        {
            Dictionary<string, PropertyInfo> result = new Dictionary<string, PropertyInfo>();
            foreach (Type possibleType in possibleTypes)
            {
                PropertyInfo[] propertyInfos = possibleType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    CustomExportAttribute customExportAttribute = propertyInfo.GetCustomAttribute<CustomExportAttribute>();
                    if (customExportAttribute != null)
                    {
                        string displayName = string.IsNullOrEmpty(customExportAttribute.DisplayName)
                            ? propertyInfo.Name
                            : customExportAttribute.DisplayName;
                        if (customExportAttribute.AddType)
                            displayName = string.Format("{0}({1})", possibleType.Name, displayName);
                        result[displayName] = propertyInfo;
                    }
                }
            }
            return result;
        }

        #endregion

        #region Private Methods

        private string SerializeObject(Parameter parameter)
        {
            if (typeof (Object).IsAssignableFrom(parameter.Type))
                return AssetDatabase.GetAssetPath((Object) parameter.Value);
            if (parameter.Type == typeof (string))
            {
                if (parameter.Value == null)
                    return string.Empty;
                return (string)parameter.Value;
            }
                
            return JsonConvert.SerializeObject(parameter.Value);
        }

        private static object DeserializeObject(string graph, Type type)
        {
            if (typeof (Object).IsAssignableFrom(type))
            {
                return AssetDatabase.LoadAssetAtPath(graph, type);
            }
            try
            {
                if (type == typeof (string))
                    return graph;
                object result = JsonConvert.DeserializeObject(graph, type);
                return result;
            }
            catch (Exception)
            {
                return type.GetDefault();
            }
        }

        #endregion
    }

    public class Parameter
    {
        #region Properties

        public Type Type { get; set; }
        public object Value { get; set; }

        #endregion
    }
}
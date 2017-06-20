using System;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Fort
{
    public class EditorFortEncryptionKey: IFortEncryptionKey
    {
        private EncryptionData _encryptionData;
        #region Implementation of IFortEncryptionKey

        public byte ChangeData(byte data)
        {
            Initialize();
            return (byte)(data ^ _encryptionData.Change);
        }

        public byte[] ResolveKey()
        {
            Initialize();
            return _encryptionData.Keys;
        }

        private void Initialize()
        {
            TextAsset encryptionTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Fort/Editor/EditorEncryption.json");
            if (encryptionTextAsset != null)
            {
                _encryptionData = JsonConvert.DeserializeObject<EncryptionData>(encryptionTextAsset.text);
            }
            else
            {
                throw new Exception("Assets/Fort/Editor/EditorEncryption.json Cannot be found.");
            }
        }

        #endregion
    }

    class EncryptionData
    {
        public byte Change { get; set; }
        public byte[] Keys { get; set; }
    }
}

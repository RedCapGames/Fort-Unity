using System;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Fort
{
    public class EditorFortEncryptionKey: IFortEncryptionKey
    {
        private EncryptionData _encryptionData;
        private TextAsset _encryptionTextAsset;

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
            if(_encryptionTextAsset != null)
                return;
            _encryptionTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Fort/Editor/EditorEncryption.json");
            if (_encryptionTextAsset != null)
            {
                _encryptionData = JsonConvert.DeserializeObject<EncryptionData>(_encryptionTextAsset.text);
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

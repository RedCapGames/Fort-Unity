using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Fort
{
    public interface IFortEncryptionKey
    {
        byte ChangeData(byte data);
        byte[] ResolveKey();
    }

    public class GameFortEncryptionKey: IFortEncryptionKey
    {
        [DllImport("native")]
        public static extern byte Change(byte data);
        [DllImport("native")]
        private static extern byte GetEncoding0();
        [DllImport("native")]
        private static extern byte GetEncoding1();
        [DllImport("native")]
        private static extern byte GetEncoding2();
        [DllImport("native")]
        private static extern byte GetEncoding3();
        [DllImport("native")]
        private static extern byte GetEncoding4();
        [DllImport("native")]
        private static extern byte GetEncoding5();
        [DllImport("native")]
        private static extern byte GetEncoding6();
        [DllImport("native")]
        private static extern byte GetEncoding7();
        [DllImport("native")]
        private static extern byte GetEncoding8();
        [DllImport("native")]
        private static extern byte GetEncoding9();
        [DllImport("native")]
        private static extern byte GetEncoding10();
        [DllImport("native")]
        private static extern byte GetEncoding11();
        [DllImport("native")]
        private static extern byte GetEncoding12();
        [DllImport("native")]
        private static extern byte GetEncoding13();
        [DllImport("native")]
        private static extern byte GetEncoding14();

        [DllImport("native")]
        private static extern byte GetEncoding15();

        #region Implementation of IFortEncryptionKey

        public byte ChangeData(byte data)
        {
            return Change(data);
        }

        public byte[] ResolveKey()
        {
            return new[]{
                GetEncoding0(), GetEncoding1(), GetEncoding2(), GetEncoding3(), GetEncoding4(), GetEncoding5(),
                GetEncoding6(), GetEncoding7(), GetEncoding8(), GetEncoding9(), GetEncoding10(), GetEncoding11(),
                GetEncoding12(), GetEncoding13(), GetEncoding14(), GetEncoding15()
            };
        }

        #endregion
    }
    public static class FortEncryptionKey
    {
        private static IFortEncryptionKey _fortEncryptionKey;

        private static void Initialize()
        {
            if (_fortEncryptionKey != null)
                return;
            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.OSXEditor)
            {
                Type fortEncryptionType =
                    TypeHelper.GetAllTypes(AllTypeCategory.Editor)
                        .Single(
                            type =>
                                string.Format("{0}.{1}", type.Namespace, type.Name) == "Fort.EditorFortEncryptionKey");
                _fortEncryptionKey = (IFortEncryptionKey) Activator.CreateInstance(fortEncryptionType);
            }
            else
            {
                _fortEncryptionKey = new GameFortEncryptionKey();
            }
            
        }
        public static byte Change(byte data)
        {
            Initialize();
            return _fortEncryptionKey.ChangeData(data);
        }
        public static byte[] ResolveKey()
        {
            Initialize();            
            return _fortEncryptionKey.ResolveKey();
        }


    }
}
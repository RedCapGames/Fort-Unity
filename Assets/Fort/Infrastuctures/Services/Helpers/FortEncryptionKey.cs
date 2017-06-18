namespace Fort
{
    public static class FortEncryptionKey
    {
#if !UNITY_EDITOR && UNITY_ANDROID
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

        public static byte[] ResolveKey()
        {
            return new[]
            {
                GetEncoding0(), GetEncoding1(), GetEncoding2(), GetEncoding3(), GetEncoding4(), GetEncoding5(),
                GetEncoding6(), GetEncoding7(), GetEncoding8(), GetEncoding9(), GetEncoding10(), GetEncoding11(),
                GetEncoding12(), GetEncoding13(), GetEncoding14(), GetEncoding15()
            };
        }
#endif
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        public static byte Change(byte data)
        {
            return (byte)(data ^ 0xdd);
        }
        public static byte[] ResolveKey()
        {
            //return Encoding.ASCII.GetBytes("ArashJafarzadeh");
            return new byte[] { 39, 118, 0, 86, 77, 226, 202, 65, 128, 8, 232, 62, 229, 16, 22, 125 };
        }

#endif
#if UNITY_EDITOR
        public static byte Change(byte data)
        {
            return (byte)(data ^ 0xdd);
        }
        public static byte[] ResolveKey()
        {
            //return Encoding.ASCII.GetBytes("ArashJafarzadeh");
            return new byte[] { 39, 118, 0, 86, 77, 226, 202, 65, 128, 8, 232, 62, 229, 16, 22, 125 };
        }
# else
        public static byte Change(byte data)
        {
            return (byte)(data ^ 0xdd);
        }
        public static byte[] ResolveKey()
        {
            //return Encoding.ASCII.GetBytes("ArashJafarzadeh");
            return new byte[] { 39, 118, 0, 86, 77, 226, 202, 65, 128, 8, 232, 62, 229, 16, 22, 125 };
        }
#endif

    }
}
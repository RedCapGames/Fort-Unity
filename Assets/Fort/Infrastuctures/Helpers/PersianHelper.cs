using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fort
{
    public static class PersianHelper
    {
        #region Fields

        private static readonly Dictionary<char, char> _inverserCharacterMap;
        private static readonly Dictionary<char, CharacterMapInfo> characterMapInfos;

        #endregion

        #region Constructors

        static PersianHelper()
        {
            string seperateSource = "ا با و بو د بد ذ بذ ر بر ز بز ة بة ژ بژ آ بآ أ بأ إ بإ ؤ بؤ";
            string seperateDestination = "ﺆﺑ ﺅ ﺈﺑ ﺇ ﺄﺑ ﺃ ﺂﺑ ﺁ ﮋﺑ ﮊ ﺔﺑ ﺓ ﺰﺑ ﺯ ﺮﺑ ﺭ ﺬﺑ ﺫ ﺪﺑ ﺩ ﻮﺑ ﻭ ﺎﺑ ﺍ";

            string attachedSource =
                "چ چچچ ج ججج ح ححح خ خخخ ه ههه ع ععع غ غغغ ف ففف ق ققق ث ثثث ص صصص ض ضضض گ گگگ ک ککک م ممم ت تتت ل للل ب ببب ی ییی س سسس ش ششش پ پپپ ط ططط ظ ظظظ ك ككك ي ييي ئ ئئئ ن ننن";
            string attachedDestination =
                "ﻦﻨﻧ ﻥ ﺊﺌﺋ ﺉ ﻲﻴﻳ ﻱ ﻚﻜﻛ ﻙ ﻆﻈﻇ ﻅ ﻂﻄﻃ ﻁ ﭗﭙﭘ ﭖ ﺶﺸﺷ ﺵ ﺲﺴﺳ ﺱ ﯽﯿﯾ ﯼ ﺐﺒﺑ ﺏ ﻞﻠﻟ ﻝ ﺖﺘﺗ ﺕ ﻢﻤﻣ ﻡ ﮏﮑﮐ ﮎ ﮓﮕﮔ ﮒ ﺾﻀﺿ ﺽ ﺺﺼﺻ ﺹ ﺚﺜﺛ ﺙ ﻖﻘﻗ ﻕ ﻒﻔﻓ ﻑ ﻎﻐﻏ ﻍ ﻊﻌﻋ ﻉ ﻪﻬﻫ ﻩ ﺦﺨﺧ ﺥ ﺢﺤﺣ ﺡ ﺞﺠﺟ ﺝ ﭻﭽﭼ ﭺ";

            string digitSource = "۱۲۳۴۵۶۷۸۹۰";
            //string digitDestination = "۱۲۳۴۵۶۷۸۹۰";

            characterMapInfos = new Dictionary<char, CharacterMapInfo>();
            string[] attachedSourceSeperated = attachedSource.SplitInParts(6).Select(s => s.TrimEnd()).ToArray();
            string[] attachedDestinationSeperated =
                attachedDestination.SplitInParts(6).Select(s => s.TrimEnd()).ToArray();
            for (int i = 0; i < attachedSourceSeperated.Length; i++)
            {
                string attachedDestinationSingle =
                    attachedDestinationSeperated[(attachedSourceSeperated.Length - 1) - i];
                characterMapInfos.Add(attachedSourceSeperated[i][0], new CharacterMapInfo
                {
                    Base = attachedSourceSeperated[i][0],
                    First = attachedDestinationSingle[2],
                    Midle = attachedDestinationSingle[1],
                    Single = attachedDestinationSingle[4],
                    End = attachedDestinationSingle[0],
                    IsCharacterBreaker = false
                });
            }
            string[] seperateSourceSeperated = seperateSource.SplitInParts(5).Select(s => s.TrimEnd()).ToArray();
            string[] seperateDestinationSeperated =
                seperateDestination.SplitInParts(5).Select(s => s.TrimEnd()).ToArray();
            for (int i = 0; i < seperateSourceSeperated.Length; i++)
            {
                string seperateDestinationSingle =
                    seperateDestinationSeperated[(seperateSourceSeperated.Length - 1) - i];
                characterMapInfos.Add(seperateSourceSeperated[i][0], new CharacterMapInfo
                {
                    Base = seperateSourceSeperated[i][0],
                    First = seperateDestinationSingle[3],
                    Midle = seperateDestinationSingle[0],
                    Single = seperateDestinationSingle[3],
                    End = seperateDestinationSingle[0],
                    IsCharacterBreaker = true
                });
            }
            //byte[] bytes = Encoding.Unicode.GetBytes(characterMapInfos.Select(pair => pair.Value.First).ToArray());
            char[] specialCharacters = Enumerable.Range(0, 224).Select(i => Convert.ToChar(i)).ToArray();
            foreach (char specialCharacter in specialCharacters)
            {
                characterMapInfos.Add(specialCharacter, new CharacterMapInfo
                {
                    Base = specialCharacter,
                    First = specialCharacter,
                    Midle = specialCharacter,
                    Single = specialCharacter,
                    End = specialCharacter,
                    IsCharacterBreaker = true,
                    IsSpace = true
                });
            }
            foreach (char digitchar in digitSource)
            {
                characterMapInfos.Add(digitchar, new CharacterMapInfo
                {
                    Base = digitchar,
                    First = digitchar,
                    Midle = digitchar,
                    Single = digitchar,
                    End = digitchar,
                    IsCharacterBreaker = true,
                    IsSpace = true
                });
            }
            characterMapInfos.Add('‌', new CharacterMapInfo
            {
                Base = ' ',
                First = ' ',
                Midle = ' ',
                Single = ' ',
                End = ' ',
                IsCharacterBreaker = true,
                IsSpace = true
            });
            //Arabic Characters
            characterMapInfos.Add('ى', characterMapInfos['ی']);

            _inverserCharacterMap = new Dictionary<char, char>();
            foreach (KeyValuePair<char, CharacterMapInfo> characterMapInfo in characterMapInfos)
            {
                _inverserCharacterMap[characterMapInfo.Value.First] = characterMapInfo.Key;
                _inverserCharacterMap[characterMapInfo.Value.End] = characterMapInfo.Key;
                _inverserCharacterMap[characterMapInfo.Value.Midle] = characterMapInfo.Key;
                _inverserCharacterMap[characterMapInfo.Value.Single] = characterMapInfo.Key;
            }
        }

        #endregion

        #region  Public Methods

        public static bool ValidPersian(this string source)
        {
            return source.All(c => characterMapInfos.ContainsKey(c));
        }

        public static string Persian(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            source = new string(source.Where(c => characterMapInfos.ContainsKey(c)).ToArray());
            try
            {
                char[] convertedData = new char[source.Length];
                for (int i = 0; i < source.Length; i++)
                {
                    if (!characterMapInfos.ContainsKey(source[i]))
                    {
                        continue;
                    }
                    if (i == 0)
                    {
                        if (source.Length > 1 && !characterMapInfos[source[i + 1]].IsSpace)
                        {
                            convertedData[i] = characterMapInfos[source[i]].First;
                        }
                        else
                        {
                            convertedData[i] = characterMapInfos[source[i]].Single;
                        }
                    }
                    else if (i == source.Length - 1)
                    {
                        if (characterMapInfos[source[i - 1]].IsCharacterBreaker)
                        {
                            convertedData[i] = characterMapInfos[source[i]].Single;
                        }
                        else
                        {
                            convertedData[i] = characterMapInfos[source[i]].End;
                        }
                    }
                    else
                    {
                        if (characterMapInfos[source[i - 1]].IsCharacterBreaker &&
                            characterMapInfos[source[i + 1]].IsSpace)
                        {
                            convertedData[i] = characterMapInfos[source[i]].Single;
                        }
                        else if (!characterMapInfos[source[i - 1]].IsCharacterBreaker &&
                                 characterMapInfos[source[i + 1]].IsSpace)
                        {
                            convertedData[i] = characterMapInfos[source[i]].End;
                        }
                        else if (characterMapInfos[source[i - 1]].IsCharacterBreaker &&
                                 !characterMapInfos[source[i + 1]].IsSpace)
                        {
                            convertedData[i] = characterMapInfos[source[i]].First;
                        }
                        else if (!characterMapInfos[source[i - 1]].IsCharacterBreaker &&
                                 !characterMapInfos[source[i + 1]].IsSpace)
                        {
                            convertedData[i] = characterMapInfos[source[i]].Midle;
                        }
                    }
                }
                List<TextPart> textParts = GetTextParts(source.ToCharArray(), convertedData, false);
                convertedData =
                    textParts.Select(part => part)
                        .Reverse()
                        .SelectMany(part => part.Invert ? part.Text.Reverse() : part.Text)
                        .ToArray();
                return new string(convertedData);
            }
            catch (Exception e)
            {
                Debug.Log(source);
                Debug.LogError(e);
                return string.Empty;
            }
        }

        public static string DePersian(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            List<TextPart> textParts = GetTextParts(source.Select(c =>
            {
                if (_inverserCharacterMap.ContainsKey(c))
                    return _inverserCharacterMap[c];
                return c;
            }).ToArray(), source.ToCharArray(), true);

            return
                new string(
                    textParts.Select(part => part)
                        .Reverse()
                        .SelectMany(part => part.Invert ? part.Text.Reverse() : part.Text)
                        .ToArray());
            //return new string().Reverse().ToArray());
        }

        public static string SingleLine(this string source)
        {
            source = source.Replace("\n", string.Empty);
            source = source.Replace("\r", string.Empty);
            return source.Replace("\t", string.Empty);
        }

        #endregion

        #region Private Methods

        private static IEnumerable<string> SplitInParts(this string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", "partLength");

            for (int i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        private static List<TextPart> GetTextParts(char[] source, char[] convertedData, bool revert)
        {
            List<TextPart> textParts = new List<TextPart>();
            if (convertedData.Length > 0)
            {
                TextPart textPart = new TextPart();
                if (!characterMapInfos.ContainsKey(source[0]))
                {
                    Debug.Log("");
                }
                textPart.Invert = !characterMapInfos[source[0]].IsSpace;
                List<char> characters = new List<char>();
                characters.Add(revert ? source[0] : convertedData[0]);
                for (int i = 1; i < convertedData.Length; i++)
                {
                    bool inverted = !characterMapInfos[source[i]].IsSpace;
                    if (inverted != textPart.Invert)
                    {
                        textPart.Text = characters.ToArray();
                        textParts.Add(textPart);
                        textPart = new TextPart();
                        textPart.Invert = inverted;
                        characters = new List<char>();
                        characters.Add(revert ? source[i] : convertedData[i]);
                    }
                    else
                    {
                        characters.Add(revert ? source[i] : convertedData[i]);
                    }
                }
                textPart.Text = characters.ToArray();
                textParts.Add(textPart);
            }
            return textParts;
        }

        #endregion

        #region Nested types

        public class CharacterMapInfo
        {
            #region Properties

            public char Base { get; set; }
            public char First { get; set; }
            public char Midle { get; set; }
            public char End { get; set; }
            public char Single { get; set; }
            public bool IsCharacterBreaker { get; set; }
            public bool IsSpace { get; set; }

            #endregion
        }

        private class TextPart
        {
            #region Properties

            public char[] Text { get; set; }
            public bool Invert { get; set; }

            #endregion
        }

        #endregion
    }
}
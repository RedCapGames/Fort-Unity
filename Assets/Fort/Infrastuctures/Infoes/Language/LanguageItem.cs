using System;
using System.Collections.Generic;
using Fort.Inspector;

namespace Fort.Info.Language
{
    [Inspector(Presentation = "Fort.CustomEditor.LanguageItemPresentation")]
    public abstract class LanguageItem
    {
        internal LanguageItem()
        {
            Id = Guid.NewGuid().ToString();

        }

        [IgnorePresentation]
        public string Id { get; set; }
    }
    public abstract class LanguageItem<T>: LanguageItem
    {
        public abstract T Value { get; }

        public static implicit operator T(LanguageItem<T> languageItem)
        {
            return languageItem == null ? default(T) : languageItem.Value;
        }


        #region Overrides of Object

        public override string ToString()
        {
            T value = Value;
            if (Equals(value, default(T)))
                return string.Empty;
            return value.ToString();
        }

        #endregion
    }

    public interface IInfoLanguageItem
    {
        bool UseOverridedValue { get; set; }
        object GetOvverideValue();
        void SetOvverideValue(object value);
    }
    public sealed class InfoLanguageItem<T> : LanguageItem<T>, IInfoLanguageItem
    {
        #region Overrides of LanguageItem<T>

        public override T Value
        {
            get
            {
                if (UseOverridedValue)
                    return OverridedValue;
                LanguageInfo activeLanguage = ServiceLocator.Resolve<ILanguageService>().GetActiveLanguage();
                if (activeLanguage == null)
                    return default(T);
                if (!activeLanguage.LanguageDatas.ContainsKey(Id))
                    return default(T);
                return (T)activeLanguage.LanguageDatas[Id];
            }
        }

        #endregion

        public T OverridedValue { get; set; }
        public bool UseOverridedValue { get; set; }
        public object GetOvverideValue()
        {
            return OverridedValue;
        }

        public void SetOvverideValue(object value)
        {
            OverridedValue = (T) value;
        }
    }

    public sealed class CustomLanguageItem<T> : LanguageItem<T>
    {
        private readonly IDictionary<string, T> _items;

        public CustomLanguageItem(IDictionary<string,T> items)
        {
            _items = items;
        }

        #region Overrides of LanguageItem<T>

        public override T Value
        {
            get
            {
                LanguageInfo activeLanguage = ServiceLocator.Resolve<ILanguageService>().GetActiveLanguage();
                if (activeLanguage == null)
                    return default(T);
                if(!_items.ContainsKey(activeLanguage.Id))
                    return default(T);
                return _items[activeLanguage.Id];
            }
        }

        #endregion
    }
}
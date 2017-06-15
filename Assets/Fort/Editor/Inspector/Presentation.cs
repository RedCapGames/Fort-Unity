using System;
using System.Linq;
using System.Reflection;
using Fort;

namespace Fort.Inspector
{
    public abstract class Presentation
    {        
        public abstract PresentationResult OnInspectorGui(PresentationParamater parameter);
    }

    public class Change
    {
        public Change()
        {
            ChildrenChange = new Change[0];
        }
        public bool IsPresentationChanged { get; set; }
        public bool IsDataChanged { get; set; }
        public Change[] ChildrenChange { get; set; }

        public bool IsAnyPresentationChanged()
        {
            if (IsPresentationChanged)
                return true;
            if (ChildrenChange != null && ChildrenChange.Any(change => change.IsAnyPresentationChanged()))
                return true;
            return false;
        }
        public bool IsAnyDataChanged()
        {
            if (IsDataChanged)
                return true;
            if (ChildrenChange != null && ChildrenChange.Any(change => change.IsAnyDataChanged()))
                return true;
            return false;
        }
    }

    public class PresentationResult
    {
        public object Result { get; set; }
        public Change Change { get; set; }
        public object PresentationData { get; set; }
    }

    public class PresentationSite
    {
        public PresentationSite BaseSite { get; set; }
        public Presentation BasePresentation { get; set; }
        public PresentationSiteType SiteType { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public object Base { get; set; }
    }

    public enum PresentationSiteType
    {
        Property,
        ArrayElement,
        None
    }

    public class PresentationParamater
    {
        public PresentationParamater(object instance, object presentationData, string title, Type dataType, PresentationSite presentationSite,FortInspector fortInspector)
        {
            Instance = instance;
            PresentationData = presentationData;
            PresentationSite = presentationSite;
            FortInspector = fortInspector;
            Title = title;
            DataType = dataType;
        }
        public Type DataType { get; private set; }
        public string Title { get; private set; }
        public object Instance { get; private set; }
        public object PresentationData { get; private set; }
        public PresentationSite PresentationSite { get; private set; }
        public FortInspector FortInspector { get; private set; }
    }

}
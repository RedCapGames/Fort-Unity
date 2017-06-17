﻿using System;
using System.Linq;
using UnityEditor;

namespace Fort.Inspector
{
    public abstract class FortInspector : Editor
    {

        
        private object _presentationData;
        private Presentation _presentation;
        private Type _targetType;
        private object _target;
        private bool _repaintOnFinish;
        public object Targer { get { return _target; } }
        public virtual IPresentationResolver GetResolver()
        {
            return new PresentationResolver();
        }
        public void RepaintOnFinish()
        {
            _repaintOnFinish = true;
        }
        private Type FindGenericFortScriptableObjectInParent(Type baseType)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof (FortScriptableObject<>))
                return baseType;
            if(baseType.BaseType == null)
                return null;
            return FindGenericFortScriptableObjectInParent(baseType.BaseType);
        }

        private void Initialize()
        {
            if(_presentation != null)
                return;
            _targetType = FindGenericFortScriptableObjectInParent(target.GetType()).GetGenericArguments().First();
            _target = ((FortScriptableObject) target).Load(_targetType);
            OnTargetChanged(_target);
            _presentationData = ((FortScriptableObject) target).LoadPresentationData();
            _presentation = new ConcretePresentation();
           
        }

        #region Overrides of Editor

        public override void OnInspectorGUI()
        {
            Initialize();
            PresentationParamater paramater = new PresentationParamater(_target, _presentationData, string.Empty, _targetType, new PresentationSite { BaseSite = null, BasePresentation = null, Base = null, PropertyInfo = null, SiteType = PresentationSiteType.None }, this);
            PresentationResult presentationResult = _presentation.OnInspectorGui(paramater);
            bool isChanged = false;
            if (presentationResult.Change.IsAnyPresentationChanged())
            {
                ((FortScriptableObject)target).SavePresentationData(presentationResult.PresentationData);
                _presentationData = presentationResult.PresentationData;
                isChanged = true;
            }
            if (presentationResult.Change.IsAnyDataChanged())
            {
                ((FortScriptableObject)target).Save(_target);
                isChanged = true;
                OnTargetChanged(_target);
            }
            if(isChanged)
                EditorUtility.SetDirty(target);

            if (_repaintOnFinish)
            {
                _repaintOnFinish = false;
                Repaint();
            }
        }

        #endregion

        protected virtual void OnTargetChanged(object targetObject)
        {
            
        }
    }
    

}

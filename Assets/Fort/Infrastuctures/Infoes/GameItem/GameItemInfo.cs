using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Fort.Inspector;

namespace Fort.Info
{
    [AbstractTypeChildResolver(typeof(GameItemAbstractTypeChildResolver))]
    public abstract class GameItemInfo
    {
    }
    public class GameItemAbstractTypeChildResolver: IAbstractTypeChildResolver
    {
        #region Implementation of IAbstractTypeChildResolver

        public Type[] ResolveChildrenType(Type baseType, PropertyInfo propertyInfo)
        {
            Type directGameItemGenericType = typeof (UnityEngine.Object);
            if (propertyInfo != null)
            {
                GameItemFilterAttribute gameItemFilterAttribute = propertyInfo.GetCustomAttribute<GameItemFilterAttribute>();
                if (gameItemFilterAttribute != null)
                {
                    directGameItemGenericType = gameItemFilterAttribute.ObjectType;
                }
            }
            return new[] {typeof (ResourceGameItem),typeof(AssetBundleGameItem),typeof(DirectGameItem<>).MakeGenericType(directGameItemGenericType) };
        }

        #endregion
    }

    public class GameItemFilterAttribute : Attribute
    {
        public Type ObjectType { get; private set; }

        public GameItemFilterAttribute(Type objectType)
        {
            ObjectType = objectType;
        }
    }
}

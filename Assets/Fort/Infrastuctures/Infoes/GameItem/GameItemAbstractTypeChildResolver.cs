using System;
using System.Reflection;
using Fort.Inspector;
using Object = UnityEngine.Object;

namespace Fort.Info.GameItem
{
    public class GameItemAbstractTypeChildResolver: IAbstractTypeChildResolver
    {
        #region Implementation of IAbstractTypeChildResolver

        public Type[] ResolveChildrenType(Type baseType, PropertyInfo propertyInfo)
        {
            Type directGameItemGenericType = typeof (Object);
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
}
using System;
using System.Linq;
using System.Reflection;
using Fort.Inspector;
using Object = UnityEngine.Object;

namespace Fort.Info.GameItem
{
    public class GameItemAbstractTypeChildResolver: IAbstractTypeChildResolver
    {
        #region Implementation of IAbstractTypeChildResolver

        public Type[] ResolveChildrenType(Type baseType, PropertyInfo[] propertyInfos)
        {
            Type directGameItemGenericType = typeof (Object);
            if (propertyInfos != null)
            {
                
                GameItemFilterAttribute gameItemFilterAttribute = propertyInfos.Select(info => info.GetCustomAttribute<GameItemFilterAttribute>()).FirstOrDefault(attribute => attribute != null);
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
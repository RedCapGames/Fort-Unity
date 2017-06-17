using System;

namespace Fort.Info.GameItem
{
    public class GameItemFilterAttribute : Attribute
    {
        public Type ObjectType { get; private set; }

        public GameItemFilterAttribute(Type objectType)
        {
            ObjectType = objectType;
        }
    }
}
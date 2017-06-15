using UnityEngine;

namespace Fort.Info
{
    public sealed class DirectGameItem<T> : GameItemInfo where T : Object
    {
        public T GameItem { get; set; }
    }
}
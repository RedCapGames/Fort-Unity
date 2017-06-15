using System;
using UnityEngine;
using System.Collections;

namespace Fort.Inspector
{
    public class InspectorAttribute : Attribute
    {
        public string Presentation { get; set; }
    }
}

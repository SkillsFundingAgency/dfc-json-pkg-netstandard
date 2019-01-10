﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DFC.JSON.Standard
{
    public interface IJsonHelper
    {
        string SerializeObjectAndRenameIdProperty<T>(T resource, string idName, string newIdName);
        string SerializeObjectsAndRenameIdProperty<T>(List<T> resource, string idName, string newIdName);
        void RenameProperty(JToken token, string newName);
        void UpdatePropertyValue(JToken token, object value);
    }
}
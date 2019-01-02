using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DFC.JSON.Core
{
    public static class JsonExtension
    {
        public static string SerializeObjectAndRenameIdProperty<T>(T resource, string idName, string newIdName)
        {
            var json = JsonConvert.SerializeObject(resource);
            var resourceJObject = JObject.Parse(json);

            if (!resourceJObject.HasValues)
                return json;

            var prop = resourceJObject.Property(idName);
            RenameProperty(prop, newIdName);

            return resourceJObject.ToString();
        }

        public static string SerializeObjectsAndRenameIdProperty<T>(List<T> resource, string idName, string newIdName)
        {
            var json = JsonConvert.SerializeObject(resource);
            var tokens = JArray.Parse(json);

            foreach (var jToken in tokens)
            {
                var item = (JObject)jToken;

                if (item == null)
                    continue;

                var prop = item.Property(idName);
                RenameProperty(prop, newIdName);
            }

            return tokens.ToString();
        }

        public static void RenameProperty(JToken token, string newName)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token), "Cannot rename a null token");

            JProperty property;

            if (token.Type == JTokenType.Property)
            {
                if (token.Parent == null)
                    throw new InvalidOperationException("Cannot rename a property with no parent");

                property = (JProperty)token;
            }
            else
            {
                if (token.Parent == null || token.Parent.Type != JTokenType.Property)
                    throw new InvalidOperationException("This token's parent is not a JProperty; cannot rename");

                property = (JProperty)token.Parent;
            }

            var newProperty = new JProperty(newName, property.Value);
            property.Replace(newProperty);
        }

        public static void UpdatePropertyValue(JToken token, object value)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token), "Cannot rename a null token");

            JProperty property;

            if (token.Type == JTokenType.Property)
            {
                if (token.Parent == null)
                    throw new InvalidOperationException("Cannot rename a property with no parent");

                property = (JProperty)token;
            }
            else
            {
                if (token.Parent == null || token.Parent.Type != JTokenType.Property)
                    throw new InvalidOperationException("This token's parent is not a JProperty; cannot rename");

                property = (JProperty)token.Parent;
            }

            var newProperty = new JProperty(property.Name, value);
            property.Replace(newProperty);
        }
    }
}
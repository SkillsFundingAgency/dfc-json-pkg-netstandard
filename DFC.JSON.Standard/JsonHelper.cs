using System;
using System.Collections.Generic;
using DFC.JSON.Standard.Attributes;
using DFC.JSON.Standard.ContractResolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DFC.JSON.Standard
{
    public class JsonHelper : IJsonHelper
    {
        public string SerializeObjectAndRenameIdProperty<T>(T resource, string idName, string newIdName)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new IgnoreSerializerContractResolver<JsonIgnoreOnSerialize>()
            };

            var json = JsonConvert.SerializeObject(resource, settings);

            if(string.IsNullOrEmpty(json))
                return string.Empty;

            var resourceJObject = JObject.Parse(json);

            if (!resourceJObject.HasValues)
                return json;

            var prop = resourceJObject.Property(idName);
            RenameProperty(prop, newIdName);

            return resourceJObject.ToString();
        }

        public string SerializeObjectsAndRenameIdProperty<T>(List<T> resource, string idName, string newIdName)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new IgnoreSerializerContractResolver<JsonIgnoreOnSerialize>()
            };

            var json = JsonConvert.SerializeObject(resource, settings);

            if (string.IsNullOrEmpty(json))
                return string.Empty;

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

        public void RenameProperty(JToken token, string newName)
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

        public void UpdatePropertyValue(JToken token, object value)
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

        public void CreatePropertyOnJObject(JObject jObject, string propName, object value)
        {
            if(jObject == null || value == null)
                return;

            jObject.Add(new JProperty(propName, value));
        }

        public string GetValue(string json, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var jsonJObject = JObject.Parse(json);

            return jsonJObject[propertyName] != null ? jsonJObject[propertyName].ToString() : null;
        }

        public bool DoesPropertyExist(JObject jsonJObject, string propertyName)
        {
            return jsonJObject[propertyName] != null;
        }

    }
}
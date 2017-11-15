using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SilverCard.Deluge
{
    internal static class Utils
    {
        public static List<String> GetAllJsonPropertyFromType(Type t)
        {
            var type = typeof(JsonPropertyAttribute);
            var props = t.GetProperties().Where(prop => Attribute.IsDefined(prop, type)).ToList();
            var propsNames = props.Select(x => x.GetCustomAttributes(type, true).Single()).Cast<JsonPropertyAttribute>().Select(x => x.PropertyName);

            return propsNames.ToList();
        }
    }
}

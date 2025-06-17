using RepFabric.Api.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RepFabric.Api.BL.Helper
{
    public static class PurchaseOrderResponseHelper
    {
        /// <summary>
        /// Gets the value of a property from PurchaseOrderResponse by its JSON property name, as type T.
        /// </summary>
        public static T? GetByJsonPropertyName<T>(this PurchaseOrderResponse order, string attributeName)
        {
            if (order == null || string.IsNullOrWhiteSpace(attributeName))
                return default;

            var type = typeof(PurchaseOrderResponse);
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var jsonAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
                if (jsonAttr != null && string.Equals(jsonAttr.Name, attributeName, StringComparison.OrdinalIgnoreCase))
                {
                    var value = prop.GetValue(order);

                    if (value == null)
                        return default;

                    // If T is string, return as string
                    if (typeof(T) == typeof(string))
                        return (T)(object)value.ToString()!;

                    // If value is already of type T, return it
                    if (value is T tValue)
                        return tValue;

                    // Try to serialize and deserialize to T (for JSON types)
                    var json = System.Text.Json.JsonSerializer.Serialize(value);
                    return System.Text.Json.JsonSerializer.Deserialize<T>(json);
                }
            }
            return default;
        }

        /// <summary>
        /// Gets a list of values from all line items by the given JSON property name, converted to type T.
        /// </summary>
        public static List<T?> GetValuesByJsonPropertyName<T>(this List<PurchaseOrderLineItemResponse> lineItems, string attributeName)
        {
            var results = new List<T?>();
            if (lineItems == null || string.IsNullOrWhiteSpace(attributeName))
                return results;

            // Find the property in the line item class by JsonPropertyName
            var prop = typeof(PurchaseOrderLineItemResponse)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p =>
                {
                    var jsonAttr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
                    return jsonAttr != null && string.Equals(jsonAttr.Name, attributeName, System.StringComparison.OrdinalIgnoreCase);
                });

            if (prop == null)
                return results;

            foreach (var item in lineItems)
            {
                var value = prop.GetValue(item);
                if (value == null)
                {
                    results.Add(default);
                    continue;
                }

                // If T is string, just ToString
                if (typeof(T) == typeof(string))
                {
                    results.Add((T)(object)value.ToString()!);
                }
                // If value is already of type T
                else if (value is T tValue)
                {
                    results.Add(tValue);
                }
                else
                {
                    // Try to serialize and deserialize to T (for JSON types)
                    var json = JsonSerializer.Serialize(value);
                    results.Add(JsonSerializer.Deserialize<T>(json));
                }
            }

            return results;
        }
    }
}

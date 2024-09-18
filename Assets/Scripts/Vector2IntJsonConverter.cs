using UnityEngine;
using Newtonsoft.Json;
using System;

public class Vector2IntJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector2Int);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector2Int vector = (Vector2Int)value;
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(vector.x);
        writer.WritePropertyName("y");
        writer.WriteValue(vector.y);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        int x = 0;
        int y = 0;

        // Đọc các thuộc tính x và y từ JSON
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.PropertyName)
            {
                string propertyName = reader.Value.ToString();
                if (propertyName == "x")
                {
                    reader.Read();
                    x = Convert.ToInt32(reader.Value);
                }
                else if (propertyName == "y")
                {
                    reader.Read();
                    y = Convert.ToInt32(reader.Value);
                }
            }
            else if (reader.TokenType == JsonToken.EndObject)
            {
                break;
            }
        }
        Debug.Log($"Deserialized Vector2Int: {x}, {y}");
        return new Vector2Int(x, y);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Globalization;
using System.Linq;

namespace TheByteStuff.DynamicTableEntityJsonSerializer
{
    /// <summary>
    ///  Based on the nuget package offered by DoguArslan (https://www.nuget.org/packages/DynamicTableEntityJsonSerializer) which uses Microsoft.WindowsAzure.Storage.Table and .Net Framework 4.5, this package is adapted from the original work to function using Microsoft.Azure.Cosmos.Table to enable working with the Microsoft Azure CosmosDB Table API as well as the Azure Table Storage API and a broader .NetStandard 2.0.
    ///  
    /// The ReadJson method logic for ETag also deviates from the original code in that a true null value will be returned if the serialized data indicates null.  The original package sets ETag to empty string when the input value is null.
    /// In addition, the original logic for parsing the Timestamp loses the Offset and just returns the date and time.   Added a hack to include the offset in the Timestamp.
    /// 
    /// https://stackoverflow.com/questions/11553760/why-does-json-net-deserializeobject-change-the-timezone-to-local-time
    /// </summary>
    public class DynamicTableEntityJsonConverter : JsonConverter
    {
        private const int EntityPropertyIndex = 0;
        private const int EntityPropertyEdmTypeIndex = 1;
        private readonly List<string> excludedProperties;

        private string[] splitdelim = { "\": " };

        public DynamicTableEntityJsonConverter(List<string> excludedProperties = null)
        {
            this.excludedProperties = excludedProperties;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                return;
            writer.WriteStartObject();
            DynamicTableEntityJsonConverter.WriteJsonProperties(writer, (DynamicTableEntity)value, this.excludedProperties);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return (object)null;
            DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
            using (List<JProperty>.Enumerator enumerator = JObject.Load(reader).Properties().ToList<JProperty>().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    JProperty current = enumerator.Current;
                    if (string.Equals(current.Name, "PartitionKey", StringComparison.Ordinal))
                        dynamicTableEntity.PartitionKey = ((object)current.Value).ToString();
                    else if (string.Equals(current.Name, "RowKey", StringComparison.Ordinal))
                        dynamicTableEntity.RowKey = ((object)current.Value).ToString();
                    else if (string.Equals(current.Name, "Timestamp", StringComparison.Ordinal))
                        dynamicTableEntity.Timestamp = DateTimeOffset.Parse(ExtractTimestamp(enumerator.Current.ToString().Split(splitdelim, StringSplitOptions.RemoveEmptyEntries)[1]));
                    //  dynamicTableEntity.Timestamp = (DateTimeOffset)current.Value.ToObject<DateTimeOffset>(serializer);
                    else if (string.Equals(current.Name, "ETag", StringComparison.Ordinal))
                    {
                        dynamicTableEntity.ETag = (current.Value.Type == JTokenType.Null) ? null : (((object)current.Value).ToString());
                    }
                    else
                    {
                        EntityProperty entityProperty = DynamicTableEntityJsonConverter.CreateEntityProperty(serializer, current);
                        dynamicTableEntity.Properties.Add(current.Name, entityProperty);
                    }
                }
            }
            return (object)dynamicTableEntity;
        }


        /// <summary>
        /// Extract the Time with offset from the original Timestamp value
        /// </summary>
        /// <param name="xx"></param>
        /// <returns></returns>
        private string ExtractTimestamp(string xx)
        {
            if (xx.Length<2)
            {
                return xx;
            }
            else
            {
                return xx.Substring(1, xx.Length - 2);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(DynamicTableEntity).IsAssignableFrom(objectType);
        }

        private static void WriteJsonProperties(
          JsonWriter writer,
          DynamicTableEntity entity,
          List<string> excludedProperties = null)
        {
            if (entity == null)
                return;
            writer.WritePropertyName("PartitionKey");
            writer.WriteValue(entity.PartitionKey);
            writer.WritePropertyName("RowKey");
            writer.WriteValue(entity.RowKey);
            writer.WritePropertyName("Timestamp");
            writer.WriteValue(entity.Timestamp);
            writer.WritePropertyName("ETag");
            writer.WriteValue(entity.ETag);
            using (IEnumerator<KeyValuePair<string, EntityProperty>> enumerator = (excludedProperties == null ? (IEnumerable<KeyValuePair<string, EntityProperty>>)entity.Properties : ((IEnumerable<KeyValuePair<string, EntityProperty>>)entity.Properties).Where<KeyValuePair<string, EntityProperty>>((Func<KeyValuePair<string, EntityProperty>, bool>)(p => !excludedProperties.Contains(p.Key)))).GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    KeyValuePair<string, EntityProperty> current = enumerator.Current;
                    DynamicTableEntityJsonConverter.WriteJsonProperty(writer, current);
                }
            }
        }

        private static void WriteJsonProperty(
          JsonWriter writer,
          KeyValuePair<string, EntityProperty> property)
        {
            // https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonToken.htm
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.table.edmtype
            if (string.IsNullOrWhiteSpace(property.Key) || property.Value == null)
                return;
            switch ((int)property.Value.PropertyType)
            {
                case 0:
                    DynamicTableEntityJsonConverter.WriteJsonPropertyWithEdmType(writer, property.Key, (object)property.Value.StringValue, property.Value.PropertyType);
                    break;
                case 1:
                    DynamicTableEntityJsonConverter.WriteJsonPropertyWithEdmType(writer, property.Key, (object)property.Value.BinaryValue, property.Value.PropertyType);
                    break;
                case 2:
                    DynamicTableEntityJsonConverter.WriteJsonPropertyWithEdmType(writer, property.Key, (object)property.Value.BooleanValue, property.Value.PropertyType);
                    break;
                case 3:
                    DynamicTableEntityJsonConverter.WriteJsonPropertyWithEdmType(writer, property.Key, (object)property.Value.DateTimeOffsetValue, property.Value.PropertyType);
                    break;
                case 4:
                    DynamicTableEntityJsonConverter.WriteJsonPropertyWithEdmType(writer, property.Key, (object)property.Value.DoubleValue, property.Value.PropertyType);
                    break;
                case 5:
                    DynamicTableEntityJsonConverter.WriteJsonPropertyWithEdmType(writer, property.Key, (object)property.Value.GuidValue, property.Value.PropertyType);
                    break;
                case 6:
                    DynamicTableEntityJsonConverter.WriteJsonPropertyWithEdmType(writer, property.Key, (object)property.Value.Int32Value, property.Value.PropertyType);
                    break;
                case 7:
                    DynamicTableEntityJsonConverter.WriteJsonPropertyWithEdmType(writer, property.Key, (object)property.Value.Int64Value, property.Value.PropertyType);
                    break;
                default:
                    throw new NotSupportedException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Unsupported EntityProperty.PropertyType:{0} detected during serialization.", (object)property.Value.PropertyType));
            }
        }

        private static void WriteJsonPropertyWithEdmType(
          JsonWriter writer,
          string key,
          object value,
          EdmType edmType)
        {
            writer.WritePropertyName(key);
            writer.WriteStartObject();
            writer.WritePropertyName(key);
            writer.WriteValue(value);
            writer.WritePropertyName("EdmType");
            writer.WriteValue(edmType.ToString());
            writer.WriteEndObject();
        }

        private static EntityProperty CreateEntityProperty(
          JsonSerializer serializer,
          JProperty property)
        {
            if (property == null)
                return (EntityProperty)null;
            List<JProperty> list = JObject.Parse(((object)property.Value).ToString()).Properties().ToList<JProperty>();
            EdmType edmType = (EdmType)Enum.Parse(typeof(EdmType), ((object)list[1].Value).ToString(), true);
            EntityProperty entityProperty;
            switch ((int)edmType)
            {
                case 0:
                    entityProperty = EntityProperty.GeneratePropertyForString((string)list[0].Value.ToObject<string>(serializer));
                    break;
                case 1:
                    entityProperty = EntityProperty.GeneratePropertyForByteArray((byte[])list[0].Value.ToObject<byte[]>(serializer));
                    break;
                case 2:
                    entityProperty = EntityProperty.GeneratePropertyForBool(new bool?((bool)list[0].Value.ToObject<bool>(serializer)));
                    break;
                case 3:
                    entityProperty = EntityProperty.GeneratePropertyForDateTimeOffset(new DateTimeOffset?((DateTimeOffset)list[0].Value.ToObject<DateTimeOffset>(serializer)));
                    break;
                case 4:
                    entityProperty = EntityProperty.GeneratePropertyForDouble(new double?((double)list[0].Value.ToObject<double>(serializer)));
                    break;
                case 5:
                    entityProperty = EntityProperty.GeneratePropertyForGuid(new Guid?((Guid)list[0].Value.ToObject<Guid>(serializer)));
                    break;
                case 6:
                    entityProperty = EntityProperty.GeneratePropertyForInt(new int?((int)list[0].Value.ToObject<int>(serializer)));
                    break;
                case 7:
                    entityProperty = EntityProperty.GeneratePropertyForLong(new long?((long)list[0].Value.ToObject<long>(serializer)));
                    break;
                default:
                    throw new NotSupportedException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Unsupported EntityProperty.PropertyType:{0} detected during deserialization.", (object)edmType));
            }
            return entityProperty;
        }
    }
}

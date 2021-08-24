using System;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TheByteStuff.DynamicTableEntityJsonSerializer
{
    /// <summary>
    ///  Based on the nuget package offered by DoguArslan (https://www.nuget.org/packages/DynamicTableEntityJsonSerializer) which uses Microsoft.WindowsAzure.Storage.Table and .Net Framework 4.5, this package is adapted from the original work to function using Microsoft.Azure.Cosmos.Table to enable working with the Microsoft Azure CosmosDB Table API as well as the Azure Table Storage API and a broader .NetStandard 2.0.
    /// </summary>
    public class DynamicTableEntityJsonSerializer
    {
        private readonly DynamicTableEntityJsonConverter jsonConverter;

        public DynamicTableEntityJsonSerializer(List<string> excludedProperties = null)
        {
            this.jsonConverter = new DynamicTableEntityJsonConverter(excludedProperties);
        }

        public string Serialize(DynamicTableEntity entity)
        {
            string str;
            if (entity != null)
                str = JsonConvert.SerializeObject((object)entity, new JsonConverter[1]
                {
          (JsonConverter) this.jsonConverter
                });
            else
                str = (string)null;
            return str;
        }

        public DynamicTableEntity Deserialize(string serializedEntity)
        {
            DynamicTableEntity local;
            if (serializedEntity != null)
                local = JsonConvert.DeserializeObject<DynamicTableEntity>(serializedEntity, new JsonConverter[1] { (JsonConverter)this.jsonConverter });
            else
                local = null;
            return (DynamicTableEntity)local;
        }
    }
}
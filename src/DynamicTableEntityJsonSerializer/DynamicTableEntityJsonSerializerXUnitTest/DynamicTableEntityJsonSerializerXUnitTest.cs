using System;
using Xunit;

using CosmosTable = Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System.Collections.Generic;

using TheByteStuff.DynamicTableEntityJsonSerializer;

namespace DynamicTableEntityJsonSerializerXUnitTest
{
    public class DynamicTableEntityJsonSerializerXUnitTest
    {
        private string BasicEntity = "{\"PartitionKey\":\"PartitionKey\",\"RowKey\":\"RowKey\",\"Timestamp\":\"2021-01-01T10:00:00+02:00\",\"ETag\":null}";

        [Fact]
        public void TestSerialize()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();

            string PartitionKeyValue = "PartitionKeyValueXX";
            string RowKeyValue = "RowKeyValueXX";
            string TimeStampValue = "2021-08-21T12:15:30-05:00";
            string EtagValue = "EtagValueXX";

            CosmosTable.DynamicTableEntity dte4 = new CosmosTable.DynamicTableEntity(PartitionKeyValue, RowKeyValue);
            dte4.ETag = EtagValue;
            dte4.Timestamp = DateTimeOffset.Parse(TimeStampValue);
            string Serialized = serializer.Serialize(dte4);

            string ExpectedValue = String.Format("{{\"PartitionKey\":\"{0}\",\"RowKey\":\"{1}\",\"Timestamp\":\"{2}\",\"ETag\":\"{3}\"}}", PartitionKeyValue, RowKeyValue, TimeStampValue, EtagValue);
            Assert.Equal(ExpectedValue, Serialized);
        }

        [Fact]
        public void TestDeserialize()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();

            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(BasicEntity);

            CosmosTable.DynamicTableEntity dte3 = new CosmosTable.DynamicTableEntity("PartitionKey", "RowKey");
            dte3.Timestamp = DateTimeOffset.Parse("2021-01-01T10:00:00+02:00");

            Assert.Equal(dte3.RowKey, dte2.RowKey);
            Assert.Equal(dte3.PartitionKey, dte2.PartitionKey);
            Assert.Equal(dte3.Properties, dte2.Properties);
            Assert.Equal(dte3.ETag, dte2.ETag);
            Assert.Equal(dte3.Timestamp, dte2.Timestamp);
        }
    }
}

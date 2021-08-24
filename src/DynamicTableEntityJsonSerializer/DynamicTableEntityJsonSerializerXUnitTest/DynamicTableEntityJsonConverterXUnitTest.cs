using System;
using System.Text;
using Xunit;

using CosmosTable = Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System.Collections.Generic;

using TheByteStuff.DynamicTableEntityJsonSerializer;

namespace DynamicTableEntityJsonSerializerXUnitTest
{
    public class DynamicTableEntityJsonConverterXUnitTest
    {
        private string ComplexEntity = "{\"PartitionKey\":\"PartitionKey\",\"RowKey\":\"RowKey\",\"Timestamp\":\"2019-12-19T12:21:16.65-05:00\",\"ETag\":\"EtagValue\",\"GUIDField\":{\"GUIDField\":\"4343651b-570e-476c-a187-2dc2e764c152\",\"EdmType\":\"GUID\"},\"StringField\":{\"StringField\":\"StringValue\",\"EdmType\":\"String\"},\"Int32Field\":{\"Int32Field\":\"12\",\"EdmType\":\"Int32\"},\"Int64Field\":{\"Int64Field\":\"255486129307\",\"EdmType\":\"Int64\"},\"BinaryField\":{\"BinaryField\":\"MDEwMTAxMDE=\",\"EdmType\":\"Binary\"},\"DoubleField\":{\"DoubleField\":\"-1.79e+308\",\"EdmType\":\"Double\"},\"BooleanField\":{\"BooleanField\":\"True\",\"EdmType\":\"Boolean\"},\"DateTimeField\":{\"DateTimeField\":\"2019-12-19T11:29:56.1896893-05:00\",\"EdmType\":\"DateTime\"}}";

        private string ComplexEntity_BadEdmType = "{\"PartitionKey\":\"PartitionKey\",\"RowKey\":\"RowKey\",\"Timestamp\":\"2019-12-19T12:21:16.65-05:00\",\"ETag\":\"W/\\\"datetime'2019-12-19T17%3A21%3A16.65Z'\\\"\",\"GUIDField\":{\"GUIDField\":\"4343651b-570e-476c-a187-2dc2e764c152\",\"EdmType\":\"GUID\"},\"StringField\":{\"StringField\":\"StringValue\",\"EdmType\":\"String\"},\"Int32Field\":{\"Int32Field\":\"12\",\"EdmType\":\"Int322\"},\"Int64Field\":{\"Int64Field\":\"255486129307\",\"EdmType\":\"Int64\"},\"BinaryField\":{\"BinaryField\":\"MDEwMTAxMDE=\",\"EdmType\":\"Binary\"},\"DoubleField\":{\"DoubleField\":\"-1.79e+308\",\"EdmType\":\"Double\"},\"BooleanField\":{\"BooleanField\":\"True\",\"EdmType\":\"Boolean\"},\"DateTimeField\":{\"DateTimeField\":\"2019-12-19T11:29:56.1896893-05:00\",\"EdmType\":\"DateTime\"}}";
        private string ComplexEntity_InvalidBinaryValue = "{\"PartitionKey\":\"PartitionKey\",\"RowKey\":\"RowKey\",\"Timestamp\":\"2019-12-19T12:21:16.65-05:00\",\"ETag\":\"W/\\\"datetime'2019-12-19T17%3A21%3A16.65Z'\\\"\",\"GUIDField\":{\"GUIDField\":\"4343651b-570e-476c-a187-2dc2e764c152\",\"EdmType\":\"GUID\"},\"StringField\":{\"StringField\":\"StringValue\",\"EdmType\":\"String\"},\"Int32Field\":{\"Int32Field\":\"12\",\"EdmType\":\"Int32\"},\"Int64Field\":{\"Int64Field\":\"255486129307\",\"EdmType\":\"Int64\"},\"BinaryField\":{\"BinaryField\":\"93-E3-E6-3F-C3-F5-E4-41-00-C0-8D-C3-14-EE-4A-C3-00\",\"EdmType\":\"Binary\"},\"DoubleField\":{\"DoubleField\":\"-1.79e+308\",\"EdmType\":\"Double\"},\"BooleanField\":{\"BooleanField\":\"True\",\"EdmType\":\"Boolean\"},\"DateTimeField\":{\"DateTimeField\":\"2019-12-19T11:29:56.1896893-05:00\",\"EdmType\":\"DateTime\"}}";
        private string ComplexEntity_InvalidBooleanValue = "{\"PartitionKey\":\"PartitionKey\",\"RowKey\":\"RowKey\",\"Timestamp\":\"2019-12-19T12:21:16.65-05:00\",\"ETag\":\"W/\\\"datetime'2019-12-19T17%3A21%3A16.65Z'\\\"\",\"GUIDField\":{\"GUIDField\":\"4343651b-570e-476c-a187-2dc2e764c152\",\"EdmType\":\"GUID\"},\"StringField\":{\"StringField\":\"StringValue\",\"EdmType\":\"String\"},\"Int32Field\":{\"Int32Field\":\"12\",\"EdmType\":\"Int32\"},\"Int64Field\":{\"Int64Field\":\"255486129307\",\"EdmType\":\"Int64\"},\"BinaryField\":{\"BinaryField\":\"MDEwMTAxMDE=\",\"EdmType\":\"Binary\"},\"DoubleField\":{\"DoubleField\":\"-1.79e+308\",\"EdmType\":\"Double\"},\"BooleanField\":{\"BooleanField\":\"TrueDat\",\"EdmType\":\"Boolean\"},\"DateTimeField\":{\"DateTimeField\":\"2019-12-19T11:29:56.1896893-05:00\",\"EdmType\":\"DateTime\"}}";
        private string ComplexEntity_InvalidDateTimeValue = "{\"PartitionKey\":\"PartitionKey\",\"RowKey\":\"RowKey\",\"Timestamp\":\"2019-12-19T12:21:16.65-05:00\",\"ETag\":\"W/\\\"datetime'2019-12-19T17%3A21%3A16.65Z'\\\"\",\"GUIDField\":{\"GUIDField\":\"4343651b-570e-476c-a187-2dc2e764c152\",\"EdmType\":\"GUID\"},\"StringField\":{\"StringField\":\"StringValue\",\"EdmType\":\"String\"},\"Int32Field\":{\"Int32Field\":\"12\",\"EdmType\":\"Int32\"},\"Int64Field\":{\"Int64Field\":\"255486129307\",\"EdmType\":\"Int64\"},\"BinaryField\":{\"BinaryField\":\"MDEwMTAxMDE=\",\"EdmType\":\"Binary\"},\"DoubleField\":{\"DoubleField\":\"-1.79e+308\",\"EdmType\":\"Double\"},\"BooleanField\":{\"BooleanField\":\"True\",\"EdmType\":\"Boolean\"},\"DateTimeField\":{\"DateTimeField\":\"2019-13-19T11:29:56.1896893-05:00\",\"EdmType\":\"DateTime\"}}";
        private string ComplexEntity_InvalidDoubleValue = "{\"PartitionKey\":\"PartitionKey\",\"RowKey\":\"RowKey\",\"Timestamp\":\"2019-12-19T12:21:16.65-05:00\",\"ETag\":\"W/\\\"datetime'2019-12-19T17%3A21%3A16.65Z'\\\"\",\"GUIDField\":{\"GUIDField\":\"4343651b-570e-476c-a187-2dc2e764c152\",\"EdmType\":\"GUID\"},\"StringField\":{\"StringField\":\"StringValue\",\"EdmType\":\"String\"},\"Int32Field\":{\"Int32Field\":\"12\",\"EdmType\":\"Int32\"},\"Int64Field\":{\"Int64Field\":\"255486129307\",\"EdmType\":\"Int64\"},\"BinaryField\":{\"BinaryField\":\"MDEwMTAxMDE=\",\"EdmType\":\"Binary\"},\"DoubleField\":{\"DoubleField\":\"-1.79ee+308\",\"EdmType\":\"Double\"},\"BooleanField\":{\"BooleanField\":\"True\",\"EdmType\":\"Boolean\"},\"DateTimeField\":{\"DateTimeField\":\"2019-12-19T11:29:56.1896893-05:00\",\"EdmType\":\"DateTime\"}}";
        private string ComplexEntity_InvalidGUIDValue = "{\"PartitionKey\":\"PartitionKey\",\"RowKey\":\"RowKey\",\"Timestamp\":\"2019-12-19T12:21:16.65-05:00\",\"ETag\":\"W/\\\"datetime'2019-12-19T17%3A21%3A16.65Z'\\\"\",\"GUIDField\":{\"GUIDField\":\"4343651b-570e-476cd-a187-2dc2e764c152\",\"EdmType\":\"GUID\"},\"StringField\":{\"StringField\":\"StringValue\",\"EdmType\":\"String\"},\"Int32Field\":{\"Int32Field\":\"12\",\"EdmType\":\"Int32\"},\"Int64Field\":{\"Int64Field\":\"255486129307\",\"EdmType\":\"Int64\"},\"BinaryField\":{\"BinaryField\":\"MDEwMTAxMDE=\",\"EdmType\":\"Binary\"},\"DoubleField\":{\"DoubleField\":\"-1.79e+308\",\"EdmType\":\"Double\"},\"BooleanField\":{\"BooleanField\":\"True\",\"EdmType\":\"Boolean\"},\"DateTimeField\":{\"DateTimeField\":\"2019-12-19T11:29:56.1896893-05:00\",\"EdmType\":\"DateTime\"}}";
        private string ComplexEntity_InvalidInt32Value = "{\"PartitionKey\":\"PartitionKey\",\"RowKey\":\"RowKey\",\"Timestamp\":\"2019-12-19T12:21:16.65-05:00\",\"ETag\":\"W/\\\"datetime'2019-12-19T17%3A21%3A16.65Z'\\\"\",\"GUIDField\":{\"GUIDField\":\"4343651b-570e-476c-a187-2dc2e764c152\",\"EdmType\":\"GUID\"},\"StringField\":{\"StringField\":\"StringValue\",\"EdmType\":\"String\"},\"Int32Field\":{\"Int32Field\":\"255486129307\",\"EdmType\":\"Int32\"},\"Int64Field\":{\"Int64Field\":\"255486129307\",\"EdmType\":\"Int64\"},\"BinaryField\":{\"BinaryField\":\"MDEwMTAxMDE=\",\"EdmType\":\"Binary\"},\"DoubleField\":{\"DoubleField\":\"-1.79e+308\",\"EdmType\":\"Double\"},\"BooleanField\":{\"BooleanField\":\"True\",\"EdmType\":\"Boolean\"},\"DateTimeField\":{\"DateTimeField\":\"2019-12-19T11:29:56.1896893-05:00\",\"EdmType\":\"DateTime\"}}";
        private string ComplexEntity_InvalidInt64Value = "{\"PartitionKey\":\"PartitionKey\",\"RowKey\":\"RowKey\",\"Timestamp\":\"2019-12-19T12:21:16.65-05:00\",\"ETag\":\"W/\\\"datetime'2019-12-19T17%3A21%3A16.65Z'\\\"\",\"GUIDField\":{\"GUIDField\":\"4343651b-570e-476c-a187-2dc2e764c152\",\"EdmType\":\"GUID\"},\"StringField\":{\"StringField\":\"StringValue\",\"EdmType\":\"String\"},\"Int32Field\":{\"Int32Field\":\"12\",\"EdmType\":\"Int32\"},\"Int64Field\":{\"Int64Field\":\"2554861e29307\",\"EdmType\":\"Int64\"},\"BinaryField\":{\"BinaryField\":\"MDEwMTAxMDE=\",\"EdmType\":\"Binary\"},\"DoubleField\":{\"DoubleField\":\"-1.79e+308\",\"EdmType\":\"Double\"},\"BooleanField\":{\"BooleanField\":\"True\",\"EdmType\":\"Boolean\"},\"DateTimeField\":{\"DateTimeField\":\"2019-12-19T11:29:56.1896893-05:00\",\"EdmType\":\"DateTime\"}}";
        private string ComplexEntity_InvalidStringValue = "{\"PartitionKey\":\"PartitionKey\",\"RowKey\":\"RowKey\",\"Timestamp\":\"2019-12-19T12:21:16.65-05:00\",\"ETag\":\"W/\\\"datetime'2019-12-19T17%3A21%3A16.65Z'\\\"\",\"GUIDField\":{\"GUIDField\":\"4343651b-570e-476c-a187-2dc2e764c152\",\"EdmType\":\"GUID\"},\"StringField\":{\"StringField\":\"String\"Value\",\"EdmType\":\"String\"},\"Int32Field\":{\"Int32Field\":\"12\",\"EdmType\":\"Int32\"},\"Int64Field\":{\"Int64Field\":\"255486129307\",\"EdmType\":\"Int64\"},\"BinaryField\":{\"BinaryField\":\"MDEwMTAxMDE=\",\"EdmType\":\"Binary\"},\"DoubleField\":{\"DoubleField\":\"-1.79e+308\",\"EdmType\":\"Double\"},\"BooleanField\":{\"BooleanField\":\"True\",\"EdmType\":\"Boolean\"},\"DateTimeField\":{\"DateTimeField\":\"2019-12-19T11:29:56.1896893-05:00\",\"EdmType\":\"DateTime\"}}";

        private string StringValue = "StringValue";
        private int Int32Value = 12;
        private long Int64Value = 255486129307;
        private bool BooleanValue = true;
        private double DoubleValue = -1.79e+308;
        private static string x ="01010101";
        private byte[] BinaryValue = Encoding.ASCII.GetBytes(x);
        private DateTime DateTimeValue2 = new DateTime(2021, 1, 21);
        private DateTime DateTimeValue3 = DateTime.Parse("2019-12-19T11:29:56.1896893-05:00");
        private DateTime DateTimeValue = DateTimeOffset.Parse("2019-12-19T11:29:56.1896893-05:00").DateTime;
        private Guid GuidValue = new Guid("4343651b-570e-476c-a187-2dc2e764c152");
        string DateTimeValueRaw = "2019-12-19T11:29:56.1896893-05:00";
        string TimestampValueRaw = "2019-12-19T12:21:16.65-05:00";


        [Fact]
        public void TestPartitionKeyField()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();
            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(ComplexEntity);
            Assert.Equal("PartitionKey", dte2.PartitionKey);
        }


        [Fact]
        public void TestRowKeyField()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();
            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(ComplexEntity);
            Assert.Equal("RowKey", dte2.RowKey);
        }


        [Fact]
        public void TestETagField()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();
            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(ComplexEntity);
            Assert.Equal("EtagValue", dte2.ETag);
        }


        [Fact]
        public void TestTimestampField()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();
            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(ComplexEntity);
            Assert.Equal(DateTimeOffset.Parse(TimestampValueRaw).UtcDateTime, dte2.Timestamp.UtcDateTime);
        }

        [Fact]
        public void TestPropertiesEdmTypeDateTime()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();
            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(ComplexEntity);
            Assert.Equal(DateTimeOffset.Parse(DateTimeValueRaw).UtcDateTime, dte2.Properties["DateTimeField"].DateTime);
        }

        [Fact]
        public void TestPropertieEdmTypeBinary()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();
            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(ComplexEntity);
            Assert.Equal(BinaryValue, dte2.Properties["BinaryField"].BinaryValue);
        }

        [Fact]
        public void TestPropertiesEdmTypeBoolean()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();
            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(ComplexEntity);
            Assert.Equal(BooleanValue, dte2.Properties["BooleanField"].BooleanValue);
        }

        [Fact]
        public void TestPropertiesEdmTypeDouble()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();
            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(ComplexEntity);
            Assert.Equal(DoubleValue, dte2.Properties["DoubleField"].DoubleValue);
        }

        [Fact]
        public void TestPropertiesEdmTypeGuid()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();
            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(ComplexEntity);
            Assert.Equal(GuidValue, dte2.Properties["GUIDField"].GuidValue);
        }

        [Fact]
        public void TestPropertiesEdmTypeInt32()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();
            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(ComplexEntity);
            Assert.Equal(Int32Value, dte2.Properties["Int32Field"].Int32Value);
        }

        [Fact]
        public void TestPropertiesEdmTypeInt64()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();
            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(ComplexEntity);
            Assert.Equal(Int64Value, dte2.Properties["Int64Field"].Int64Value);
        }

        [Fact]
        public void TestPropertiesEdmTypeString()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();
            CosmosTable.DynamicTableEntity dte2 = serializer.Deserialize(ComplexEntity);
            Assert.Equal(StringValue, dte2.Properties["StringField"].StringValue);
        }

        [Fact]
        public void TestExceptions()
        {
            DynamicTableEntityJsonSerializer serializer = new DynamicTableEntityJsonSerializer();

            var exceptionBadEdmType = Assert.Throws<System.ArgumentException>(() =>  serializer.Deserialize(ComplexEntity_BadEdmType));
            Assert.Contains("Requested value 'Int322' was not found.", exceptionBadEdmType.Message);

            var exceptionInvalidBinary = Assert.Throws<System.FormatException>(() => serializer.Deserialize(ComplexEntity_InvalidBinaryValue));
            Assert.Contains("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters.", exceptionInvalidBinary.Message);

            var exceptionInvalidBoolean = Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => serializer.Deserialize(ComplexEntity_InvalidBooleanValue));
            Assert.Contains("Could not convert string to boolean: TrueDat. Path 'BooleanField',", exceptionInvalidBoolean.Message);

            var exceptionInvalidDateTime = Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => serializer.Deserialize(ComplexEntity_InvalidDateTimeValue));
            Assert.Contains("Could not convert string to DateTimeOffset: 2019-13-19T11:29:56.1896893-05:00. Path 'DateTimeField'", exceptionInvalidDateTime.Message);

            var exceptionInvalidDouble = Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => serializer.Deserialize(ComplexEntity_InvalidDoubleValue));
            Assert.Contains("Could not convert string to double: -1.79ee+308. Path 'DoubleField'", exceptionInvalidDouble.Message);

            var exceptionInvalidGuid = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => serializer.Deserialize(ComplexEntity_InvalidGUIDValue));
            Assert.Contains("Error converting value \"4343651b-570e-476cd-a187-2dc2e764c152\" to type 'System.Guid'. Path 'GUIDField'", exceptionInvalidGuid.Message);

            var exceptionInvalidInt32 = Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => serializer.Deserialize(ComplexEntity_InvalidInt32Value));
            Assert.Contains("Could not convert string to integer: 255486129307. Path 'Int32Field'", exceptionInvalidInt32.Message);

            var exceptionInvalidInt64 = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => serializer.Deserialize(ComplexEntity_InvalidInt64Value));
            Assert.Contains("Error converting value \"2554861e29307\" to type 'System.Int64'", exceptionInvalidInt64.Message);

            var exceptionInvalidString = Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => serializer.Deserialize(ComplexEntity_InvalidStringValue));
            Assert.Contains("After parsing a value an unexpected character was encountered: V. Path 'StringField.StringField'", exceptionInvalidString.Message);
        }
    }
}
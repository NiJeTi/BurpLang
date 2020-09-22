using System.IO;

using BurpLang.Core.Tests.Entities;

using NUnit.Framework;

namespace BurpLang.Core.Tests
{
    public class SerializationTest
    {
        private BaseEntity? _baseEntity;
        private string? _serializedEntity;

        [SetUp]
        public void SetUp()
        {
            _baseEntity = new BaseEntity();
            _serializedEntity = File.ReadAllText(Path.Combine("Content", "testSerializedEntity.bl"));
        }
        
        [Test]
        public void Serialize()
        {
            string serialized = Converter.Serialize(_baseEntity);

            Assert.That(serialized, Is.EqualTo(_serializedEntity));
        }
    }
}
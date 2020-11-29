using System.IO;

using BurpLang.Common.Entities;

using NUnit.Framework;

namespace BurpLang.Tests
{
    public class DeserializationTest
    {
        private readonly Entity _entity = new Entity();

        private string _serializedEntity = null!;

        [SetUp]
        public void SetUp()
        {
            _serializedEntity = File.ReadAllText(Path.Combine("Content", "testSerializedEntity.bl"));
        }

        [Test]
        public void Deserialize()
        {
            var deserialized = Converter.Deserialize<Entity>(_serializedEntity!);

            Assert.That(deserialized, Is.EqualTo(_entity));
        }
    }
}
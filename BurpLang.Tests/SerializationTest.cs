using System.IO;

using BurpLang.Tests.Entities;

using NUnit.Framework;

namespace BurpLang.Tests
{
    public class SerializationTest
    {
        private readonly Entity _entity = new Entity();

        private string _serializedEntity = null!;

        [SetUp]
        public void SetUp()
        {
            _serializedEntity = File.ReadAllText(Path.Combine("Content", "testSerializedEntity.bl"));
        }

        [Test]
        public void Serialize()
        {
            string serialized = Converter.Serialize(_entity!);

            Assert.That(serialized, Is.EqualTo(_serializedEntity));
        }
    }
}
using System.IO;

using BurpLang.Common.Entities;

using NUnit.Framework;

namespace BurpLang.Tests
{
    public class ParsingTests
    {
        private readonly Entity _entity = new Entity();

        private Parser<Entity> _parser = null!;

        [SetUp]
        public void SetUp()
        {
            var serializedEntity = File.ReadAllText(Path.Combine("Entities", "SerializedEntity.bl"));

            _parser = new Parser<Entity>(serializedEntity);
        }

        [Test]
        public void ParseFromFile_ValidSyntax()
        {
            var parsedEntity = _parser.GetObject();

            Assert.That(parsedEntity, Is.EqualTo(_entity));
        }
    }
}
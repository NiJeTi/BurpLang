using System.IO;

using BurpLang.Common.Entities;

using NUnit.Framework;

namespace BurpLang.Tests
{
    public class ParsingTests
    {
        private Entity _expectedEntity = null!;

        private Parser<Entity> _parser = null!;

        [SetUp]
        public void SetUp()
        {
            _expectedEntity = new Entity
            {
                SomeText = "String",
                ThisIsNumber = 123,
                ThisIsFloatingPointNumber = 123.456f,
                SomeLogicalStatement = true,
                TextLines = new[]
                {
                    "String1",
                    "String2",
                    "String3"
                },
                MultipleNumbers = new[]
                {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6
                },
                MultipleRealNumbers = new[]
                {
                    1.2f,
                    3.4f,
                    5.6f,
                    7.8f,
                    9.0f
                },
                BunchOfStatements = new[]
                {
                    true,
                    false,
                    false,
                    true
                }
            };

            var serializedEntity = File.ReadAllText(Path.Combine("Entities", "SerializedEntity.bl"));
            _parser = new Parser<Entity>(serializedEntity);
        }

        [Test]
        public void ParseFromFile_ValidSyntax()
        {
            var parsedEntity = _parser.GetObject();

            Assert.That(parsedEntity, Is.EqualTo(_expectedEntity));
        }
    }
}
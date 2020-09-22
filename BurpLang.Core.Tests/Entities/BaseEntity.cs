using System.Collections.Generic;

namespace BurpLang.Core.Tests.Entities
{
    public class BaseEntity
    {
        public string String { get; } = "String";

        public IEnumerable<int> EnumerablePrimitives { get; } = new[] { 1, 2, 3, 4, 5, 6 };

        public NestedEntity1 NestedEntity1 { get; set; } = new NestedEntity1
        {
            IntValue = 123456,
            FloatValue = 123.456f
        };

        public IEnumerable<NestedEntity2> EnumerableObjects { get; } = new[]
        {
            new NestedEntity2
            {
                StringValue = "Value1",
                NumericValue = 10
            },
            new NestedEntity2
            {
                StringValue = "Value2",
                NumericValue = 20
            },
            new NestedEntity2
            {
                StringValue = "Value3",
                NumericValue = 30
            }
        };

        public string? Empty { get; } = null;

        public bool True { get; } = true;

        public bool False { get; } = false;
    }
}
using System;
using System.Collections.Generic;

namespace BurpLang.Core.Tests.Entities
{
    public class Entity : IEquatable<Entity>
    {
        public string String { get; set; } = "String";

        public int Integer { get; set; } = 123;

        public float FloatingPointNumber { get; set; } = 123.456f;

        public IEnumerable<string> EnumerableStrings { get; set; } = new[] { "String1", "String2", "String3" };

        public IEnumerable<int> EnumerableNumbers { get; set; } = new[] { 1, 2, 3, 4, 5, 6 };

        public bool TrueBoolean { get; set; } = true;

        public bool FalseBoolean { get; set; } = false;

        public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);

        public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType()
                && Equals((Entity) obj);
        }

        public override int GetHashCode() =>
            HashCode.Combine(String, Integer, FloatingPointNumber, EnumerableStrings, EnumerableNumbers,
                TrueBoolean, FalseBoolean);

        public bool Equals(Entity? other)
        {
            if (other is null)
                return false;

            return String == other.String
                && EnumerableNumbers.Equals(other.EnumerableNumbers)
                && EnumerableStrings.Equals(other.EnumerableStrings)
                && TrueBoolean == other.TrueBoolean
                && FalseBoolean == other.FalseBoolean;
        }
    }
}
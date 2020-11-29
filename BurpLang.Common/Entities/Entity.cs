using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace BurpLang.Common.Entities
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
            HashCode.Combine(String, Integer, FloatingPointNumber,
                EnumerableStrings, EnumerableNumbers,
                TrueBoolean, FalseBoolean);

        public bool Equals(Entity? other)
        {
            if (other is null)
                return false;

            return String == other.String
                && EnumerableNumbers.SequenceEqual(other.EnumerableNumbers)
                && EnumerableStrings.SequenceEqual(other.EnumerableStrings)
                && TrueBoolean == other.TrueBoolean
                && FalseBoolean == other.FalseBoolean;
        }

        public override string ToString() =>
            $"{nameof(String)}: {String}, " +
            $"{nameof(Integer)}: {Integer}, " +
            $"{nameof(FloatingPointNumber)}: {FloatingPointNumber}, " +
            $"{nameof(EnumerableStrings)}: {EnumerableStrings}, " +
            $"{nameof(EnumerableNumbers)}: {EnumerableNumbers}, " +
            $"{nameof(TrueBoolean)}: {TrueBoolean}, " +
            $"{nameof(FalseBoolean)}: {FalseBoolean}";
    }
}
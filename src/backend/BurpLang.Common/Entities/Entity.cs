using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace BurpLang.Common.Entities
{
    public class Entity : IEquatable<Entity>
    {
        public string? SomeText { get; set; }
        public int ThisIsNumber { get; set; }
        public float ThisIsFloatingPointNumber { get; set; }
        public bool SomeLogicalStatement { get; set; }

        public IEnumerable<string>? TextLines { get; set; }
        public IEnumerable<int>? MultipleNumbers { get; set; }

        public IEnumerable<float>? MultipleRealNumbers { get; set; }

        public IEnumerable<bool>? BunchOfStatements { get; set; }

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
            HashCode.Combine(
                SomeText, ThisIsNumber, ThisIsFloatingPointNumber, SomeLogicalStatement,
                TextLines, MultipleNumbers, MultipleRealNumbers, BunchOfStatements);

        public override string ToString() =>
            $"{nameof(SomeText)}: {SomeText}, " +
            $"{nameof(ThisIsNumber)}: {ThisIsNumber}, " +
            $"{nameof(ThisIsFloatingPointNumber)}: {ThisIsFloatingPointNumber}, " +
            $"{nameof(SomeLogicalStatement)}: {SomeLogicalStatement}, " +
            $"{nameof(TextLines)}: {TextLines}, " +
            $"{nameof(MultipleNumbers)}: {MultipleNumbers}, " +
            $"{nameof(MultipleRealNumbers)}: {MultipleRealNumbers}, " +
            $"{nameof(BunchOfStatements)}: {BunchOfStatements}";

        public bool Equals(Entity? other)
        {
            if (other is null)
                return false;

            return SomeText == other.SomeText
                && ThisIsNumber == other.ThisIsNumber
                && ThisIsFloatingPointNumber.Equals(ThisIsFloatingPointNumber)
                && SomeLogicalStatement == other.SomeLogicalStatement && TextLines is not null &&
                other.TextLines is not null && TextLines.SequenceEqual(other.TextLines) &&
                MultipleNumbers is not null && other.MultipleNumbers is not null &&
                MultipleNumbers.SequenceEqual(other.MultipleNumbers) && MultipleRealNumbers is not null &&
                other.MultipleRealNumbers is not null && MultipleRealNumbers.SequenceEqual(other.MultipleRealNumbers) &&
                BunchOfStatements is not null && other.BunchOfStatements is not null &&
                BunchOfStatements.SequenceEqual(other.BunchOfStatements);
        }
    }
}
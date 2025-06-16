// filepath: DDDSample1/Domain/Shared/FilePath.cs
using DDDSample1.Domain.Shared; // Assuming IValueObject and BusinessRuleValidationException are here
using System;
using System.IO; // Required for Path.GetInvalidPathChars()
using System.Linq; // Required for LINQ operations like Any()

namespace DDDSample1.Domain.Shared
{
    public class FilePath : IValueObject
    {
        public string Value { get; private set; }

        private FilePath() { } // For ORM or deserialization

        public FilePath(string value)
        {
            // Allow "None" as a special case, similar to the Url class
            if (!string.Equals(value, "None", StringComparison.OrdinalIgnoreCase) && 
                !string.Equals(value, "N/A", StringComparison.OrdinalIgnoreCase)) // Added N/A as another common placeholder
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new BusinessRuleValidationException("File path cannot be empty or whitespace.");
                }

                // Check for invalid path characters.
                // Path.GetInvalidPathChars() returns characters that are not allowed in path names.
                // Path.GetInvalidFileNameChars() returns characters not allowed in file names (could also be used if it's specifically a file name).
                // For a general path (which might include directory parts), GetInvalidPathChars is more appropriate.
                var invalidChars = Path.GetInvalidPathChars();
                if (value.Any(c => invalidChars.Contains(c)))
                {
                    throw new BusinessRuleValidationException($"File path '{value}' contains invalid characters.");
                }

                // Optional: You might want to add more specific rules,
                // e.g., disallowing overly long paths, or enforcing a specific root,
                // but basic character validation is a good start.
                // For example, checking for rooted paths if necessary:
                // if (!Path.IsPathRooted(value) && !value.StartsWith("..") && !value.StartsWith("./"))
                // {
                //     // This depends on whether you expect absolute or relative paths
                //     // For now, allowing both.
                // }
            }

            this.Value = value;
        }

        public override string ToString() => Value;

        public string AsString() => Value;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            FilePath other = (FilePath)obj;
            return Value == other.Value;
        }

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        // Optional: Implicit conversion to string
        public static implicit operator string(FilePath filePath)
        {
            return filePath?.Value;
        }

        // Optional: Explicit conversion from string
        public static explicit operator FilePath(string value)
        {
            return new FilePath(value);
        }
    }
}

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Faque.Common.AspNet;

/// <summary>
/// Validates that dictionary keys match a specified regular expression.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class KeyValidationRegexAttribute : ValidationAttribute
{
    private readonly Regex _regex;

    public KeyValidationRegexAttribute([RegexPattern] string regex)
        : base("Value '{0}' is an invalid key. The key must match the regular expression '{1}'.")
    {
        _regex = new Regex(regex);
    }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value is not IDictionary dictionary)
        {
            throw new InvalidOperationException("Value is not a dictionary");
        }

        foreach (DictionaryEntry entry in dictionary)
        {
            if (entry.Key is not string key)
            {
                continue;
            }

            if (!_regex.IsMatch(key))
            {
                var errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    ErrorMessageString,
                    key,
                    _regex.ToString());
                return new ValidationResult(errorMessage, [validationContext.MemberName!]);
            }
        }

        return ValidationResult.Success;
    }
}

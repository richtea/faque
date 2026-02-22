using System.Collections;
using System.ComponentModel.DataAnnotations;
using Faque.Common.AspNet;

namespace Faque.Tests.Common.AspNet;

public class KeyValidationRegexAttributeFixture
{
    private const string TestRegexPattern = "^[a-z0-9_-]+$";

    private readonly KeyValidationRegexAttribute _attribute;

    public KeyValidationRegexAttributeFixture()
    {
        _attribute = new KeyValidationRegexAttribute(TestRegexPattern);
    }

    [Fact]
    public void should_return_success_for_null_value()
    {
        // **** ARRANGE ****
        var context = new ValidationContext(new object());

        // **** ACT ****
        var result = _attribute.GetValidationResult(null, context);

        // **** ASSERT ****
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void should_throw_invalid_operation_exception_for_non_dictionary_value()
    {
        // **** ARRANGE ****
        var value = "not a dictionary";
        var context = new ValidationContext(value);

        // **** ACT ****
        var act = () => _attribute.GetValidationResult(value, context);

        // **** ASSERT ****
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Value is not a dictionary");
    }

    [Fact]
    public void should_return_success_for_empty_dictionary()
    {
        // **** ARRANGE ****
        var value = new Dictionary<string, string>();
        var context = new ValidationContext(value);

        // **** ACT ****
        var result = _attribute.GetValidationResult(value, context);

        // **** ASSERT ****
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void should_return_success_for_dictionary_with_matching_keys()
    {
        // **** ARRANGE ****
        var value = new Dictionary<string, string>
        {
            ["valid-key"] = "value1",
            ["another_key_123"] = "value2",
        };
        var context = new ValidationContext(value);

        // **** ACT ****
        var result = _attribute.GetValidationResult(value, context);

        // **** ASSERT ****
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void should_return_failure_for_dictionary_with_non_matching_key()
    {
        // **** ARRANGE ****
        var value = new Dictionary<string, string>
        {
            ["valid-key"] = "value1",
            ["Invalid Key!"] = "value2",
        };
        var context = new ValidationContext(value) { MemberName = "TestProperty" };

        // **** ACT ****
        var result = _attribute.GetValidationResult(value, context);

        // **** ASSERT ****
        result.Should().NotBe(ValidationResult.Success);
        result.ErrorMessage.Should().Be(
            $"Value 'Invalid Key!' is an invalid key. The key must match the regular expression '{TestRegexPattern}'.");
        result.MemberNames.Should().Contain("TestProperty");
    }

    [Fact]
    public void should_ignore_non_string_keys()
    {
        // **** ARRANGE ****
        var value = new Hashtable
        {
            [123] = "value1", // Non-string key
            ["valid-key"] = "value2",
        };
        var context = new ValidationContext(value);

        // **** ACT ****
        var result = _attribute.GetValidationResult(value, context);

        // **** ASSERT ****
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void should_return_failure_if_any_key_does_not_match()
    {
        // **** ARRANGE ****
        var value = new Dictionary<string, string>
        {
            ["invalid space"] = "value1",
            ["valid"] = "value2",
        };
        var context = new ValidationContext(value);

        // **** ACT ****
        var result = _attribute.GetValidationResult(value, context);

        // **** ASSERT ****
        result.Should().NotBe(ValidationResult.Success);
        result.ErrorMessage.Should().Be(
            $"Value 'invalid space' is an invalid key. The key must match the regular expression '{TestRegexPattern}'.");
    }
}

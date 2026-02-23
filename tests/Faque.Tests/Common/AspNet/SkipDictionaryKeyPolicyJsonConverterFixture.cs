using System.Text.Json;
using System.Text.Json.Serialization;
using Faque.Common.AspNet;

namespace Faque.Tests.Common.AspNet;

#pragma warning disable CA1869

public class SkipDictionaryKeyPolicyJsonConverterFixture
{
    private readonly SkipDictionaryKeyPolicyJsonConverter _factory = new();

    [Fact]
    public void can_convert_should_return_true_for_dictionary_with_string_key()
    {
        // **** ACT ****
        var result = _factory.CanConvert(typeof(Dictionary<string, int>));

        // **** ASSERT ****
        result.Should().BeTrue();
    }

    [Fact]
    public void can_convert_should_return_false_for_dictionary_with_non_string_key()
    {
        // **** ACT ****
        var result = _factory.CanConvert(typeof(Dictionary<int, string>));

        // **** ASSERT ****
        result.Should().BeFalse();
    }

    [Fact]
    public void can_convert_should_return_false_for_non_dictionary_type()
    {
        // **** ACT ****
        var result = _factory.CanConvert(typeof(List<string>));

        // **** ASSERT ****
        result.Should().BeFalse();
    }

    [Fact]
    public void create_converter_should_return_converter_for_valid_dictionary()
    {
        // **** ARRANGE ****
        var options = new JsonSerializerOptions();

        // **** ACT ****
        var converter = _factory.CreateConverter(typeof(Dictionary<string, string>), options);

        // **** ASSERT ****
        converter.Should().NotBeNull();
        converter.Should().BeAssignableTo<JsonConverter<Dictionary<string, string>>>();
    }

    [Fact]
    public void serialize_should_skip_key_policy()
    {
        // **** ARRANGE ****
        var options = new JsonSerializerOptions
        {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };

        var model = new TestModel
        {
            Data = new Dictionary<string, string>
            {
                { "KeepMyCasing", "Value" },
                { "another_Key", "Value2" },
            },
        };

        // **** ACT ****
        var json = JsonSerializer.Serialize(model, options);

        // **** ASSERT ****
        json.Should().Contain("\"KeepMyCasing\"");
        json.Should().Contain("\"another_Key\"");
        json.Should().NotContain("\"keepMyCasing\"");
    }

    [Fact]
    public void deserialize_should_work_correctly()
    {
        // **** ARRANGE ****
        const string Json = "{\"Data\":{\"KeepMyCasing\":\"Value1\",\"another_Key\":\"Value2\"}}";
        var options = new JsonSerializerOptions();

        // **** ACT ****
        var model = JsonSerializer.Deserialize<TestModel>(Json, options);

        // **** ASSERT ****
        model.Should().NotBeNull();
        model.Data.Should().HaveCount(2);
        model.Data.Should().ContainKey("KeepMyCasing").WhoseValue.Should().Be("Value1");
        model.Data.Should().ContainKey("another_Key").WhoseValue.Should().Be("Value2");
    }

    [Fact]
    public void deserialize_should_handle_null_dictionary()
    {
        // **** ARRANGE ****
        const string Json = "{\"Data\":null}";
        var options = new JsonSerializerOptions();

        // **** ACT ****
        var model = JsonSerializer.Deserialize<TestModel>(Json, options);

        // **** ASSERT ****
        model.Should().NotBeNull();
        model.Data.Should().BeNull();
    }

    [Fact]
    public void deserialize_should_handle_empty_dictionary()
    {
        // **** ARRANGE ****
        const string Json = "{\"Data\":{}}";
        var options = new JsonSerializerOptions();

        // **** ACT ****
        var model = JsonSerializer.Deserialize<TestModel>(Json, options);

        // **** ASSERT ****
        model.Should().NotBeNull();
        model.Data.Should().BeEmpty();
    }

    [Fact]
    public void deserialize_should_handle_int_values()
    {
        // **** ARRANGE ****
        const string Json = "{\"Data\":{\"key1\":123,\"key2\":456}}";
        var options = new JsonSerializerOptions();

        // **** ACT ****
        var model = JsonSerializer.Deserialize<TestModelWithInt>(Json, options);

        // **** ASSERT ****
        model.Should().NotBeNull();
        model.Data.Should().HaveCount(2);
        model.Data["key1"].Should().Be(123);
        model.Data["key2"].Should().Be(456);
    }

    [Fact]
    public void deserialize_should_throw_on_null_value_for_non_nullable_type()
    {
        // **** ARRANGE ****
        const string Json = "{\"Data\":{\"key1\":null}}";
        var options = new JsonSerializerOptions();

        // **** ACT ****
        var act = () => JsonSerializer.Deserialize<TestModelWithInt>(Json, options);

        // **** ASSERT ****
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void deserialize_should_handle_null_value_for_nullable_type()
    {
        // **** ARRANGE ****
        const string Json = "{\"Data\":{\"key1\":null,\"key2\":789}}";
        var options = new JsonSerializerOptions();

        // **** ACT ****
        var model = JsonSerializer.Deserialize<TestModelWithNullableInt>(Json, options);

        // **** ASSERT ****
        model.Should().NotBeNull();
        model.Data.Should().HaveCount(2);
        model.Data["key1"].Should().BeNull();
        model.Data["key2"].Should().Be(789);
    }

    [Fact]
    public void deserialize_should_throw_on_invalid_json_structure()
    {
        // **** ARRANGE ****
        const string Json = "{\"Data\":[1,2,3]}"; // Should be an object, not an array
        var options = new JsonSerializerOptions();

        // **** ACT ****
        var act = () => JsonSerializer.Deserialize<TestModel>(Json, options);

        // **** ASSERT ****
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void deserialize_should_throw_on_missing_end_object()
    {
        // **** ARRANGE ****
        var json = "{\"Data\":{\"key\":\"value\""; // Missing closing braces
        var options = new JsonSerializerOptions();

        // **** ACT ****
        var act = () => JsonSerializer.Deserialize<TestModel>(json, options);

        // **** ASSERT ****
        act.Should().Throw<JsonException>();
    }

    private class TestModel
    {
        [SkipDictionaryKeyPolicy]
        public Dictionary<string, string> Data { get; init; } = [];
    }

    private class TestModelWithInt
    {
        [SkipDictionaryKeyPolicy]
        public Dictionary<string, int> Data { get; init; } = [];
    }

    private class TestModelWithNullableInt
    {
        [SkipDictionaryKeyPolicy]
        public Dictionary<string, int?> Data { get; init; } = [];
    }
}

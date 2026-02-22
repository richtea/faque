using Faque.Middleware;
using Microsoft.OpenApi;

namespace Faque.Tests.Middleware;

public class RemoveStandardWordingSchemaTransformerFixture
{
    private readonly RemoveStandardWordingSchemaTransformer _transformer;

    public RemoveStandardWordingSchemaTransformerFixture()
    {
        _transformer = new RemoveStandardWordingSchemaTransformer();
    }

    [Theory]
    [InlineData("Gets or sets the name.", "The name.")]
    [InlineData("Gets the name.", "The name.")]
    [InlineData("Sets the name.", "The name.")]
    public async Task should_remove_standard_prefixes_and_capitalize_first_letter(string original, string expected)
    {
        // **** ARRANGE ****
        var schema = new OpenApiSchema
        {
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                { "name", new OpenApiSchema { Description = original } },
            },
        };

        // **** ACT ****
        await _transformer.TransformAsync(schema, null!, CancellationToken.None);

        // **** ASSERT ****
        schema.Properties["name"].Description.Should().Be(expected);
    }

    [Theory]
    [InlineData("GETS OR SETS the name.", "The name.")]
    [InlineData("gets the name.", "The name.")]
    [InlineData("sETs the name.", "The name.")]
    public async Task should_be_case_insensitive_when_removing_prefixes(string original, string expected)
    {
        // **** ARRANGE ****
        var schema = new OpenApiSchema
        {
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                { "name", new OpenApiSchema { Description = original } },
            },
        };

        // **** ACT ****
        await _transformer.TransformAsync(schema, null!, CancellationToken.None);

        // **** ASSERT ****
        schema.Properties["name"].Description.Should().Be(expected);
    }

    [Fact]
    public async Task should_not_change_description_without_standard_prefix()
    {
        // **** ARRANGE ****
        const string original = "The user's name.";
        var schema = new OpenApiSchema
        {
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                { "name", new OpenApiSchema { Description = original } },
            },
        };

        // **** ACT ****
        await _transformer.TransformAsync(schema, null!, CancellationToken.None);

        // **** ASSERT ****
        schema.Properties["name"].Description.Should().Be(original);
    }

    [Fact]
    public async Task should_handle_null_description()
    {
        // **** ARRANGE ****
        var schema = new OpenApiSchema
        {
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                { "name", new OpenApiSchema { Description = null } },
            },
        };

        // **** ACT ****
        await _transformer.TransformAsync(schema, null!, CancellationToken.None);

        // **** ASSERT ****
        schema.Properties["name"].Description.Should().BeNull();
    }

    [Fact]
    public async Task should_handle_empty_properties()
    {
        // **** ARRANGE ****
        var schema = new OpenApiSchema
        {
            Properties = new Dictionary<string, IOpenApiSchema>(),
        };

        // **** ACT ****
        var act = () => _transformer.TransformAsync(schema, null!, CancellationToken.None);

        // **** ASSERT ****
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task should_handle_null_properties()
    {
        // **** ARRANGE ****
        var schema = new OpenApiSchema
        {
            Properties = null,
        };

        // **** ACT ****
        var act = () => _transformer.TransformAsync(schema, null!, CancellationToken.None);

        // **** ASSERT ****
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task should_handle_description_equal_to_prefix()
    {
        // **** ARRANGE ****
        var schema = new OpenApiSchema
        {
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                { "name", new OpenApiSchema { Description = "Gets " } },
            },
        };

        // **** ACT ****
        await _transformer.TransformAsync(schema, null!, CancellationToken.None);

        // **** ASSERT ****
        schema.Properties["name"].Description.Should().BeEmpty();
    }
}

namespace Faque.Models;

/// <summary>
/// Request model for creating/updating routes.
/// </summary>
public class RouteUpdateRequestModel
{
    public RouteResponse Response { get; set; } = new();

    public bool Enabled { get; set; } = true;
}

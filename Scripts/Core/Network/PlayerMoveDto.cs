public record PlayerMoveDto
{
    public string Id { get; init; }
    public float X { get; init; }
    public float Y { get; init; }
    public float DirX { get; init; }
    public float DirY { get; init; }
    public string State { get; init; }
}
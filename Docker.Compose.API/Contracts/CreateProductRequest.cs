namespace Docker.Compose.API.Contracts;

public sealed class CreateProductRequest
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }
}
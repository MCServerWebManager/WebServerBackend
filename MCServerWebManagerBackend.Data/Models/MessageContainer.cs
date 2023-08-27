namespace MCServerWebManagerBackend.Data.Models;

public struct MessageContainer<T>
{
    public T? Data { get; set; }

    public string? Message { get; set; }

    public bool Success { get; set; }
}
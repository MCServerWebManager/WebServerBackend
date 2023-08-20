namespace MCServerWebManagerBackend.Data.Models;

public struct MessageContainer<T>
{
    public T? ReturnData { get; set; }

    public string? Message { get; set; }

    public bool Success { get; set; }
}
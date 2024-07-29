namespace Domain.Entities;

public class User
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public int Money { get; set; } = 100;
}
namespace Domain.Entities;

public class User
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public int Elo { get; set; } = 600;
    public int Money { get; set; } = 100;
}
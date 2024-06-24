namespace Application.Common.Interfaces;

public interface IAuthorizedUser
{
    string? RequestedUserId { get; set; }
    string? RequestedUserName { get; set; }
}

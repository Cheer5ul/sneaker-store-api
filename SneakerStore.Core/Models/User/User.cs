using SneakerStore.Core.Results;
using SneakerStore.Core.Results.Errors.User;

namespace SneakerStore.Core.Models.User;

public class User
{
    public const int MAX_USERNAME_LENGTH = 100;
    public const int MAX_EMAIL_LENGTH = 100;
    
    private User(Guid id, string username, string email, string passwordHash)
    {
        Id = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
    }
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }

    public static Result<User> Create(Guid id, string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length > MAX_USERNAME_LENGTH)
        {
            return Result<User>.Failure([UserErrors.InvalidUsername(username)]);
        }
        else if (string.IsNullOrWhiteSpace(email) || email.Length > MAX_EMAIL_LENGTH)
        {
            return Result<User>.Failure([UserErrors.InvalidEmail(email)]);
        }

        User user = new User(id, username, email, passwordHash)
        {
            Id = id,
            Email = email,
            PasswordHash = passwordHash,
            Username = username
        };
        
        return Result<User>.Success(user);
    }
}
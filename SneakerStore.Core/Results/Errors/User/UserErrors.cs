namespace SneakerStore.Core.Results.Errors.User;

public static class UserErrors
{
    private static class Codes
    {
        public const string InvalidUsername = "User.InvalidName";
        public const string InvalidEmail = "User.InvalidEmail";
    }
    public static Error InvalidUsername(string name) =>
        new Error(Codes.InvalidUsername,
            $"Username {name} is invalid.");
    public static Error InvalidEmail(string email) =>
        new Error(Codes.InvalidEmail,
            $"Email {email} is invalid.");
}
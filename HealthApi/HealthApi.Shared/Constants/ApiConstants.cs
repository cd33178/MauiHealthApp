namespace HealthApi.Shared.Constants;
public static class ApiConstants
{
    public static class Scopes
    {
        public const string Read = "health.read";
        public const string Write = "health.write";
        public const string Admin = "health.admin";
    }

    public static class Roles
    {
        public const string User = "User";
        public const string Admin = "Admin";
        public const string Doctor = "Doctor";
    }

    public static class Policies
    {
        public const string CanRead = "CanRead";
        public const string CanWrite = "CanWrite";
        public const string IsAdmin = "IsAdmin";
    }
}

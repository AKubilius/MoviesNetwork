namespace Bakis.Auth.Model
{
    public class Roles
    {
        public const string Admin = nameof(Admin);
        public const string User = nameof(User);

        public static readonly IReadOnlyCollection<string> All = new[] { Admin, User };
        public static string[] Admins = new[] { Admin };
    }
}

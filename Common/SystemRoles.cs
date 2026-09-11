namespace HelpEmpowermentApi.Common;

public static class SystemRoles
{
    public static readonly Guid AdminId = Guid.Parse("A1000000-0000-0000-0000-000000000001");
    public static readonly Guid TrainerId = Guid.Parse("A1000000-0000-0000-0000-000000000002");

    public const string Admin = "Admin";
    public const string Trainer = "Trainer";
}

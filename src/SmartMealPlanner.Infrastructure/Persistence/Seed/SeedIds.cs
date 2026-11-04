namespace SmartMealPlanner.Infrastructure.Persistence.Seed;

internal static class SeedIds
{
    public static readonly Guid UserRoleId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid AdminRoleId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public static readonly Guid ProduceCategoryId = Guid.Parse("c1000001-0000-0000-0000-000000000001");
    public static readonly Guid DairyCategoryId = Guid.Parse("c1000001-0000-0000-0000-000000000002");
    public static readonly Guid MeatCategoryId = Guid.Parse("c1000001-0000-0000-0000-000000000003");
    public static readonly Guid PantryStaplesCategoryId = Guid.Parse("c1000001-0000-0000-0000-000000000004");
    public static readonly Guid FrozenCategoryId = Guid.Parse("c1000001-0000-0000-0000-000000000005");
    public static readonly Guid BeveragesCategoryId = Guid.Parse("c1000001-0000-0000-0000-000000000006");
    public static readonly Guid OtherCategoryId = Guid.Parse("c1000001-0000-0000-0000-000000000099");

    public static readonly DateTimeOffset SeedTimestamp = new(2026, 6, 1, 0, 0, 0, TimeSpan.Zero);
}

using FluentAssertions;
using SmartMealPlanner.Domain.Enums;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class DomainEnumTests
{
    [Fact]
    public void HouseholdRole_HasOwnerAndMember()
    {
        Enum.GetValues<HouseholdRole>().Should().BeEquivalentTo([
            HouseholdRole.Owner,
            HouseholdRole.Member
        ]);
    }
}

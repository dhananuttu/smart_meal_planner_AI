using SmartMealPlanner.Application.Auth.Models;

namespace SmartMealPlanner.Application.Auth.Interfaces;

public interface ITokenService
{
    TokenPair GenerateTokens(AuthUser user, UserHouseholdInfo householdInfo, string platformRole);
}

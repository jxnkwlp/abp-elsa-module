using Microsoft.AspNetCore.Identity;

namespace Passingwind.WorkflowApp.Web.Controllers;

public static class LoginHelper
{
    public static AbpLoginResult GetAbpLoginResult(SignInResult result)
    {
        if (result.IsLockedOut)
        {
            return new AbpLoginResult(LoginResultType.LockedOut);
        }

        if (result.RequiresTwoFactor)
        {
            return new AbpLoginResult(LoginResultType.RequiresTwoFactor);
        }

        if (result.IsNotAllowed)
        {
            return new AbpLoginResult(LoginResultType.NotAllowed);
        }

        if (!result.Succeeded)
        {
            return new AbpLoginResult(LoginResultType.InvalidUserNameOrPassword);
        }

        return new AbpLoginResult(LoginResultType.Success);
    }
}

public class AbpLoginResult
{
    public AbpLoginResult(LoginResultType result)
    {
        Result = result;
    }

    public LoginResultType Result { get; }

    public string Description => Result.ToString();
}

public enum LoginResultType : byte
{
    Success = 1,

    InvalidUserNameOrPassword = 2,

    NotAllowed = 3,

    LockedOut = 4,

    RequiresTwoFactor = 5
}

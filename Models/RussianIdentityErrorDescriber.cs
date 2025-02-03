using Microsoft.AspNetCore.Identity;

namespace AuthApp.Models;

public class RussianIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError PasswordRequiresDigit()
        => new IdentityError { Description = "Минимум 1 цифра" };

    public override IdentityError PasswordRequiresUpper()
        => new IdentityError { Description = "Минимум 1 большая буква" };

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => new IdentityError { Description = "Минимум 1 специальный символ" };

    public override IdentityError PasswordTooShort(int length)
        => new IdentityError { Description = "Минимум 6 символов" };

    public override IdentityError PasswordRequiresLower()
        => new IdentityError { Description = "Минимум 1 строчная буква" };
} 
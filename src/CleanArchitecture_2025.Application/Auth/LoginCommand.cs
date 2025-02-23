using CleanArchitecture_2025.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TS.Result;

namespace CleanArchitecture_2025.Application.Auth
{
    public sealed record LoginCommand
    (
        string UserNameOrEmail,
        string Password
    ) : IRequest<Result<LoginCommandResponse>>;

    public sealed record LoginCommandResponse
    {
        public string AccessToken { get; set; } = default!;
    }

    internal sealed class LoginCommandHandler(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager) : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
    {
        public async Task<Result<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            AppUser? user = await userManager.Users.FirstOrDefaultAsync(p => p.Email == request.UserNameOrEmail || p.UserName == request.UserNameOrEmail, cancellationToken);

            if (user is null)
            {
                return Result<LoginCommandResponse>.Failure("User cannot found");
            }

            SignInResult signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);

            if (signInResult.IsLockedOut)
            {
                TimeSpan? timeSpan = user.LockoutEnd - DateTime.UtcNow;
                if (timeSpan is not null)
                    return (500, $"You have entered your password incorrectly 3 times.Blocked for minutes {Math.Ceiling(timeSpan.Value.TotalMinutes)}");
                else
                    return (500, "Your user has been blocked for 5 minutes because they entered the wrong password 3 times.");
            }

            if (signInResult.IsNotAllowed)
            {
                return (500, "Mail address is not confirmed.");
            }

            if (!signInResult.Succeeded)
            {
                return (500, "Username or password is wrong.");
            }

            var response = new LoginCommandResponse()
            {
                AccessToken = ""
            };

            return  response;
        }
    }
}

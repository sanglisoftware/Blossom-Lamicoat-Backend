using Api.Application.DTOs;
using Api.Application.Interfaces;

namespace Api.API.EndPoints.Inventory
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (AuthRequestDto request, IAuthService authService) =>
            {
                var result = await authService.AuthenticateAsync(request);
                return result is null ? Results.Unauthorized() : Results.Ok(result);
            });

            app.MapPut("/change-password", async (string oldPassword, string newPassword, IAuthService authService) =>
            {
                try
                {
                    var result = await authService.ChangeExistingPassword(oldPassword, newPassword);
                    return result ? Results.Ok() : Results.BadRequest("Failed to change password.");
                }
                catch (Exception ex) { return Results.Problem(ex.Message); }
            }).RequireAuthorization();
        }
    }
}

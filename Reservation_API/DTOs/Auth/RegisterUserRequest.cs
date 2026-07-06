namespace Reservation_API.DTOs.Auth
{
    public record RegisterUserRequest(Guid TenantId, string Email, string Password);
}

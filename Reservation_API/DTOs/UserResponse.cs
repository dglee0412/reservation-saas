namespace Reservation_API.DTOs
{
    public record UserResponse(Guid Id, string Email, Guid TenantId);
}

using ApiContracts.Enums;

namespace ApiContracts.Dtos.User;

public record LoginDto(int Id, string Email, UserRole UserRole);
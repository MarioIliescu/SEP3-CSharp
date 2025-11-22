using ApiContracts.Enums;

namespace ApiContracts.Dtos.Auth;

public record LoginDto(int Id,string Email, UserRole UserRole){}
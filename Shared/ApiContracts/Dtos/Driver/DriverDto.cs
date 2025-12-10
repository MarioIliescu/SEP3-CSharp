using ApiContracts.Enums;

namespace ApiContracts.Dtos.Driver;

public record DriverDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,

    // User role (forced to DRIVER by builder but included for completeness)
    UserRole Role,
    string PhotoUrl,
    // Driver-specific fields
    string McNumber,
    DriverStatus Status,
    TrailerType TrailerType,
    string LocationState,
    int LocationZip,
    DriverCompanyRole CompanyRole
);
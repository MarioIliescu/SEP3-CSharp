using ApiContracts.Enums;

namespace ApiContracts.Dtos.Driver;

public record CreateDriverDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string McNumber,
    TrailerType TrailerType,
    DriverStatus StatusType,
    DriverCompanyRole CompanyRole,
    string LocationState,
    int LocationZipCode
);
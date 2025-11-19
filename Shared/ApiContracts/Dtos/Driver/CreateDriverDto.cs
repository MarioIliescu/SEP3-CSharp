
using ApiContracts.Enums;

namespace ApiContracts.Dtos.Driver;

public record CreateDriverDto
(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string McNumber, 
    DriverStatus Status,
    TrailerType Type,
    string LocationState,
    int LocationZipCode
);
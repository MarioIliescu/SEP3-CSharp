using ApiContracts.Enums;

namespace ApiContracts.Dtos.Driver;

public record DriverDto
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
    int LocationZipCode,
    string CompanyName
    
);
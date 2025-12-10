namespace ApiContracts.Dtos.Dispatcher;

public record DispatcherDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    double CurrentRate,
    string PhotoUrl
);
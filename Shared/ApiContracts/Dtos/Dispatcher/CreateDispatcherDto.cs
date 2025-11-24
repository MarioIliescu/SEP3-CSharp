namespace ApiContracts.Dtos.Dispatcher;

public record CreateDispatcherDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    double CurrentRate,
    string Password
);
namespace ApiContracts.Dtos.Dispatcher;

public record CreateDispatcherDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    int CurrentRate
);

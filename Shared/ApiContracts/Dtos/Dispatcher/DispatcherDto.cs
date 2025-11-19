namespace ApiContracts.Dtos.Dispatcher;

public record DispatcherDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    int CurrentRate
);

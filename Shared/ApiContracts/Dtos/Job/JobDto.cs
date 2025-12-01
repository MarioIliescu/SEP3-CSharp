using ApiContracts.Enums;

namespace ApiContracts.Dtos.Job;

public record JobDto(
    int Id,
    int DispatcherId, 
    int DriverId,
    string Title, 
    string Description, 
    int loadedMiles, 
    int weightOfCargo,
    TrailerType TypeOfTrailerNeeded, 
    int TotalPrice, 
    string CargoInfo, 
    DateTime PickupTime,
    DateTime DeliveryTime,
    string PickupLocationState, 
    int PickupLocationZip,
    string DropLocationState, 
    int DropLocationZip,
    JobStatus CurrentStatus
    );
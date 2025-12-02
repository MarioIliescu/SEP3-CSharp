using ApiContracts.Enums;

namespace ApiContracts.Dtos.Job;

public record CreateJobDto(
    int DispatcherId, 
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
    int DropLocationZip
    );
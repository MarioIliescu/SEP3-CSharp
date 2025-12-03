using ApiContracts.Enums;

namespace ApiContracts.Dtos.Job;

public record UpdateJobDto
{
    public int Id { get; set; }
    public int DispatcherId { get; set; }
    public int DriverId { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int LoadedMiles { get; set; }
    public int WeightOfCargo { get; set; }
    public TrailerType TypeOfTrailerNeeded { get; set; }
    public int TotalPrice { get; set; }
    public string CargoInfo { get; set; } = "";
    public DateTime PickupTime { get; set; }
    public DateTime DeliveryTime { get; set; }
    public string PickupLocationState { get; set; } = "";
    public int PickupLocationZip { get; set; }
    public string DropLocationState { get; set; } = "";
    public int DropLocationZip { get; set; }
    public JobStatus CurrentStatus { get; set; }
}

using System.ComponentModel;
using ApiContracts.Enums;
using Entities;
using Google.Protobuf.WellKnownTypes;
using GrpcAPI.Protos;

namespace GrpcAPI.GrpcUtils;

public static class ProtoUtils
{
    public static Driver ParseFromProtoToDriver(DriverProto proto)
    {
        return new Driver.Builder()
            .SetId(proto.User.Id)
            .SetLocationState(proto.CurrentState)
            .SetLocationZip(proto.CurrentZIPCODE)
            .SetTrailerType(ParseTrailerTypeProtoToTrailerType(proto.TrailerType))
            .SetCompanyRole(ParseFromDriverCompanyRoleProtoToCompanyRole(proto.CompanyRole))
            .SetStatus(ParseDriverStatusProtoToDriverStatus(proto.DriverStatus))
            .SetMcNumber(proto.CompanyMcNumber)
            .SetEmail(proto.User.Email)
            .SetFirstName(proto.User.FirstName)
            .SetLastName(proto.User.LastName)
            .SetPassword(proto.User.Password)
            .SetRole(UserRole.Driver)
            .Build();
    }

    public static Company ParseFromProtoToCompany(CompanyProto proto)
    {
        return new Company.Builder()
            .SetCompanyName(proto.CompanyName
            ).SetMcNumber(proto.McNumber)
            .Build();
    }
    
    public static Job ParseFromProtoToJob(JobProto proto)
    {
        return new Job.Builder()
            .SetId(proto.JobId)
            .SetDispatcherId(proto.JobDispatcherId)
            .SetDriverId(proto.JobDriverId)
            .SetTitle(proto.Title)
            .SetDescription(proto.Description)
            .SetLoadedMiles(proto.LoadedMiles)
            .SetWeight(proto.WeightOfCargo)
            .SetTrailerType(ParseTrailerTypeProtoToTrailerType(proto.JobTrailerType))
            .SetTotalPrice(proto.TotalPrice)
            .SetCargoInfo(proto.CargoInfo)
            .SetPickupTime(proto.PickUpTime.ToDateTime())
            .SetDeliveryTime(proto.DeliveryTime.ToDateTime())
            .SetPickupState(proto.PickUpLocationState)
            .SetPickupZip(proto.PickUpLocationZipCode)
            .SetDropState(proto.DropLocationState)
            .SetDropZip(proto.DropLocationZipCode)
            .SetStatus(ParseJobStatusProtoToJobStatus(proto.CurrentJobStatus))
            .Build();
    }

    public static RequestProto ParseCompanyRequest(ActionTypeProto proto, Company company)
    {
        return new RequestProto()
        {
            Handler = HandlerTypeProto.HandlerCompany,
            Action = proto,
            Payload = Any.Pack(ParseCompanyToProto(company)),
        };
    }

    public static RequestProto ParseJobRequest(ActionTypeProto proto, Job job)
    {
        return new RequestProto()
        {
            Handler = HandlerTypeProto.HandlerJob,
            Action = proto,
            Payload = Any.Pack(ParseJobToProto(job))
        };
    }
    


    public static DriverStatus ParseDriverStatusProtoToDriverStatus(StatusDriverProto proto)
    {
        switch (proto)
        {
            case StatusDriverProto.Available:
                return DriverStatus.Available;
            case StatusDriverProto.Busy:
                return DriverStatus.Busy;
            case StatusDriverProto.OffDuty:
                return DriverStatus.Off_duty;
            default:
                throw new InvalidEnumArgumentException("Invalid status driver status");
        }
    }
    public static JobStatus ParseJobStatusProtoToJobStatus(JobStatusProto proto)
    {
        switch (proto)
        {
            case JobStatusProto.JobAvailable:
                return JobStatus.Available;
            case JobStatusProto.JobAssigned:
                return JobStatus.Assigned;
            case JobStatusProto.JobOngoing:
                return JobStatus.Ongoing;
            case JobStatusProto.JobCompleted:
                return JobStatus.Completed;
            case JobStatusProto.JobExpired:
                return JobStatus.Expired;
            case JobStatusProto.JobAccepted :
                return JobStatus.Accepted;
            case JobStatusProto.JobLoading:
                return JobStatus.Loading;
            case JobStatusProto.JobUnloading:
                return JobStatus.Unloading;
            default:
                throw new InvalidEnumArgumentException("Invalid job status");
        }
    }

    public static DriverCompanyRole ParseFromDriverCompanyRoleProtoToCompanyRole(DriverCompanyRoleProto proto)
    {
        switch (proto)
        {
            case DriverCompanyRoleProto.Driver:
                return DriverCompanyRole.Driver;
            case DriverCompanyRoleProto.OwnerOperator:
                return DriverCompanyRole.OwnerOperator;
            default:
                throw new InvalidEnumArgumentException("Invalid driver company role");
        }
    }

    public static TrailerType ParseTrailerTypeProtoToTrailerType(TrailerTypeProto trailerType)
    {
        switch (trailerType)
        {
            case TrailerTypeProto.DryVan:
                return TrailerType.Dry_van;
            case TrailerTypeProto.Flatbed:
                return TrailerType.Flatbed;
            case TrailerTypeProto.Reefer:
                return TrailerType.Reefer;
            default:
                throw new InvalidEnumArgumentException("Unknown trailer type");
        }
    }

    public static RequestProto ParseDriverRequest(ActionTypeProto action, Driver payload)
    {
        return new RequestProto()
        {
            Handler = HandlerTypeProto.HandlerDriver,
            Action = action,
            Payload = Any.Pack(ParseDriverToProto(payload))
        };
    }

    public static DriverProto ParseDriverToProto(Driver payload)
    {
        int zipcode = 35010;
        if (payload.Location_Zip_Code == 0)
        {
            zipcode = payload.Location_Zip_Code;
        }

        return new DriverProto()
        {
            User = ParseUserToProto(payload),
            CompanyMcNumber = payload.McNumber ?? "DEFAULTMCN",
            CurrentState = payload.Location_State ?? "",
            DriverStatus = ParseStatusToProto(payload.Status),
            CompanyRole = ParseCompanyRoleToProto(payload.CompanyRole),
            TrailerType = ParseTrailerTypeToProto(payload.Trailer_type),
            CurrentZIPCODE = zipcode
        };
    }
    
    public static Dispatcher ParseFromProtoToDispatcher(DispatcherProto proto)
    {
        return new Dispatcher.Builder()
            .SetId(proto.User.Id)
            .SetCurrentRate(proto.CurrentRate)
            .SetEmail(proto.User.Email)
            .SetFirstName(proto.User.FirstName)
            .SetLastName(proto.User.LastName)
            .SetPassword(proto.User.Password)
            .SetRole(UserRole.Dispatcher)
            .Build();
    }
    public static RequestProto ParseDispatcherRequest(ActionTypeProto action, Dispatcher payload)
    {
        return new RequestProto()
        {
            Handler = HandlerTypeProto.HandlerDispatcher,
            Action = action,
            Payload = Any.Pack(ParseDispatcherToProto(payload))
        };
    }
    
    public static DispatcherProto ParseDispatcherToProto(Dispatcher payload)
    {
        return new DispatcherProto()
        {
            User = ParseUserToProto(payload),
            CurrentRate = payload.Current_Rate
        };
    }

    public static UserProto ParseUserToProto(User user)
    {
        return new UserProto()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            PhoneNumber = user.PhoneNumber,
            Password = user.Password,
            Role = ParseUserRoleToProto(user.Role),
        };
    }

    public static StatusDriverProto ParseStatusToProto(DriverStatus status)
    {
        switch (status)
        {
            case DriverStatus.Available:
                return StatusDriverProto.Available;
            case DriverStatus.Busy:
                return StatusDriverProto.Busy;
            case DriverStatus.Off_duty:
                return StatusDriverProto.OffDuty;
            default:
                throw new InvalidEnumArgumentException("Status unknown");
        }
    }
    public static JobStatusProto ParseJobStatusToProto(JobStatus status)
    {
        switch (status)
        {
            case JobStatus.Assigned:
                return JobStatusProto.JobAssigned;
            case JobStatus.Available:
                return JobStatusProto.JobAvailable;
            case JobStatus.Ongoing:
                return JobStatusProto.JobOngoing;
            case JobStatus.Completed:
                return JobStatusProto.JobCompleted;
            case JobStatus.Expired:
                return JobStatusProto.JobExpired;
            case JobStatus.Loading:
                return JobStatusProto.JobLoading;
            case JobStatus.Unloading:
                return JobStatusProto.JobUnloading;
            case JobStatus.Accepted:
                return JobStatusProto.JobAccepted;
            default:
                throw new InvalidEnumArgumentException("Unknown job status");
        }
    }

    public static DriverCompanyRoleProto ParseCompanyRoleToProto(DriverCompanyRole payload)
    {
        switch (payload)
        {
            case DriverCompanyRole.Driver:
                return DriverCompanyRoleProto.Driver;
            case DriverCompanyRole.OwnerOperator:
                return DriverCompanyRoleProto.OwnerOperator;
            default:
                throw new InvalidEnumArgumentException("Unknown company role");
        }
    }

    public static TrailerTypeProto ParseTrailerTypeToProto(TrailerType trailerType)
    {
        switch (trailerType)
        {
            case TrailerType.Dry_van:
                return TrailerTypeProto.DryVan;
            case TrailerType.Flatbed:
                return TrailerTypeProto.Flatbed;
            case TrailerType.Reefer:
                return TrailerTypeProto.Reefer;
            default:
                throw new InvalidEnumArgumentException("Unknown trailer type");
        }
    }

    public static CompanyProto ParseCompanyToProto(Company company)
    {
        return new CompanyProto()
        {
            CompanyName = company.CompanyName,
            McNumber = company.McNumber,
        };
    }

    public static JobProto ParseJobToProto(Job job)
    {
        return new JobProto()
        {
            JobId = job.JobId,
            JobDispatcherId = job.DispatcherId,
            JobDriverId = job.DriverId,
            Title = job.Title,
            Description = job.Description,
            LoadedMiles = job.Loaded_Miles,
            WeightOfCargo = job.Weight_Of_Cargo,
            JobTrailerType =
                ParseTrailerTypeToProto(job.Type_Of_Trailer_Needed),
            TotalPrice = job.Total_Price,
            CargoInfo = job.Cargo_Info,
            PickUpTime =
                Timestamp.FromDateTime(job.Pickup_Time.ToUniversalTime()),
            DeliveryTime =
                Timestamp.FromDateTime(job.Delivery_Time.ToUniversalTime()),
            PickUpLocationState = job.Pickup_Location_State,
            PickUpLocationZipCode = job.Pickup_Location_Zip,
            DropLocationState = job.Drop_Location_State,
            DropLocationZipCode = job.Drop_Location_Zip,
            CurrentJobStatus = ParseJobStatusToProto(job.Current_Status)
        };
    }

    public static User ParseFromProtoToUser(UserProto proto)
    {
        return new User.Builder()
            .SetId(proto.Id)
            .SetEmail(proto.Email)
            .SetFirstName(proto.FirstName)
            .SetLastName(proto.LastName)
            .SetPhoneNumber(proto.PhoneNumber)
            .SetPassword(proto.Password)
            .Build();
    }

    public static UserRoleProto ParseUserRoleToProto(UserRole role)
    {
        switch (role)
        {
            case UserRole.Driver:
                return UserRoleProto.UserDriver;
            case UserRole.Dispatcher:
                return UserRoleProto.UserDispatcher;
            default:
                throw new InvalidEnumArgumentException("Unknown role");
        }
    }

    public static UserRole ParseProtoUserRoleToUserRole(UserRoleProto role)
    {
        switch (role)
        {
            case UserRoleProto.UserDriver:
                return UserRole.Driver;
            case UserRoleProto.UserDispatcher:
                return UserRole.Dispatcher;
            default:
                throw new InvalidEnumArgumentException("Unknown role");
        }
    }
}
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
            .SetRole(UserRole.DRIVER)
            .Build();
    }

    public static Company ParseFromProtoToCompany(CompanyProto proto)
    {
        return new Company.Builder()
            .SetCompanyName(proto.CompanyName
            ).SetMcNumber(proto.McNumber)
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

    public static DriverStatus ParseDriverStatusProtoToDriverStatus(StatusDriverProto proto)
    {
        switch (proto)
        {
            case StatusDriverProto.Available:
                return DriverStatus.available;
            case StatusDriverProto.Busy:
                return DriverStatus.busy;
            case StatusDriverProto.OffDuty:
                return DriverStatus.off_duty;
            default:
                throw new InvalidEnumArgumentException("Invalid status driver status");
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
                return TrailerType.dry_van;
            case TrailerTypeProto.Flatbed:
                return TrailerType.flatbed;
            case TrailerTypeProto.Reefer:
                return TrailerType.reefer;
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
            case DriverStatus.available:
                return StatusDriverProto.Available;
            case DriverStatus.busy:
                return StatusDriverProto.Busy;
            case DriverStatus.off_duty:
                return StatusDriverProto.OffDuty;
            default:
                throw new InvalidEnumArgumentException("Status unknown");
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
            case TrailerType.dry_van:
                return TrailerTypeProto.DryVan;
            case TrailerType.flatbed:
                return TrailerTypeProto.Flatbed;
            case TrailerType.reefer:
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
            case UserRole.DRIVER:
                return UserRoleProto.UserDriver;
            case UserRole.DISPATCHER:
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
                return UserRole.DRIVER;
            case UserRoleProto.UserDispatcher:
                return UserRole.DISPATCHER;
            default:
                throw new InvalidEnumArgumentException("Unknown role");
        }
    }
}
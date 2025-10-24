using ApiContracts.Enums;

namespace ApiContracts;

public record Response(Status Status, object Payload);
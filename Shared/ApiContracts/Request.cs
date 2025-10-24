using ApiContracts.Enums;

namespace ApiContracts;

public record Request(ActionType Action, HandlerType Handler, object Payload);
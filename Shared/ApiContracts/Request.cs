using ApiContracts.Enums;

namespace ApiContracts;

public record Request(ActionType ActionType, HandlerType HandlerType, object Payload);
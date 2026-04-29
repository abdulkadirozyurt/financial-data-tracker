namespace FinancialDataTracker.Core.Exceptions;

public sealed class NotFoundException(string message) : Exception(message);

public sealed class ConflictException(string message) : Exception(message);

public sealed class ExternalServiceException(string message) : Exception(message);

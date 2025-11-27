using PackingLineMonitor.Domain;

namespace PackingLineMonitor.Infrastructure.Events;

public record LineStatusChangedEvent(LineStatus Status, DateTime? Timestamp);
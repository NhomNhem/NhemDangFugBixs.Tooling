using NhemDangFugBixs.Attributes;

namespace NhemDangFugBixs.Common.Models;

/// <summary>
/// Information about a MessagePipe broker registration.
/// </summary>
internal readonly struct MessagePipeBrokerInfo {
    /// <summary>
    /// Full name of the broker type (e.g., "Demo.PlayerJoined").
    /// </summary>
    public string BrokerType { get; }

    /// <summary>
    /// Message type for the broker (e.g., "Demo.PlayerJoined" for IPublisher{PlayerJoined}).
    /// </summary>
    public string MessageType { get; }

    /// <summary>
    /// Scope name where this broker is registered.
    /// </summary>
    public string ScopeName { get; }

    /// <summary>
    /// MessagePipe type (Publisher, Subscriber, or Handler).
    /// </summary>
    public MessagePipeType BrokerKind { get; }

    public MessagePipeBrokerInfo(
        string brokerType,
        string messageType,
        string scopeName,
        MessagePipeType brokerKind)
        => (BrokerType, MessageType, ScopeName, BrokerKind) = (brokerType, messageType, scopeName, brokerKind);
}

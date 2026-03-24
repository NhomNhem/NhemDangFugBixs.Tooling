using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// MessagePipe type enum for broker registration.
    /// Used to identify whether a service is a publisher, subscriber, or handler.
    /// </summary>
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    enum MessagePipeType {
        /// <summary>
        /// MessagePipe publisher (IPublisher{T}).
        /// </summary>
        Publisher,

        /// <summary>
        /// MessagePipe subscriber (ISubscriber{T}).
        /// </summary>
        Subscriber,

        /// <summary>
        /// MessagePipe handler (IRequestHandler{TRequest, TResponse} or IMessageHandler{T}).
        /// </summary>
        Handler
    }
}

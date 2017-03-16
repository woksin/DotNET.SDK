﻿/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Bifrost.Domain;
using Bifrost.Events;
using Bifrost.Execution;
using Bifrost.Lifecycle;

namespace Bifrost.Commands
{
    /// <summary>
    /// Defines a context for a <see cref="ICommand">command</see> passing through
    /// the system
    /// </summary>
    public interface ICommandContext : ITransaction
    {
        /// <summary>
        /// Gets the <see cref="TransactionCorrelationId"/> for the <see cref="ICommandContext"/>
        /// </summary>
        TransactionCorrelationId TransactionCorrelationId { get; }

        /// <summary>
        /// Gets the <see cref="ICommand">command</see> the context is for
        /// </summary>
        ICommand Command { get; }

        /// <summary>
        /// Gets the <see cref="IExecutionContext"/> for the command
        /// </summary>
        IExecutionContext ExecutionContext { get; }

        /// <summary>
        /// Register an aggregated root for tracking
        /// </summary>
        /// <param name="aggregatedRoot">Aggregated root to track</param>
        void RegisterForTracking(IAggregateRoot aggregatedRoot);

        /// <summary>
        /// Get objects that are being tracked
        /// </summary>
        /// <returns>All tracked objects</returns>
        IEnumerable<IAggregateRoot> GetObjectsBeingTracked();

        /// <summary>
        /// Get commmitted events for a specific <see cref="IEventSource"/>
        /// </summary>
        /// <param name="eventSource"><see cref="IEventSource"/> to get from</param>
        /// <returns><see cref="CommittedEventStream"/> for the <see cref="EventSource"/></returns>
        CommittedEventStream GetCommittedEventsFor(IEventSource eventSource);

        /// <summary>
        /// Returns the last committed <see cref="EventSourceVersion">Event Source Version</see> for the <see cref="IEventSource"/>
        /// </summary>
        /// <param name="eventSource"><see cref="IEventSource"/> to get <see cref="EventSourceVersion">version</see> for</param>
        /// <returns>The last committed <see cref="EventSourceVersion">version</see></returns>
        EventSourceVersion GetLastCommittedVersionFor(IEventSource eventSource);
    }
}
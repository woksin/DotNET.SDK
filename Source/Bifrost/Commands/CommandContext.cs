﻿/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using Bifrost.Domain;
using Bifrost.Events;
using Bifrost.Execution;
using Bifrost.Lifecycle;

namespace Bifrost.Commands
{
    /// <summary>
    /// Represents a <see cref="ICommandContext">ICommandContext</see>
    /// </summary>
    public class CommandContext : ICommandContext
    {
        TransactionCorrelationId _transactionCorrelationId;
        IEventStore _eventStore;
        IUncommittedEventStreamCoordinator _uncommittedEventStreamCoordinator;
        List<IAggregateRoot> _objectsTracked = new List<IAggregateRoot>();

        /// <summary>
        /// Initializes a new <see cref="CommandContext">CommandContext</see>
        /// </summary>
        /// <param name="command">The <see cref="ICommand">command</see> the context is for</param>
        /// <param name="executionContext">The <see cref="IExecutionContext"/> for the command</param>
        /// <param name="eventStore">A <see cref="IEventStore"/> that will receive any events generated</param>
        /// <param name="uncommittedEventStreamCoordinator">The <see cref="IUncommittedEventStreamCoordinator"/> to use for coordinating the committing of events</param>
        public CommandContext(
            ICommand command,
            IExecutionContext executionContext,
            IEventStore eventStore,
            IUncommittedEventStreamCoordinator uncommittedEventStreamCoordinator)
        {
            Command = command;
            ExecutionContext = executionContext;
            _eventStore = eventStore;
            _uncommittedEventStreamCoordinator = uncommittedEventStreamCoordinator;

            // This should be exposed to the client somehow - maybe even coming from the client
            _transactionCorrelationId = Guid.NewGuid();
        }


#pragma warning disable 1591 // Xml Comments
        public ICommand Command { get; private set; }
        public IExecutionContext ExecutionContext { get; private set; }

        public void RegisterForTracking(IAggregateRoot aggregatedRoot)
        {
            _objectsTracked.Add(aggregatedRoot);
        }

        public IEnumerable<IAggregateRoot> GetObjectsBeingTracked()
        {
            return _objectsTracked;
        }


        /// <summary>
        /// Disposes the CommandContext by Committing
        /// </summary>
        public void Dispose()
        {
            Commit();
        }

        public void Commit()
        {
            var trackedObjects = GetObjectsBeingTracked();
            foreach (var trackedObject in trackedObjects)
            {
                var events = trackedObject.UncommittedEvents;
                if (events.HasEvents)
                {
                    _uncommittedEventStreamCoordinator.Commit(_transactionCorrelationId, events);
                    trackedObject.Commit();
                }
            }
        }

        public void Rollback()
        {
            // Todo : Should rollback any aggregated roots that are being tracked - 
            // PS: What do you do with events that has already been dispatched and stored?
        }


        public CommittedEventStream GetCommittedEventsFor(IEventSource eventSource)
        {
            return _eventStore.GetFor(eventSource);
        }

        public EventSourceVersion GetLastCommittedVersionFor(IEventSource eventSource)
        {
            return _eventStore.GetLastCommittedVersionFor(eventSource);
        }

#pragma warning restore 1591 // Xml Comments


    }
}

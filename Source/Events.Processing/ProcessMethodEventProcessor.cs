﻿/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Reflection;
using Dolittle.DependencyInversion;
using Dolittle.Time;
using Dolittle.Applications;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Logging;

namespace Dolittle.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor"/> for systems marked with the
    /// <see cref="ICanProcessEvents"/> marker interface and has the "Process" method according to the
    /// convention
    /// </summary>
    public class ProcessMethodEventProcessor : IEventProcessor
    {
        readonly IContainer _container;
        readonly MethodInfo _methodInfo;
        readonly ISystemClock _systemClock;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ProcessMethodEventProcessor"/>
        /// </summary>
        /// <param name="container"><see cref="IContainer"/> to use for getting instances of <see cref="ICanProcessEvents"/> implementation</param>
        /// <param name="systemClock"><see cref="ISystemClock"/> for timing purposes</param>
        /// <param name="identifier"><see cref="EventProcessorIdentifier"/> that uniquely identifies the <see cref="ProcessMethodEventProcessor"/></param>
        /// <param name="event"><see cref="IApplicationArtifactIdentifier">Identifier</see> for identifying the <see cref="IEvent"/></param>
        /// <param name="methodInfo"><see cref="MethodInfo"/> for the actual process method</param>
        /// <param name="logger"></param>
        public ProcessMethodEventProcessor(
            IContainer container,
            ISystemClock systemClock,
            EventProcessorIdentifier identifier,
            IApplicationArtifactIdentifier @event,
            MethodInfo methodInfo,
            ILogger logger)
        {
            Identifier = identifier;
            Event = @event;
            _container = container;
            _systemClock = systemClock;
            _methodInfo = methodInfo;
            _logger = logger;
            _logger.Trace($"ProcessMethodEventProcessor for '{@event}' exists on type '{methodInfo}'");
        }

        /// <inheritdoc/>
        public IApplicationArtifactIdentifier Event { get; }

        /// <inheritdoc/>
        public EventProcessorIdentifier Identifier { get; }

        /// <inheritdoc/>
        public IEventProcessingResult Process(IEventEnvelope envelope, IEvent @event)
        {
            _logger.Trace($"Process through a process method");
            var status = EventProcessingStatus.Success;
            var messages = new EventProcessingMessage[0];

            _logger.Trace("Get current time");
            var start = _systemClock.GetCurrentTime();

            _logger.Trace($"Start : {start}");

            try
            {
                _logger.Trace($"Process event using {_methodInfo}");
                var processor = _container.Get(_methodInfo.DeclaringType);
                _logger.Trace("Invoke method");
                _methodInfo.Invoke(processor, new[] { @event });
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Failed processing");
                status = EventProcessingStatus.Failed;
                messages = new[] {
                    new EventProcessingMessage(EventProcessingMessageSeverity.Error, exception.Message, exception.StackTrace.Split(Environment.NewLine.ToCharArray()))
                };
            }
            var end = _systemClock.GetCurrentTime();

            return new EventProcessingResult(envelope.CorrelationId, this, status, start, end, messages);
        }
    }
}

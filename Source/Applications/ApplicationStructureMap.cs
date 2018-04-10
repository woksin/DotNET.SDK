/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Dolittle.Collections;
using Dolittle.Strings;

namespace Dolittle.Applications
{
    /// <summary>
    /// Represents an implementation of <see cref="IApplicationStructureMap"/>
    /// </summary>
    public class ApplicationStructureMap : IApplicationStructureMap
    {
        readonly IApplication _application;
        readonly IDictionary<ApplicationArea, IEnumerable<IStringFormat>> _formatsPerArea;
        readonly IEnumerable<IStringFormat> _formats;

        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationStructureMap"/>
        /// </summary>
        /// <param name="application"></param>
        /// <param name="formatsPerArea"></param>
        public ApplicationStructureMap(IApplication application, IDictionary<ApplicationArea, IEnumerable<IStringFormat>> formatsPerArea)
        {
            _application = application;
            _formatsPerArea = formatsPerArea;
            _formats = formatsPerArea.Values.SelectMany(_ => _).ToArray();
        }

        /// <inheritdoc/>
        public IEnumerable<IStringFormat> Formats => _formats;

        /// <inheritdoc/>
        public bool DoesAnyFitInStructure(IEnumerable<Type> types)
        {
            return types.Any(DoesFitInStructure);
        }

        /// <inheritdoc/>
        public bool DoesFitInStructure(Type type)
        {
            return _formats.Any(format => format.Match(GetTypeStringFor(type)).HasMatches);
        }

        /// <inheritdoc/>
        public Type GetBestMatchingTypeFor(IEnumerable<Type> types)
        {
            var matchedType = types.Where(type => _formats.Any(format => format.Match(GetTypeStringFor(type)).HasMatches == true)).SingleOrDefault();
            ThrowIfNoMatchingStructure(types, matchedType);
            return matchedType;
        }

        string GetTypeStringFor(Type type)
        {
            return $"{type.Namespace}.{type.Name}";
        }

        void ThrowIfNoMatchingStructure(IEnumerable<Type> types, Type matchedType)
        {
            if (matchedType == null) throw new NoMatchingStructure(types);
        }

        void ThrowIfAmbiguousTypes(IApplicationArtifactIdentifier identifier, IEnumerable<Type> typesMatchingName)
        {
            if (typesMatchingName.Count() > 1) 
            {
                throw new AmbiguousTypes(identifier);
            }
        }
    }
}
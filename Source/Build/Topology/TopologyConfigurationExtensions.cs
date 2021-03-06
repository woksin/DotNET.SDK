/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Applications;
using Dolittle.Applications.Configuration;
using Dolittle.Collections;

namespace Dolittle.Build.Topology
{
    /// <summary>
    /// Extensions for the topology of a <see cref="BoundedContextConfiguration"/>
    /// </summary>
    public static class TopologyConfigurationExtensions
    {
        /// <summary>
        /// Returns a collapsed list of <see cref="ModuleDefinition"/> 
        /// </summary>
        public static IDictionary<Module, ModuleDefinition> GetCollapsedModules(this IDictionary<Module,ModuleDefinition> modules)
        {
            var collapsedModules = new Dictionary<Module,ModuleDefinition>();

            foreach (var group in modules.GroupBy(_ => _.Value.Name))
            {
                var module = group.ElementAt(0);
                var features = new Dictionary<Feature, FeatureDefinition>(module.Value.Features);
                features = features.Concat(group.Skip(1).SelectMany(_ => _.Value.Features)).ToDictionary(_ => _.Key, _ => _.Value);

                var newModule = new ModuleDefinition(module.Value.Name, features.GetCollapsedFeatures());
                collapsedModules[module.Key] = newModule;
            }

            return collapsedModules;
        }
        /// <summary>
        /// Returns a collapsed list of <see cref="FeatureDefinition"/> 
        /// </summary>
        public static IDictionary<Feature, FeatureDefinition> GetCollapsedFeatures(this IDictionary<Feature, FeatureDefinition> features)
        {
            var collapsedFeatures = new Dictionary<Feature,FeatureDefinition>();

            foreach (var group in features.GroupBy(_ => _.Value.Name))
            {
                var feature = group.ElementAt(0);
                var subFeatures = new Dictionary<Feature, FeatureDefinition>(feature.Value.SubFeatures);
                subFeatures = subFeatures.Concat(group.Skip(1).SelectMany(_ => _.Value.SubFeatures)).ToDictionary(_ => _.Key, _ => _.Value);
                var newFeature = new FeatureDefinition(feature.Value.Name, subFeatures);
                collapsedFeatures[feature.Key] = newFeature;
            }
            return collapsedFeatures;
        }
        /// <summary>
        /// Validates the <see cref="Applications.Configuration.Topology"/>
        /// </summary>
        public static void ValidateTopology(this Applications.Configuration.Topology topology, bool useModules, IBuildMessages buildMessages)
        {
            ThrowIfDuplicateId(topology, useModules, buildMessages);
        }

        static void ThrowIfDuplicateId(Applications.Configuration.Topology topology, bool useModules, IBuildMessages buildMessages)
        {
            var idMap = new Dictionary<Guid, string>();
            bool hasDuplicateId = false;

            if (useModules)
            {
                foreach (var module in topology.Modules)
                {
                    if (idMap.ContainsKey(module.Key))
                    {
                        hasDuplicateId = true;
                        var name = idMap[module.Key];

                        buildMessages.Error(
                            $"Duplicate id found in bounded-context topology.\n" +
                            $"The id: '{module.Key.Value}' is already occupied by the Module/Feature: '{name}' ");
                    }
                    else
                    {
                        idMap.Add(module.Key, module.Value.Name);
                    }
                    ThrowIfDuplicateId(module.Value.Features, ref idMap, ref hasDuplicateId, buildMessages);
                }
            }
            else 
            {
                ThrowIfDuplicateId(topology.Features, ref idMap, ref hasDuplicateId, buildMessages);
            }

            if (hasDuplicateId)
            {
                throw new InvalidTopology("Bounded context topology has one or more Features/Modules with the same Id");
            }
        }
        static void ThrowIfDuplicateId(IDictionary<Feature,FeatureDefinition> features, ref Dictionary<Guid, string> idMap, ref bool hasDuplicateId, IBuildMessages buildMessages)
        {
            foreach (var feature in features)
            {
                if (idMap.ContainsKey(feature.Key))
                {
                    hasDuplicateId = true;
                    var name = idMap[feature.Key];
                    
                    buildMessages.Error(
                        $"Duplicate id found in bounded-context topology.\n" +
                        $"The id: '{feature.Key.Value}' is already occupied by the Module/Feature: '{name}' ");
                }
                else
                {
                    idMap.Add(feature.Key, feature.Value.Name);
                }
                ThrowIfDuplicateId(feature.Value.SubFeatures, ref idMap, ref hasDuplicateId, buildMessages);
            }
        }
    }
}
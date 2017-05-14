﻿using Bifrost.Security;
using Bifrost.SomeRandomNamespace;
using Bifrost.Specs.Security.Fakes;

namespace Bifrost.Specs.Security.for_NamespaceSecurable.given
{
    public class a_namespace_securable
    {
        protected static NamespaceSecurable namespace_securable;
        protected static object action_with_namespace_match;
        protected static object action_within_another_namespace;

        public a_namespace_securable()
        {
            action_with_namespace_match = new SimpleCommand();
            action_within_another_namespace = new CommandInADifferentNamespace();

            namespace_securable = new NamespaceSecurable(typeof(SimpleCommand).Namespace);
        }
    }
}
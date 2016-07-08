namespace MailTrace.Host.Ninject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Ninject.Components;
    using global::Ninject.Infrastructure;
    using global::Ninject.Planning.Bindings;
    using global::Ninject.Planning.Bindings.Resolvers;

    public class ContravariantBindingResolver : NinjectComponent, IBindingResolver
    {
        public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, Type service)
        {
            if (service.IsGenericType)
            {
                var genericType = service.GetGenericTypeDefinition();
                var genericArguments = genericType.GetGenericArguments();
                if (genericArguments.Count() == 1
                    && genericArguments.Single().GenericParameterAttributes.HasFlag(GenericParameterAttributes.Contravariant))
                {
                    var argument = service.GetGenericArguments().Single();
                    var matches = bindings.Where(kvp => kvp.Key.IsGenericType
                                                        && kvp.Key.GetGenericTypeDefinition() == genericType
                                                        && kvp.Key.GetGenericArguments().Single() != argument
                                                        && kvp.Key.GetGenericArguments().Single().IsAssignableFrom(argument))
                                          .SelectMany(kvp => kvp.Value);
                    return matches;
                }
            }

            return Enumerable.Empty<IBinding>();
        }
    }
}
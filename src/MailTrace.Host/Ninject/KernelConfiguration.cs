namespace MailTrace.Host.Ninject
{
    using System;
    using System.IO;
    using System.Net.NetworkInformation;

    using global::Ninject;
    using global::Ninject.Extensions.Conventions;
    using global::Ninject.Planning.Bindings.Resolvers;

    using MediatR;

    public static class KernelConfiguration
    {
        public static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            BindMediatR(kernel);

            return kernel;
        }

        // ReSharper disable once IdentifierTypo
        private static void BindMediatR(IKernel kernel)
        {
            kernel.Components.Add<IBindingResolver, ContravariantBindingResolver>();
            kernel.Bind(scan => scan.FromAssemblyContaining<IMediator>().SelectAllClasses().BindDefaultInterface());
            kernel.Bind(scan => scan.FromAssemblyContaining<Ping>().SelectAllClasses().BindAllInterfaces());
            kernel.Bind<TextWriter>().ToConstant(Console.Out);
            kernel.Bind<SingleInstanceFactory>().ToMethod(ctx => t => ctx.Kernel.Get(t));
            kernel.Bind<MultiInstanceFactory>().ToMethod(ctx => t => ctx.Kernel.GetAll(t));
        }
    }
}
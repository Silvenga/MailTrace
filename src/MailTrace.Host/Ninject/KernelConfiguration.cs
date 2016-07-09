namespace MailTrace.Host.Ninject
{
    using AutoMapper;

    using global::Ninject;
    using global::Ninject.Extensions.Conventions;
    using global::Ninject.Planning.Bindings.Resolvers;

    using MailTrace.Host.Queries;

    using MediatR;

    public static class KernelConfiguration
    {
        public static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            //kernel.Bind<IKernel>().ToConstant(kernel);

            BindAutoMapper(kernel);
            BindMediatR(kernel);

            return kernel;
        }

        // ReSharper disable once IdentifierTypo
        private static void BindMediatR(IKernel kernel)
        {
            kernel.Components.Add<IBindingResolver, ContravariantBindingResolver>();
            kernel.Bind(scan => scan.FromAssemblyContaining<IMediator>().SelectAllClasses().BindDefaultInterface());
            kernel.Bind(scan => scan.FromAssemblyContaining<ListLogsHandler>().SelectAllClasses().BindDefaultInterface());
            kernel.Bind<SingleInstanceFactory>().ToMethod(ctx => t => ctx.Kernel.Get(t));
            kernel.Bind<MultiInstanceFactory>().ToMethod(ctx => t => ctx.Kernel.GetAll(t));
        }

        private static void BindAutoMapper(IKernel kernel)
        {
            IConfigurationProvider config = new MapperConfiguration(cfg => { cfg.CreateMissingTypeMaps = true; });

            kernel.Bind<IMapper>().ToMethod(x => config.CreateMapper());
            kernel.Bind<IConfigurationProvider>().ToConstant(config);
        }
    }
}
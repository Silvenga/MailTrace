namespace MailTrace.Host.Ninject
{
    using AutoMapper;

    using global::Ninject;
    using global::Ninject.Extensions.Conventions;
    using global::Ninject.Planning.Bindings.Resolvers;

    using MailTrace.Host.Queries;
    using MailTrace.Host.Queries.Logs;

    using MediatR;

    public static class KernelConfiguration
    {
        public static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            BindAutoMapper(kernel);
            BindMediatR(kernel);

            return kernel;
        }

        // ReSharper disable once IdentifierTypo
        private static void BindMediatR(IKernel kernel)
        {
            kernel.Components.Add<IBindingResolver, ContravariantBindingResolver>();
            kernel.Bind(scan => scan.FromAssemblyContaining<IMediator>().SelectAllClasses().BindDefaultInterface());
            kernel.Bind<SingleInstanceFactory>().ToMethod(ctx => t => ctx.Kernel.Get(t));
            kernel.Bind<MultiInstanceFactory>().ToMethod(ctx => t => ctx.Kernel.GetAll(t));

            kernel.Bind(x => x.FromAssemblyContaining<ListLogsHandler>()
                              .SelectAllClasses()
                              .InheritedFrom(typeof(IRequestHandler<,>))
                              .BindSingleInterface());

            kernel.Bind(x => x.FromAssemblyContaining<ListLogsHandler>()
                              .SelectAllClasses()
                              .InheritedFrom(typeof(IAsyncRequestHandler<,>))
                              .BindSingleInterface());
        }

        private static void BindAutoMapper(IKernel kernel)
        {
            IConfigurationProvider config = new MapperConfiguration(cfg => { cfg.CreateMissingTypeMaps = true; });

            kernel.Bind<IConfigurationProvider>().ToConstant(config);
            kernel.Bind<IMapper>().ToMethod(x => x.Kernel.Get<IConfigurationProvider>().CreateMapper());
        }
    }
}
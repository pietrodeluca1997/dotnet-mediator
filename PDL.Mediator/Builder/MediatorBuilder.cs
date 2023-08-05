using Microsoft.Extensions.DependencyInjection;
using PDL.Mediator.Contracts;
using System.Reflection;

namespace PDL.Mediator.Builder
{
    public class MediatorBuilder
    {
        #region Atributos

        // Lista de assemblies do projeto solicitador
        private readonly List<Assembly> _projectAssemblies;

        // Configurações
        private readonly MediatorBuilderOptions _mediatorBuilderOptions;

        // Container de dependência
        private readonly IServiceCollection _serviceCollection;

        #endregion

        #region Inicialização

        private MediatorBuilder(MediatorBuilderOptions options, IServiceCollection serviceCollection)
        {
            _mediatorBuilderOptions = options;
            _projectAssemblies = new List<Assembly>();
            _serviceCollection = serviceCollection;
        }

        public static MediatorBuilder CreateBuilder(MediatorBuilderOptions options, IServiceCollection serviceCollection)
        {
            if (options.ClassTypeFromProjectRoot is null) throw new ArgumentNullException(nameof(options.ClassTypeFromProjectRoot), "Project root class type not informed.");
            return new MediatorBuilder(options, serviceCollection);
        }

        #endregion

        // Busca os assemblies do projeto solicitador
        public MediatorBuilder FetchAssemblies()
        {
            AddRootAssembly();

            AddRootReferencedAssemblies();

            return this;
        }

        // Busca implementações válidas nos assemblies carregados
        public MediatorBuilder ScanForImplementations()
        {
            foreach (Assembly assembly in _projectAssemblies)
            {
                foreach (Type concreteType in assembly.GetTypes())
                {
                    IEnumerable<Type> interfaceTypes = concreteType.GetInterfaces().Where(IsTypeHandlerValid);

                    foreach (Type interfaceType in interfaceTypes)
                    {
                        AddHandlerIntoDIContainer(interfaceType, concreteType);
                    }
                }
            }

            return this;
        }

        // Adiciona o assembly raiz
        private void AddRootAssembly()
        {
            _projectAssemblies.Add(_mediatorBuilderOptions.ClassTypeFromProjectRoot.Assembly);
        }

        // Adiciona todos os assemblies que o assembly raiz depende
        private void AddRootReferencedAssemblies()
        {
            AssemblyName[] assemblyNames = _mediatorBuilderOptions.ClassTypeFromProjectRoot.Assembly.GetReferencedAssemblies();

            foreach (AssemblyName assemblyName in assemblyNames)
            {
                Assembly assembly = Assembly.Load(assemblyName);
                _projectAssemblies.Add(assembly);
            }
        }
        public void Build()
        {
            AddCoreDependencies();
        }

        // Adiciona o mediator
        private void AddCoreDependencies()
        {
            _serviceCollection.AddTransient<IMediator, Core.Mediator>();
        }

        // Adiciona os manipuladores no container de injeção de dependência
        private void AddHandlerIntoDIContainer(Type interfaceType, Type concreteType)
        {
            _serviceCollection.AddTransient(interfaceType, concreteType);
        }

        // O tipo implementa uma das interfaces válidas do mediator?
        private static bool IsTypeHandlerValid(Type type)
        {
            return type.IsGenericType &&
                (type.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                 type.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                 type.GetGenericTypeDefinition() == typeof(IEventSubscriber<>));
        }
    }
}
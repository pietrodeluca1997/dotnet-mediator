using Microsoft.Extensions.DependencyInjection;
using PDL.Mediator.Builder;

namespace PDL.Mediator.Configuration
{
    public static class MediatorConfiguration
    {
        /// <summary>
        ///     Adiciona o mediator no container de dependências
        /// </summary>
        /// <param name="serviceCollection"> Coleção de serviços do projeto consumidor </param>
        /// <param name="mediatorBuilderOptions"> Estrutura para configurar o mediator </param>
        public static void AddMediator(this IServiceCollection serviceCollection, MediatorBuilderOptions mediatorBuilderOptions)
        {
            MediatorBuilder.CreateBuilder(mediatorBuilderOptions, serviceCollection)
                .FetchAssemblies()
                .ScanForImplementations()
                .Build();
        }
    }
}
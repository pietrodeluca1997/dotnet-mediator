using Microsoft.Extensions.DependencyInjection;
using PDL.Mediator.Contracts;

namespace PDL.Mediator.Core
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        ///     Enviar comando ao seu responsável
        /// </summary>
        /// <param name="command">Comando</param>
        /// <typeparam name="TCommand">Classe do comando</typeparam>
        /// <exception cref="ApplicationException">Caso não tenha sido implementado ICommandHandler para este TCommand</exception>
        public async Task SendCommand<TCommand>(TCommand command) where TCommand : class
        {
            if (_serviceProvider.GetService(typeof(ICommandHandler<TCommand>)) is ICommandHandler<TCommand> commandHandler)
            {
                await commandHandler.HandleCommand(command);
            }
            else
            {
                throw new ApplicationException($"Message: {command.GetType().Name} - doesn't have any responsible handler");
            }
        }

        /// <summary>
        ///     Enviar comando ao seu responsável com resposta
        /// </summary>
        /// <param name="command">Comando</param>
        /// <typeparam name="TCommand">Classe do comando</typeparam>
        /// <typeparam name="TCommandResponse">Classe da resposta do comando</typeparam>
        /// <returns>Resposta do comando</returns>
        /// <exception cref="ApplicationException">Caso não tenha sido implementando ICommandHandler para este TCommand com a resposta TCommandResponse</exception>
        public async Task<TCommandResponse> SendCommand<TCommand, TCommandResponse>(TCommand command) where TCommand : class
                                                                                                      where TCommandResponse : class
        {
            if (_serviceProvider.GetService(typeof(ICommandHandler<TCommand, TCommandResponse>)) is ICommandHandler<TCommand, TCommandResponse> commandHandler)
            {
                return await commandHandler.HandleCommand(command);
            }
            else
            {
                throw new ApplicationException($"Message: {command.GetType().Name} - doesn't have any responsible handler");
            }
        }


        /// <summary>
        ///     Notificação sequencial - Não utilizar quando houver dependência externa entre os inscritos
        /// </summary>
        /// <param name="event">Evento</param>
        /// <typeparam name="TEvent">Classe do evento</typeparam>
        public async Task Notify<TEvent>(TEvent @event) where TEvent : class
        {
            if (_serviceProvider.GetServices(typeof(IEventSubscriber<TEvent>)) is IEnumerable<IEventSubscriber<TEvent>> subscribers)
            {
                foreach (IEventSubscriber<TEvent> subscriber in subscribers) await subscriber.HandleNotification(@event);
            }
        }

        /// <summary>
        ///     Transmissão possivelmente paralela - Não utilizar quando houver dependência externa entre os inscritos
        /// </summary>
        /// <param name="event">Evento</param>
        /// <typeparam name="TEvent">Classe do evento</typeparam>
        public async Task Broadcast<TEvent>(TEvent @event) where TEvent : class
        {
            if (_serviceProvider.GetServices(typeof(IEventSubscriber<TEvent>)) is IEnumerable<IEventSubscriber<TEvent>> subscribers)
            {
                IEnumerable<Task> tasks = subscribers.Select(subscriber => subscriber.HandleNotification(@event));

                await Task.WhenAll(tasks);
            }
        }
    }
}
namespace PDL.Mediator.Contracts
{
    public interface IMediator
    {
        Task SendCommand<TCommand>(TCommand command) where TCommand : class;

        Task<TCommandResponse> SendCommand<TCommand, TCommandResponse>(TCommand command) where TCommand : class
                                                                                         where TCommandResponse : class;
        Task Notify<TEvent>(TEvent @event) where TEvent : class;
        Task Broadcast<TEvent>(TEvent @event) where TEvent : class;
    }
}
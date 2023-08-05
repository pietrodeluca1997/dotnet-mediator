namespace PDL.Mediator.Contracts
{
    public interface ICommandHandler<in TCommand> where TCommand : class
    {
        Task HandleCommand(TCommand command);
    }

    public interface ICommandHandler<in TCommand, TCommandResponse> where TCommand : class
                                                                    where TCommandResponse : class
    {
        Task<TCommandResponse> HandleCommand(TCommand command);
    }
}
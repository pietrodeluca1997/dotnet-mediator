namespace PDL.Mediator.Contracts
{
    public interface IEventSubscriber<in TEvent> where TEvent : class
    {
        Task HandleNotification(TEvent @event);
    }
}
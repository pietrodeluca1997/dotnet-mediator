namespace PDL.Mediator.Builder
{
    public record struct MediatorBuilderOptions(Type ClassTypeFromProjectRoot)
    {
        public readonly Type ClassTypeFromProjectRoot = ClassTypeFromProjectRoot;
    }
}
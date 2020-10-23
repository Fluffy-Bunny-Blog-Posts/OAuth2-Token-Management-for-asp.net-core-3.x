namespace TestServices
{
    public class SingletonService : ISingletonService
    {
        public string Name => nameof(SingletonService);
    }
}

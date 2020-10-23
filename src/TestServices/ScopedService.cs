namespace TestServices
{
    public class ScopedService : IScopedService
    {
        public string Name => nameof(TransientService);
    }
}

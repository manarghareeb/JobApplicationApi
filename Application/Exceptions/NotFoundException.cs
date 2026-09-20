namespace Application.Exceptions
{
    internal class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}

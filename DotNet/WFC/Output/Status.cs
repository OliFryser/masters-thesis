namespace WFC.Output
{
    public class Status
    {
        public Status(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
    }
}
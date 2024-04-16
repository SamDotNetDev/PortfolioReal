namespace Portfolio.Statuses
{
    public class Status
    {
        public string Message { get; set; }
        public object Data { get; set; }
    }
    enum StatusMessage
    {
        Success,
        NotFound,
        Exist,
        Error,
        IndexOutOfBounds
    }
}

namespace Services.Model.NeginHub
{
    public class NeginHubBaseResult
    {
        public long NeginHubLogId { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}

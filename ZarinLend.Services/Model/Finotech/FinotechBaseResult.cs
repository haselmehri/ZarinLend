using System;

namespace Services.Model
{
    [Serializable]
    public class FinotechBaseResult<T> where T : class
    {
        public T? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string TrackId { get; set; }
        public string ResponseCode { get; set; }
        public string Status { get; set; }
        public FinotechErrorResult? Error { get; set; }
    }
}

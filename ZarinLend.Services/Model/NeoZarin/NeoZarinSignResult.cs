namespace Services.Model
{
    public class SignatureParam
    {
        public string trackId { get; set; }
        public string mobile { get; set; }
        public string fileUrl { get; set; }
        public string description { get; set; }
        public string callback { get; set; }
    }
    public class NeoZarinSignResult
    {
        public bool HasError { get; set; }
        public string Message { get; set; }
        public NeoZarinSignDataResult Data { get; set; }
    }

    public class NeoZarinSignDataResult
    {
        public string TrackId { get; set; }
    }
}

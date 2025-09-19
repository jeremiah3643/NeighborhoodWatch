namespace NeighborhoodWatch.Models
{
    public class JobOutboundResult
    {
        public int? RecordCount { get; set; }
        public string? OutboundFilename { get; set; }
        public string? ExternalSystemId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? TrackingKey { get; set; }
        public string? FileStatus { get; set; }
    }
}
namespace NeighborhoodWatch.Models
{
    public class EmailTemplates
    {
        public EmailTemplate FileCreated { get; set; } = new();
        public EmailTemplate FileProcessed { get; set; } = new();
        public EmailTemplate Testing { get; set; } = new();
        public EmailTemplate FileCreatedZip { get; set; } = new();
    }

    public class EmailTemplate
    {
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
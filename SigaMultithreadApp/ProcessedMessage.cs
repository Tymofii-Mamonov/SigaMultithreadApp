using System;

namespace SigaMultithreadApp
{
    public class ProcessedMessage
    {
        public Message Message { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public DateTime EndProcessingTime { get; set; }
    }
}

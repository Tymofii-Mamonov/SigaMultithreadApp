using System;

namespace SigaMultithreadApp
{
    public class Message
    {
        public Message()
        {
            Id = Guid.NewGuid();
            SentTime = DateTime.UtcNow;
            MessageText = $"Message Test {SentTime}";
        }

        public Guid Id { get; }
        public DateTime SentTime { get; set; }
        public string MessageText { get; set; }
    }
}

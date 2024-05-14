namespace ParkingReservationApp.Services.ChatGPT.Helpers
{
    public class GptModel
    {
        public Message[] Messages { get; set; }

        public float Temperature { get; set; }

        public int MaxTokens { get; set; }

        public float TopP { get; set; }

        public float FrequencyPenalty { get; set; }

        public float PresencePenalty { get; set; }

        public string Model { get; set; }

        public bool Stream { get; set; }
        public string Assistent { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class Response
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public int Created { get; set; }
        public string Model { get; set; }
        public ResponseUsage ResponseUsage { get; set; }
        public Choice[] Choices { get; set; }
    }

    public class ResponseUsage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }

    public class Choice
    {
        public ResponseMessage ResponseMessage { get; set; }
        public string FinishReason { get; set; }
        public int Index { get; set; }
    }

    public class ResponseMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
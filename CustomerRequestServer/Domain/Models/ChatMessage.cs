namespace CustomerRequestServer.Domain.Models;

public class ChatMessage
{
    public string Message { get; set; }
    public string Author { get; set; }

    public ChatMessage(string author,string message)
    {
        Message = message;
        Author = author;
    }

}
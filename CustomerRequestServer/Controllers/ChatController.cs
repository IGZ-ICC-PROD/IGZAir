using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using CustomerRequestServer.Domain.Models;
using CustomerRequestServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace CustomerRequestServer.Controllers; 

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly Kernel _kernel;
    private readonly IHubContext<DevConsoleHub> _hubContext;
    
    private static readonly ConcurrentDictionary<string, ChatHistory> ChatHistories = new ConcurrentDictionary<string, ChatHistory>();


    private const string SystemMessage =
        @"You are an AI assistant for the airline 'IGZ Air', tasked with modifying reservations based on user input. 
The user will provide a natural language description of the desired booking modification. 
Your responsibilities include generating a valid MongoDB update query in JSON format that performs the requested operation and executing it on the database using the plugins at your disposal.

To accomplish this, you'll need to:

1. Allways Use the GetReservationsAsync method to retrieve the current reservations before modifying them, this can also help you identify the reservation that needs to be modified.
2. Generate a MongoDB update query that’s correctly formatted to be directly parsed into a BsonDocument. This includes using the correct operation and filter criteria (using _id instead of 'ReservationId' in the filter criteria).
   An example of a valid command looks like this: { 'update': 'Reservations', 'updates': [ { 'q': { '_id': 'R001' }, 'u': { '$set': { 'DepartureDate': '2024-03-22T00:00:00Z' } } } ] }
3. Use the ExecuteMongoQueryAsync method to execute the update on the database.
4. Handle exceptions and errors during the query execution process, providing meaningful responses to the user in cases of failures.
IMPORTANT: In the Reservation model, the 'ReservationId' property is mapped to the '_id' field in MongoDB. When generating update queries, ensure you use '_id', not 'ReservationId', in the filter criteria.

Finally, always respond to the user with a confirmation message indicating the success or failure of the operation. Remember, this is a customer-facing application, so prioritize clarity, friendliness, and avoiding technical jargon in your responses"";";

    
    public ChatController(IKernelBuilder semanticKernelBuilder, IHubContext<DevConsoleHub> hubContext)
    {
        _hubContext = hubContext;
        _kernel = semanticKernelBuilder.Build();
    }

    [HttpPost("{conversationId}")]
    public async Task<ActionResult<string>> Chat(string conversationId, [FromBody] ChatRequest request)
    {
        var history = ChatHistories.GetOrAdd(conversationId, _ =>
        {
            var newHistory = new ChatHistory();
            newHistory.AddSystemMessage(SystemMessage);
            return newHistory;
        });

        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        history.AddUserMessage(request.Message);
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        IReadOnlyList<ChatMessageContent> response = await chatCompletionService.GetChatMessageContentsAsync(history, openAIPromptExecutionSettings, _kernel);
        history.AddAssistantMessage(response.Last().Content);

        return Ok(response.Last().Content);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using CustomerRequestServer.Domain.Models;
using CustomerRequestServer.Hubs;
using CustomerRequestServer.Infrastructure.AI;
using CustomerRequestServer.Infrastructure.Repositories;
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
    private readonly IChatHistoryRepository _chatHistoryRepository;
    
    public ChatController(IKernelBuilder semanticKernelBuilder, IHubContext<DevConsoleHub> hubContext, IChatHistoryRepository chatHistoryRepository)
    {
        _hubContext = hubContext;
        _chatHistoryRepository = chatHistoryRepository;
        _kernel = semanticKernelBuilder.Build();
    }
    
    [HttpPost("{conversationId}")]
    public async Task<ActionResult<string>> Chat(Guid conversationId, [FromBody] ChatRequest request)
    {
        ChatHistory history = await _chatHistoryRepository.GetOrCreateChatHistoryAsync(conversationId, request.AgentType);
        IChatCompletionService chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        _chatHistoryRepository.UpdateChatHistory(conversationId, AuthorRole.User, request.Message);
        
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        IReadOnlyList<ChatMessageContent> response = await chatCompletionService.GetChatMessageContentsAsync(history, openAIPromptExecutionSettings, _kernel);
        
        _chatHistoryRepository.UpdateChatHistory(conversationId,AuthorRole.Assistant, response.Last().Content);

        return Ok(response.Last().Content);
    }
    
    [HttpGet("{conversationId}")]
    public async Task<ActionResult<IEnumerable<ChatMessage>>> GetChatHistory(Guid conversationId,[FromQuery]AgentType agentType)
    {
        ChatHistory history = await _chatHistoryRepository.GetOrCreateChatHistoryAsync(conversationId,agentType);
        IEnumerable<ChatMessage> chatMessages = history.Where(entry => entry.Role == AuthorRole.Assistant || entry.Role == AuthorRole.User).Select((content => new ChatMessage(content.Role.Label,content.Content)));

        return Ok(chatMessages);
    }
    
}
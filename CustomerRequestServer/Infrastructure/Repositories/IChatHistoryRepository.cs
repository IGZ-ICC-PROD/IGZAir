using CustomerRequestServer.Infrastructure.AI;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CustomerRequestServer.Infrastructure.Repositories;

public interface IChatHistoryRepository
{
    Task<ChatHistory> GetOrCreateChatHistoryAsync(Guid conversationId, AgentType agentType);
    void UpdateChatHistory(Guid conversationId, AuthorRole role, string message);
    
    Task<ChatHistory> GetChatHistoryAsync(Guid conversationId);
}
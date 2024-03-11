using System.Collections.Concurrent;
using CustomerRequestServer.Infrastructure.AI;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CustomerRequestServer.Infrastructure.Repositories;

public class ChatHistoryRepository : IChatHistoryRepository
{
    private readonly ConcurrentDictionary<Guid,ChatHistory> _chatHistoryDictionary = new();
    private readonly IAgentProvider _agentProvider;

    public ChatHistoryRepository(IAgentProvider agentProvider)
    {
        _agentProvider = agentProvider;
    }
    
    public async Task<ChatHistory> GetOrCreateChatHistoryAsync(Guid conversationId, AgentType agentType)
    {
        if (_chatHistoryDictionary.TryGetValue(conversationId, out var chatHistory))
        {
            return chatHistory;
        }

        var newHistory = new ChatHistory();
        var systemMessage = await _agentProvider.GetSystemMessageAsync(agentType);
        newHistory.AddSystemMessage(systemMessage);
        var greetingMessage = await _agentProvider.GetGreetingMessageAsync(agentType);
        newHistory.AddAssistantMessage(greetingMessage);
        _chatHistoryDictionary.TryAdd(conversationId, newHistory);
        return newHistory;
    }

    public void UpdateChatHistory(Guid conversationId, AuthorRole role, string message)
    {
        if (_chatHistoryDictionary.TryGetValue(conversationId, out var chatHistory))
        {
            chatHistory.AddMessage(role, message);
        }
    }

    public Task<ChatHistory> GetChatHistoryAsync(Guid conversationId)
    {
        if (_chatHistoryDictionary.TryGetValue(conversationId, out var chatHistory))
        {
            return Task.FromResult(chatHistory);
        }
       
        throw new InvalidOperationException("Chat history not found");
    }
}
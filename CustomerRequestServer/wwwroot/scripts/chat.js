let currentAgent = 'customerSupport';
let conversationIds = {
    customerSupport: uuidv4(),
    technicalSupport: uuidv4(),
    current: function() {
        return this[currentAgent];
    }
}

async function loadChatHistory() {
    try {
        const response = await axios.get(`/api/chat/${conversationIds.current()}?agentType=${currentAgent}`);
        const chatHistory = response.data;
        const chatMessages = document.getElementById('chatMessages');
        chatMessages.innerHTML = '';
        chatHistory.forEach(message => {
            const messageElement = document.createElement('div');
            messageElement.classList.add('message', message.author === 'user' ? 'user-message' : 'agent-message');
            if(message.author !== 'user' && currentAgent === 'technicalSupport') {
                messageElement.classList.add('technical');
            }
            const senderName = message.author === 'user' ? 'You' : currentAgent === 'customerSupport' ? 'Skye' : 'JetCode';
            messageElement.innerHTML = createMessage(message.author, message.message);
            //messageElement.innerHTML = `<span class="${message.author === 'user' ? 'user-name' : 'agent-name' + currentAgent === 'technicalSupport' ? ' technical' : ''}">${senderName}</span>: ${message.message}`;
            chatMessages.appendChild(messageElement);
        });
        chatMessages.scrollTop = chatMessages.scrollHeight;
    } catch (error) {
        console.error('Error loading chat history:', error);
    }
}

function createMessage(author, message) {
    if(author === 'user') {
        return `<span class="user-name">You</span>: ${message}`;
    }
    else if(currentAgent === 'customerSupport') {
        return `<span class="agent-name">Skye</span>: ${message}`;
    }
    else {
        return `<span class="agent-name technical">JetCode</span>: ${message}`;
    }
}


async function sendMessage() {
    const chatInput = document.getElementById('chatInput');
    const message = chatInput.value;
    if (message.trim() !== '') {
        const chatMessages = document.getElementById('chatMessages');
        const userMessage = document.createElement('div');
        userMessage.classList.add('message', 'user-message');
        userMessage.innerHTML = `<span class="user-name">You</span>: ${message}`;
        chatMessages.appendChild(userMessage);
        chatInput.value = '';

        // Scroll to the bottom of the chatbox
        chatMessages.scrollTop = chatMessages.scrollHeight;

        // Send the message to the AI agent
        await sendMessageToAgent(message);
    }
}

async function onChatKeyUp(event) {
    if (event.key === 'Enter') {
       await sendMessage();
    }
}

async function sendMessageToAgent(message) {
    try {
        const typingIndicator = document.getElementById('typingIndicator');
        const typingIndicatorText = document.getElementById('typingIndicatorText');
        typingIndicatorText.innerText = `${currentAgent === 'customerSupport' ? 'Skye' : 'JetCode'} is typing...`;
        typingIndicator.style.display = 'flex';

        const response = await axios.post(`/api/chat/${conversationIds.current()}`, { message: message, agentType: currentAgent });
        const aiResponse = response.data;
        const chatMessages = document.getElementById('chatMessages');
        const aiMessage = document.createElement('div');
        aiMessage.classList.add('message','agent-message');
        if(currentAgent === 'technicalSupport') {
            aiMessage.classList.add('technical');
        }
        aiMessage.innerHTML = `<span class="agent-name${currentAgent === 'technicalSupport' ? ' technical' : ''}">${currentAgent === 'customerSupport' ? 'Skye' : 'JetCode'}</span>: ${aiResponse}`;
        chatMessages.appendChild(aiMessage);

        // Scroll to the bottom of the chatbox
        chatMessages.scrollTop = chatMessages.scrollHeight;

        // Refresh the reservations table after receiving the AI response
        await fetchReservations();

        typingIndicator.style.display = 'none';
    } catch (error) {
        console.error('Error sending message to AI agent:', error);
        const typingIndicator = document.getElementById('typingIndicator');
        typingIndicator.style.display = 'none';
    }
}

async function toggleAgent(agent) {
    currentAgent = agent;
    await loadChatHistory();
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'
        .replace(/[xy]/g, function (c) {
            const r = Math.random() * 16 | 0,
                v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
}

async function initializeChat() {
    const chatInput = document.getElementById('chatInput');
    chatInput.addEventListener('keyup', onChatKeyUp);

    const agentToggle = document.getElementById('agentToggle');
    
    agentToggle.addEventListener('change', function() {
        toggleAgent(currentAgent === 'customerSupport' ? 'technicalSupport' : 'customerSupport');
    });
    
    await loadChatHistory();
}

document.addEventListener('DOMContentLoaded', initializeChat);
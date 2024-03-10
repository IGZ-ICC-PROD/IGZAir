let currentAgent = 'customerSupport';
let conversationId = Math.random();


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
        typingIndicator.style.display = 'flex';

        const response = await axios.post(`/api/chat/${conversationId}`, { message });
        const aiResponse = response.data;
        const chatMessages = document.getElementById('chatMessages');
        const aiMessage = document.createElement('div');
        aiMessage.classList.add('message', 'agent-message');
        aiMessage.innerHTML = `<span class="agent-name">Skye</span>: ${aiResponse}`;
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

function toggleAgent(agent) {
    currentAgent = agent;
    const customerSupportBtn = document.getElementById('customerSupportBtn');
    const technicalSupportBtn = document.getElementById('technicalSupportBtn');

    if (currentAgent === 'customerSupport') {
        customerSupportBtn.classList.add('active');
        technicalSupportBtn.classList.remove('active');
    } else if (currentAgent === 'technicalSupport') {
        customerSupportBtn.classList.remove('active');
        technicalSupportBtn.classList.add('active');
    }
}

function initializeChat() {
    const chatInput = document.getElementById('chatInput');
    chatInput.addEventListener('keyup', onChatKeyUp);

    const customerSupportBtn = document.getElementById('customerSupportBtn');
    const technicalSupportBtn = document.getElementById('technicalSupportBtn');

    customerSupportBtn.addEventListener('click', function() {
        toggleAgent('customerSupport');
    });

    technicalSupportBtn.addEventListener('click', function() {
        toggleAgent('technicalSupport');
    });
}

document.addEventListener('DOMContentLoaded', initializeChat);
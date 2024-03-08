let conversationId = Math.random();

function displayReservations(reservations) {
    const tableBody = document.querySelector('#reservationsTable tbody');
    tableBody.innerHTML = '';

    reservations.forEach(reservation => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${reservation.reservationId}</td>
            <td>${reservation.customerName}</td>
            <td>${reservation.flightNumber}</td>
            <td>${reservation.departureDate}</td>
            <td>${reservation.from}</td>
            <td>${reservation.to}</td>
        `;
        tableBody.appendChild(row);
    });
}

async function fetchReservations() {
    try {
        const response = await axios.get('/api/reservation');
        const reservations = response.data;
        displayReservations(reservations);
    } catch (error) {
        console.error('Error fetching reservations:', error);
    }
}

function sendMessage() {
    const chatInput = document.getElementById('chatInput');
    const message = chatInput.value;
    if (message.trim() !== '') {
        const chatMessages = document.getElementById('chatMessages');
        const userMessage = document.createElement('div');
        userMessage.classList.add('message', 'user-message');
        userMessage.textContent = `You: ${message}`;
        chatMessages.appendChild(userMessage);
        chatInput.value = '';

        // Scroll to the bottom of the chatbox
        chatMessages.scrollTop = chatMessages.scrollHeight;

        // Send the message to the AI agent
        sendMessageToAgent(message);
    }
}

async function sendMessageToAgent(message) {
    try {
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
    } catch (error) {
        console.error('Error sending message to AI agent:', error);
    }
}
function toggleConsole() {
    const consoleContent = document.querySelector('.console-content');
    const toggleIcon = document.querySelector('.toggle-icon');

    if (consoleContent.style.display === 'block') {
        consoleContent.style.display = 'none';
        toggleIcon.innerHTML = '&#9660;';
    } else {
        consoleContent.style.display = 'block';
        toggleIcon.innerHTML = '&#9650;';
    }
}

function streamConsoleLog(message) {
    const consoleOutput = document.getElementById('consoleOutput');
    consoleOutput.textContent += `${message}\n`;
    consoleOutput.scrollTop = consoleOutput.scrollHeight;
}


// Send message on pressing Enter key
document.getElementById('chatInput').addEventListener('keyup', function (event) {
    if (event.key === 'Enter') {
        sendMessage();
    }
});

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/devConsoleHub")
    .build();

connection.on("PushEventLog", function (message) {
    streamConsoleLog(message);
});

connection.start().catch(function (err) {
    console.error(err.toString());
});

fetchReservations();

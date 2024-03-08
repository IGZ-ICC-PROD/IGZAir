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

async function sendMessage() {
    const chatInput = document.getElementById('chatInput');
    const message = chatInput.value;
    if (message.trim() !== '') {
        const chatMessages = document.getElementById('chatMessages');
        const userMessage = document.createElement('div');
        userMessage.classList.add('message', 'user-message');
        userMessage.textContent = `You: ${message}`;
        chatMessages.appendChild(userMessage);
        chatInput.value = '';

        try {
            const response = await axios.post(`/api/chat/${conversationId}`, { message });
            const aiResponse = response.data;
            const aiMessage = document.createElement('div');
            aiMessage.classList.add('message');
            aiMessage.textContent = `IGZ Air: ${aiResponse}`;
            chatMessages.appendChild(aiMessage);

            // Scroll to the bottom of the chatbox
            chatMessages.scrollTop = chatMessages.scrollHeight;

            // Refresh the reservations table after receiving the AI response
            await fetchReservations();
        } catch (error) {
            console.error('Error sending message to AI agent:', error);
        }
    }
}

fetchReservations();

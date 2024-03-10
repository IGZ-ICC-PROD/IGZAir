
function displayReservations(reservations) {
    const tableBody = document.querySelector('#reservationsTable tbody');
    tableBody.innerHTML = '';

    reservations.forEach(reservation => {
        const row = document.createElement('tr');
        const formattedDate = formatDate(reservation.departureDate);
        row.innerHTML = `
            <td>${reservation.reservationId}</td>
            <td>${reservation.customerName}</td>
            <td>${reservation.flightNumber}</td>
            <td>${formattedDate}</td>
            <td>${reservation.from}</td>
            <td>${reservation.to}</td>
        `;
        tableBody.appendChild(row);
    });
}

function formatDate(dateString) {
    const options = { day: '2-digit', month: '2-digit', year: 'numeric' };
    const date = new Date(dateString);
    return date.toLocaleDateString('de-DE', options);
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

document.addEventListener('DOMContentLoaded', fetchReservations);

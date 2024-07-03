function initializeMap(chargingStations) {
    var map = L.map('map').setView([45.95, 12.66], 13); // Coordinate per Pordenone

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors'
    }).addTo(map);

    // Aggiungi i marker per le colonnine di ricarica
    chargingStations.forEach(station => {
        var isActiveClass = station.isActive ? '' : 'disabled'; // Aggiungi classe 'disabled' se la stazione non è attiva
        var buttonDisabled = station.isActive ? '' : 'disabled="disabled"'; // Disabilita il pulsante se la stazione non è attiva
        var buttonColor = station.isActive ? 'btn-primary' : 'btn-secondary'; // Imposta il colore del pulsante in base allo stato

        var popupContent = `
            <p>${station.address}</p>
            <p>Prezzo: ${station.kwPrice} €/kWh</p>
            <p>Stato: ${station.isActive ? 'Attivo' : 'Disattivo'}</p>
            <button class="btn ${buttonColor} ${isActiveClass}" onclick="startCharge(${station.id})" ${buttonDisabled}>START CHARGE</button>
        `;

        var marker = L.marker([station.latitude, station.longitude]).addTo(map)
            .bindPopup(popupContent)
            .openPopup();
    });
}

function startCharge(stationId) {
    console.log(`Avviare la carica per la colonnina con ID ${stationId}`);
    window.location.href = `/startcharge/${stationId}`;
}

window.showConfirmationModal = function () {
    var modal = document.getElementById('confirmModal');
    if (modal) {
        modal.style.display = 'block';
    }
};

window.hideConfirmationModal = function () {
    var modal = document.getElementById('confirmModal');
    if (modal) {
        modal.style.display = 'none';
    }
};

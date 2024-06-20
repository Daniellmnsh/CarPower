// wwwroot/js/site.js
function initializeMap(chargingStations) {
    var map = L.map('map').setView([45.95, 12.66], 13); // Coordinate per Pordenone

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors'
    }).addTo(map);

    // Aggiungi i marker per le colonnine di ricarica
    chargingStations.forEach(station => {
        var marker = L.marker([station.latitude, station.longitude]).addTo(map)
            .bindPopup(`${station.address} - ${station.kwPrice} €/kWh`)
            .openPopup();
    });
}

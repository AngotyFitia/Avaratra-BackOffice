
// Import CSV
document.getElementById('btnImportCsv').addEventListener('click', function (e) {
    e.preventDefault(); // empêche le submit direct
    document.getElementById('csvFileInput').click();
});

document.getElementById('csvFileInput').addEventListener('change', function () {
    // une fois le fichier choisi, soumettre le formulaire
    this.form.submit();
});

// Google Maps localisation
var map, marker;
function initMap() {
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 0, lng: 0 },
        zoom: 2
    });
    map.addListener('click', function (e) {
        placeMarker(e.latLng);
    });
}

function placeMarker(location) {
    if (marker) marker.setPosition(location);
    else marker = new google.maps.Marker({ position: location, map: map });

    document.getElementById('Commune_latitude').value = location.lat();
    document.getElementById('Commune_longitude').value = location.lng();
}

// Position actuelle
document.getElementById("btnGetLocation").addEventListener("click", function () {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            document.getElementById("Commune_latitude").value = position.coords.latitude;
            document.getElementById("Commune_longitude").value = position.coords.longitude;
        }, function (error) {
            alert("Impossible de récupérer la position : " + error.message);
        });
    } else {
        alert("La géolocalisation n'est pas supportée par votre navigateur.");
    }
});

document.addEventListener('DOMContentLoaded', function () {
    var editModal = document.getElementById('editModal');
    if (editModal) {
        editModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            editModal.querySelector('#communeId').value = button.getAttribute('data-id');
            editModal.querySelector('#communeName').value = button.getAttribute('data-name');
            editModal.querySelector('#communePop').value = button.getAttribute('data-pop');

            // Sélectionner la région actuelle
            var districtSelect = editModal.querySelector('#districtList');
            var currentDistrictId = button.getAttribute('data-discrict-id');
            if (districtSelect && currentDistrictId) {
                districtSelect.value = currentDistrictId;
            }
        });
    }

    var deleteModal = document.getElementById('deleteModal');
    if (deleteModal) {
        deleteModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            deleteModal.querySelector('#communeId').value = button.getAttribute('data-id');
            deleteModal.querySelector('#communeName').textContent = button.getAttribute('data-name');
        });
    }

    var validateModal = document.getElementById('validateModal');
    if (validateModal) {
        validateModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            validateModal.querySelector('#communeId').value = button.getAttribute('data-id');
            validateModal.querySelector('#communeName').textContent = button.getAttribute('data-name');
        });
    }
});



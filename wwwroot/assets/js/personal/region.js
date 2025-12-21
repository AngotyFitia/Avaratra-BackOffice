
// Import CSV
document.getElementById('btnImportCsv').addEventListener('click', function () {
    document.getElementById('csvFileInput').click();
});

document.getElementById('csvFileInput').addEventListener('change', function (event) {
    const file = event.target.files[0];
    if (file) {
        console.log("Fichier choisi :", file.name);
    }
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

    document.getElementById('Region_latitude').value = location.lat();
    document.getElementById('Region_longitude').value = location.lng();
}

// Position actuelle
document.getElementById("btnGetLocation").addEventListener("click", function () {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            document.getElementById("Region_latitude").value = position.coords.latitude;
            document.getElementById("Region_longitude").value = position.coords.longitude;
        }, function (error) {
            alert("Impossible de récupérer la position : " + error.message);
        });
    } else {
        alert("La géolocalisation n'est pas supportée par votre navigateur.");
    }
});

// Modification, suppression et validation modal
document.addEventListener('DOMContentLoaded', function () {
    var editModal = document.getElementById('editModal');
    if (editModal) {
        editModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            editModal.querySelector('#regionId').value = button.getAttribute('data-id');
            editModal.querySelector('#regionName').value = button.getAttribute('data-name');
            editModal.querySelector('#regionLat').value = button.getAttribute('data-lat');
            editModal.querySelector('#regionLng').value = button.getAttribute('data-lng');
            editModal.querySelector('#regionPop').value = button.getAttribute('data-pop');
        });
    }

    var deleteModal = document.getElementById('deleteModal');
    if (deleteModal) {
        deleteModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            deleteModal.querySelector('#regionId').value = button.getAttribute('data-id');
            deleteModal.querySelector('#regionName').textContent = button.getAttribute('data-name');
        });
    }

    var validateModal = document.getElementById('validateModal');
    if (validateModal) {
        validateModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            validateModal.querySelector('#regionId').value = button.getAttribute('data-id');
            validateModal.querySelector('#regionName').textContent = button.getAttribute('data-name');
        });
    }
});

// Suppression de tout, validation de tout
document.addEventListener('DOMContentLoaded', function () {
    // Sélectionner/désélectionner toutes les cases
    document.getElementById('selectAll').addEventListener('change', function () {
        const checked = this.checked;
        document.querySelectorAll('.rowCheckbox').forEach(cb => cb.checked = checked);
    });

    document.getElementById('deleteAllBtn').addEventListener('click', function () {
        const ids = Array.from(document.querySelectorAll('.rowCheckbox:checked'))
                 .map(cb => parseInt(cb.getAttribute('data-id')));


        if (ids.length === 0) {
            alert("Aucune région sélectionnée.");
            return;
        }

        fetch('?handler=DeleteSelected', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(ids)
        })
        .then(resp => {
            if (resp.ok) {
                location.reload();
            } else {
                    return resp.json().then(data => {
                        alert("Erreur : " + data.error);
                    });
            }
        })
        .catch(err => {
            alert("Erreur réseau : " + err);
        });
        
    });

    document.getElementById('validateAllBtn').addEventListener('click', function () {
        const ids = Array.from(document.querySelectorAll('.rowCheckbox:checked'))
                         .map(cb => cb.getAttribute('data-id'));

        if (ids.length === 0) {
            alert("Aucune région sélectionnée.");
            return;
        }

        fetch('?handler=ValidateSelected', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(ids)
        })
        .then(resp => {
            if (resp.ok) location.reload();
            else alert("Erreur lors de la validation.");
        });
    });
});

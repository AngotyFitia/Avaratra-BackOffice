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

    document.getElementById('District_latitude').value = location.lat();
    document.getElementById('District_longitude').value = location.lng();
}

// Position actuelle
document.getElementById("btnGetLocation").addEventListener("click", function () {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            document.getElementById("District_latitude").value = position.coords.latitude;
            document.getElementById("District_longitude").value = position.coords.longitude;
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
            editModal.querySelector('#regionId').value = button.getAttribute('data-id');
            editModal.querySelector('#regionName').value = button.getAttribute('data-name');
            editModal.querySelector('#regionPop').value = button.getAttribute('data-pop');

            // Sélectionner la région actuelle
            var regionSelect = editModal.querySelector('#regionList');
            var currentRegionId = button.getAttribute('data-regionid');
            if (regionSelect && currentRegionId) {
                regionSelect.value = currentRegionId;
            }
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

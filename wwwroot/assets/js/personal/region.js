// Import CSV
document.addEventListener("DOMContentLoaded", function () {
    const btnImportCsv = document.getElementById('btnImportCsv');
    const csvFileInput = document.getElementById('csvFileInput');

    if (btnImportCsv && csvFileInput) {
        btnImportCsv.addEventListener('click', function (e) {
            e.preventDefault(); // empêche le submit direct
            csvFileInput.click();
        });

        csvFileInput.addEventListener('change', function () {
            // une fois le fichier choisi, soumettre le formulaire
            if (this.form) this.form.submit();
        });
    }
});

// Position actuelle (géolocalisation)
document.addEventListener("DOMContentLoaded", function () {
    const btnGetLocation = document.getElementById("btnGetLocation");

    if (btnGetLocation) {
        btnGetLocation.addEventListener("click", function () {
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
    }
});

// Modals (édition, suppression, validation, détails)
document.addEventListener('DOMContentLoaded', function () {
    const editModal = document.getElementById('editModal');
    if (editModal) {
        editModal.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            editModal.querySelector('#regionId').value = button.getAttribute('data-id');
            editModal.querySelector('#regionName').value = button.getAttribute('data-name');
            editModal.querySelector('#regionPop').value = button.getAttribute('data-pop');
        });
    }

    const deleteModal = document.getElementById('deleteModal');
    if (deleteModal) {
        deleteModal.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            deleteModal.querySelector('#regionId').value = button.getAttribute('data-id');
            deleteModal.querySelector('#regionName').textContent = button.getAttribute('data-name');
        });
    }

    const validateModal = document.getElementById('validateModal');
    if (validateModal) {
        validateModal.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            validateModal.querySelector('#regionId').value = button.getAttribute('data-id');
            validateModal.querySelector('#regionName').textContent = button.getAttribute('data-name');
        });
    }

    const viewModal = document.getElementById('viewModal');
    if (viewModal) {
        viewModal.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;

            // Infos simples depuis data-*
            viewModal.querySelector('#regionName').textContent = button.getAttribute('data-name');
            viewModal.querySelector('#regionPop').textContent = button.getAttribute('data-pop');

            // Charger les districts depuis le serveur
            const id = button.getAttribute('data-id');
            fetch(`/Regions?handler=Districts&id=${id}`)
                .then(resp => resp.json())
                .then(districts => {
                    viewModal.querySelector('#regionDistricts').textContent =
                        districts.length === 0 ? "Aucun district lié" : districts.length;
                })
                .catch(() => {
                    viewModal.querySelector('#regionDistricts').textContent = "Erreur de chargement";
                });
        });
    }
});

// Actions groupées (suppression / validation)
document.addEventListener('DOMContentLoaded', function () {
    const deleteUrl = '@Url.Page("Index", "DeleteSelected")';
    const validateUrl = '@Url.Page("Index", "ValidateSelected")';
    const selectAll = document.getElementById('selectAll');
    const deleteAllBtn = document.getElementById('deleteAllBtn');
    const validateAllBtn = document.getElementById('validateAllBtn');

    // Sélectionner / désélectionner toutes les cases
    if (selectAll) {
        selectAll.addEventListener('change', function () {
            const checked = this.checked;
            document.querySelectorAll('.rowCheckbox').forEach(cb => cb.checked = checked);
        });
    }

    // Suppression groupée
    if (deleteAllBtn) {
        deleteAllBtn.addEventListener('click', function () {
            const ids = Array.from(document.querySelectorAll('.rowCheckbox:checked'))
                .map(cb => parseInt(cb.dataset.id));

            if (ids.length === 0) {
                alert("Aucune région sélectionnée.");
                return;
            }
            console.log(JSON.stringify(ids));

            fetch('/Regions?handler=DeleteSelected', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(ids)
            })
            .then(resp => {
                if (resp.ok) { 
                    location.reload(); 
                } 
                else { 
                    return resp.json().then(data => alert("Erreur : " + data.error)); 
                }
            })
            .catch(err => alert("Erreur réseau : " + err.message));
        });
    }

    // Validation groupée
    if (validateAllBtn) {
        validateAllBtn.addEventListener('click', function () {
            const ids = Array.from(document.querySelectorAll('.rowCheckbox:checked'))
                .map(cb => cb.dataset.id);

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
            })
            .catch(err => alert("Erreur réseau : " + err));
        });
    }
});


// Toast notifications
document.addEventListener("DOMContentLoaded", function () {
    const toastEl = document.getElementById("liveToast");
    if (toastEl) {
        toastEl.classList.add("show");
        setTimeout(() => toastEl.classList.remove("show"), 3000);
    }
});

function hideToast() {
    const toastEl = document.getElementById("liveToast");
    if (toastEl) toastEl.classList.remove("show");
    console.log("toast");
}

// Pagination select
function toggleCustomInput(select) {
    const input = document.getElementById('customPageSize');
    if (!input) return;

    if (select.value === 'custom') {
        input.style.display = 'inline-block';
    } else {
        input.style.display = 'none';
        input.value = select.value;
    }
}

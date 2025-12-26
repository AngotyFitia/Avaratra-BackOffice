document.addEventListener('DOMContentLoaded', function () {
    var editModal = document.getElementById('editModal');
    if (editModal) {
        editModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            editModal.querySelector('#infrastructureId').value = button.getAttribute('data-id');
            editModal.querySelector('#infrastructureName').value = button.getAttribute('data-name');

            var regionSelect = editModal.querySelector('#categoryList');
            var currentRegionId = button.getAttribute('data-categorieId');
            if (regionSelect && currentRegionId) {
                regionSelect.value = currentRegionId;
            }
        });
    }

    var deleteModal = document.getElementById('deleteModal');
    if (deleteModal) {
        deleteModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            deleteModal.querySelector('#infrastructureId').value = button.getAttribute('data-id');
            deleteModal.querySelector('#infrastructureName').textContent = button.getAttribute('data-name');
        });
    }

    var validateModal = document.getElementById('validateModal');
    if (validateModal) {
        validateModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            validateModal.querySelector('#infrastructureId').value = button.getAttribute('data-id');
            validateModal.querySelector('#infrastructureName').textContent = button.getAttribute('data-name');
        });
    }
});
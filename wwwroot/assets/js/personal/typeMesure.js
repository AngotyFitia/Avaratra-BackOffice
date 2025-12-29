document.addEventListener('DOMContentLoaded', function () {
    var editModal = document.getElementById('editModal');
    if (editModal) {
        editModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            editModal.querySelector('#typeMesureId').value = button.getAttribute('data-id');
            editModal.querySelector('#typeMesureName').value = button.getAttribute('data-name');
            editModal.querySelector('#typeMesureDescription').value = button.getAttribute('data-description');

            var regionSelect = editModal.querySelector('#unityList');
            var currentRegionId = button.getAttribute('data-unitiesIds');
            if (regionSelect && currentRegionId) {
                regionSelect.value = currentRegionId;
            }
        });
    }

    var deleteModal = document.getElementById('deleteModal');
    if (deleteModal) {
        deleteModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            deleteModal.querySelector('#typeMesureId').value = button.getAttribute('data-id');
            deleteModal.querySelector('#typeMesureName').textContent = button.getAttribute('data-name');
        });
    }

    var validateModal = document.getElementById('validateModal');
    if (validateModal) {
        validateModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            validateModal.querySelector('#typeMesureId').value = button.getAttribute('data-id');
            validateModal.querySelector('#typeMesureName').textContent = button.getAttribute('data-name');
        });
    }
});
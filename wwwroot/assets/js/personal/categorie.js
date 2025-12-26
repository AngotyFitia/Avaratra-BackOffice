document.addEventListener('DOMContentLoaded', function () {
    var editModal = document.getElementById('editModal');
    if (editModal) {
        editModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            editModal.querySelector('#categorieId').value = button.getAttribute('data-id');
            editModal.querySelector('#categorieName').value = button.getAttribute('data-name');

        });
    }

    var deleteModal = document.getElementById('deleteModal');
    if (deleteModal) {
        deleteModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            deleteModal.querySelector('#categorieId').value = button.getAttribute('data-id');
            console.log(button.getAttribute('data-id'));
            console.log(button.getAttribute('data-name'));
            deleteModal.querySelector('#categorieName').textContent = button.getAttribute('data-name');
        });
    }

    var validateModal = document.getElementById('validateModal');
    if (validateModal) {
        validateModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            validateModal.querySelector('#categorieId').value = button.getAttribute('data-id');
            validateModal.querySelector('#categorieName').textContent = button.getAttribute('data-name');
        });
    }
});
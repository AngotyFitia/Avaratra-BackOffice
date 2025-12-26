document.addEventListener('DOMContentLoaded', function () {
    var editModal = document.getElementById('editModal');
    if (editModal) {
        editModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            editModal.querySelector('#uniteId').value = button.getAttribute('data-id');
            editModal.querySelector('#uniteName').value = button.getAttribute('data-name');

        });
    }

    var deleteModal = document.getElementById('deleteModal');
    if (deleteModal) {
        deleteModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            deleteModal.querySelector('#uniteId').value = button.getAttribute('data-id');
            console.log(button.getAttribute('data-id'));
            console.log(button.getAttribute('data-name'));
            deleteModal.querySelector('#uniteName').textContent = button.getAttribute('data-name');
        });
    }

    var validateModal = document.getElementById('validateModal');
    if (validateModal) {
        validateModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            validateModal.querySelector('#uniteId').value = button.getAttribute('data-id');
            validateModal.querySelector('#uniteName').textContent = button.getAttribute('data-name');
        });
    }
});
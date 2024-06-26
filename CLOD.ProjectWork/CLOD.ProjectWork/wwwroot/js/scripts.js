window.showConfirmationModal = function() {
    var modal = document.getElementById('confirmModal');
    if (modal) {
        modal.style.display = 'block';
    }
};

window.hideConfirmationModal = function() {
    var modal = document.getElementById('confirmModal');
    if (modal) {
        modal.style.display = 'none';
    }
};

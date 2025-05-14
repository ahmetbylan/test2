document.addEventListener('DOMContentLoaded', function () {
    const productModal = document.getElementById('productModal');
    if (productModal) {
        productModal.addEventListener('show.bs.modal', function () {
            const modalDialog = this.querySelector('.modal-dialog');
            if (window.innerWidth < 768) {
                modalDialog.classList.add('modal-fullscreen-sm-down');
            } else {
                modalDialog.classList.remove('modal-fullscreen-sm-down');
            }
        });
        window.addEventListener('resize', function () {
            const modalDialog = productModal.querySelector('.modal-dialog');
            if (productModal.classList.contains('show')) { // Modal açıksa kontrol et
                if (window.innerWidth < 768) {
                    modalDialog.classList.add('modal-fullscreen-sm-down');
                } else {
                    modalDialog.classList.remove('modal-fullscreen-sm-down');
                }
            }
        });
    }
});
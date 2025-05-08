// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Mobil menü toggle işlevi
document.addEventListener('DOMContentLoaded', function () {
    // Dropdown menüler için
    var dropdowns = document.querySelectorAll('.dropdown-toggle');
    dropdowns.forEach(function (dropdown) {
        dropdown.addEventListener('click', function (e) {
            if (window.innerWidth < 992) {
                e.preventDefault();
                var menu = this.nextElementSibling;
                menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
            }
        });
    });

    // Aktif menü öğesini vurgula
    var currentPath = window.location.pathname;
    document.querySelectorAll('.nav-link').forEach(function (link) {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
            link.innerHTML = '<i class="fas fa-arrow-right me-2"></i>' + link.textContent;
        }
    });
});
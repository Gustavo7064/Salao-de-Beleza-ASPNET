// Camada de apresentação apenas — nenhuma lógica de negócio aqui.

document.addEventListener('DOMContentLoaded', function () {
    var toggle = document.getElementById('sidebarToggle');
    var sidebar = document.getElementById('sidebar');

    if (toggle && sidebar) {
        toggle.addEventListener('click', function () {
            sidebar.classList.toggle('open');
        });

        document.addEventListener('click', function (e) {
            var isClickInsideSidebar = sidebar.contains(e.target);
            var isClickOnToggle = toggle.contains(e.target);
            if (!isClickInsideSidebar && !isClickOnToggle && sidebar.classList.contains('open')) {
                sidebar.classList.remove('open');
            }
        });
    }

    // Auto-dismiss de alertas de sucesso após alguns segundos
    document.querySelectorAll('.alert-success').forEach(function (alertEl) {
        setTimeout(function () {
            if (window.bootstrap && bootstrap.Alert) {
                var instance = bootstrap.Alert.getOrCreateInstance(alertEl);
                instance.close();
            }
        }, 5000);
    });
});

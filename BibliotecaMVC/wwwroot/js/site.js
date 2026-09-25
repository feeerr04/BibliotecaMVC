// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(function () {
    // Muestra un mensaje de éxito (SweetAlert) que llega desde TempData en cualquier vista.
    var successMessage = $('body').data('success-message');
    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: 'Éxito',
            text: successMessage,
            timer: 2000,
            showConfirmButton: false
        });
    }

    // Solicita confirmación antes de eliminar un registro.
    // Requiere: un botón con clase "btn-eliminar", atributo data-id, y opcionalmente
    // data-entidad (ej. "el autor"); y un <form> oculto con id "form-eliminar-{id}".
    $(document).on('click', '.btn-eliminar', function () {
        var boton = this;
        var id = boton.getAttribute('data-id');
        var entidad = boton.getAttribute('data-entidad') || 'el registro';

        Swal.fire({
            title: '¿Eliminar ' + entidad + '?',
            text: 'Esta acción no se puede deshacer.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then(function (resultado) {
            if (resultado.isConfirmed) {
                document.getElementById('form-eliminar-' + id).submit();
            }
        });
    });

    // Solicita confirmación antes de guardar cambios en un formulario de edición.
    // Requiere: un <form> con clase "form-editar" y opcionalmente data-entidad.
    $(document).on('submit', 'form.form-editar', function (e) {
        e.preventDefault();
        var form = this;
        var entidad = $(form).data('entidad') || 'los cambios';

        Swal.fire({
            title: '¿Guardar cambios?',
            text: 'Esta acción no se puede deshacer.',
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#0d6efd',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Sí, guardar',
            cancelButtonText: 'Cancelar'
        }).then(function (resultado) {
            if (resultado.isConfirmed) {
                form.submit();
            }
        });
    });
});

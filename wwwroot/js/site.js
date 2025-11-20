// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    // Função para restringir a entrada a apenas dígitos (0-9).
    $('.js-numbers-only').on('input', function() {
        var input = $(this);
        var value = input.val();
        
        // Expressão Regular que remove qualquer caractere que NÃO seja um dígito (globalmente).
        var filteredValue = value.replace(/[^\d]/g, ''); 
        
        // Verifica se houve alguma alteração (se caracteres não-numéricos foram digitados)
        if (value !== filteredValue) {
            // Se sim, atualiza o campo com o valor filtrado.
            input.val(filteredValue);
        }
    });

    // Opcional, mas recomendado: Limitar o comprimento para CPF (11 dígitos)
    $('.js-numbers-only[asp-for="Cpf"]').attr('maxlength', '11');
});
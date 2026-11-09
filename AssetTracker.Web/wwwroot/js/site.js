// =====================================================
// AssetTracker – Utilidades reutilizables (11B)
// =====================================================

/**
 * Inicializa una búsqueda en vivo (AJAX con debounce) sobre un input.
 * @param {string} inputId     Id del input de texto.
 * @param {string} tablaBodyId Id del <tbody> que se redibuja.
 * @param {string} urlTemplate URL con placeholder {texto} (MISMO ORIGEN: la pagina Razor
 *                             hace de proxy hacia la API; evita CORS y oculta el backend).
 * @param {function(object): string} filaHtml  Función que arma el HTML de cada fila.
 * @param {number} [ms=300]    Demora del debounce en milisegundos.
 */
function inicializarBusquedaEnVivo(inputId, tablaBodyId, urlTemplate, filaHtml, ms = 300) {
    const input = document.getElementById(inputId);
    const tbody = document.getElementById(tablaBodyId);
    if (!input || !tbody) return;

    let timer = null;

    input.addEventListener('input', () => {
        clearTimeout(timer);
        timer = setTimeout(async () => {
            const texto = input.value.trim();
            if (texto.length < 2) { tbody.innerHTML = ''; return; }

            try {
                const resp = await fetch(urlTemplate.replace('{texto}', encodeURIComponent(texto)));
                if (!resp.ok) { tbody.innerHTML = ''; return; }
                const datos = await resp.json();

                tbody.innerHTML = datos.length
                    ? datos.map(a => filaHtml(a)).join('')
                    : `<tr><td colspan="5" class="text-center text-muted">Sin resultados para "${texto}"</td></tr>`;
            } catch (e) {
                tbody.innerHTML = '<tr><td colspan="5" class="text-center text-danger">No se pudo conectar con el servicio de búsqueda.</td></tr>';
            }
        }, ms);
    });
}
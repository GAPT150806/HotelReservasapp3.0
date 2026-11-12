
document.addEventListener("DOMContentLoaded", function () {

    const toggle = document.getElementById("sidebarToggle");
    const sidebar = document.getElementById("appSidebar");
    const overlay = document.getElementById("sidebarOverlay");

    if (!toggle || !sidebar || !overlay)
        return;

    function closeSidebar() {
        sidebar.classList.remove("is-open");
        overlay.classList.remove("is-visible");
        toggle.setAttribute("aria-expanded", "false");
    }

    toggle.addEventListener("click", function () {
        const isOpen = sidebar.classList.toggle("is-open");

        overlay.classList.toggle("is-visible", isOpen);
        toggle.setAttribute("aria-expanded", String(isOpen));
    });

    overlay.addEventListener("click", closeSidebar);

    sidebar.querySelectorAll(".sidebar-link").forEach(link => {
        link.addEventListener("click", closeSidebar);
    });

});


// =============================================
// BUSQUEDA Y ORDENAMIENTO DE TABLAS
// =============================================

document.addEventListener("DOMContentLoaded", function () {

    document.querySelectorAll("table[data-enhanced-table]").forEach(table => {

        const container = table.closest(".card-body");
        const searchInput = container?.querySelector("[data-table-search]");
        const countLabel = container?.querySelector("[data-table-count]");
        const tbody = table.querySelector("tbody");

        if (!tbody)
            return;

        const rows = Array.from(tbody.querySelectorAll("tr"));
        const headers = Array.from(table.querySelectorAll("thead th"));

        let sortColumn = -1;
        let sortDirection = "asc";

        // Crear fila para cuando no existan coincidencias.
        const noResultsRow = document.createElement("tr");
        noResultsRow.className = "table-no-results";
        noResultsRow.style.display = "none";

        const noResultsCell = document.createElement("td");
        noResultsCell.colSpan = headers.length;
        noResultsCell.textContent = "No se encontraron resultados.";

        noResultsRow.appendChild(noResultsCell);
        tbody.appendChild(noResultsRow);

        function getCellValue(row, index) {
            return row.cells[index]?.innerText.trim() ?? "";
        }

        function compareValues(a, b) {
            const cleanA = a.replace(/[^\d,.-]/g, "").replace(/,/g, "");
            const cleanB = b.replace(/[^\d,.-]/g, "").replace(/,/g, "");

            const isNumberA = /^-?\d+(\.\d+)?$/.test(cleanA);
            const isNumberB = /^-?\d+(\.\d+)?$/.test(cleanB);

            if (isNumberA && isNumberB) {
                return Number(cleanA) - Number(cleanB);
            }

            return a.localeCompare(b, "es", {
                numeric: true,
                sensitivity: "base"
            });
        }

        function sortTable(index, direction) {

            const sortedRows = [...rows].sort((rowA, rowB) => {
                const valueA = getCellValue(rowA, index);
                const valueB = getCellValue(rowB, index);

                const result = compareValues(valueA, valueB);

                return direction === "asc" ? result : -result;
            });

            sortedRows.forEach(row => tbody.appendChild(row));

            headers.forEach(header => {
                header.removeAttribute("aria-sort");
                header.removeAttribute("data-sort-direction");
            });

            headers[index].setAttribute(
                "aria-sort",
                direction === "asc" ? "ascending" : "descending"
            );

            headers[index].dataset.sortDirection = direction;

            sortColumn = index;
            sortDirection = direction;

            applySearch();
        }

        function applySearch() {

            const term = (searchInput?.value ?? "")
                .trim()
                .toLocaleLowerCase("es");

            let visibleCount = 0;

            rows.forEach(row => {

                // La última columna contiene las acciones.
                const searchableText = Array.from(row.cells)
                    .slice(0, -1)
                    .map(cell => cell.innerText)
                    .join(" ")
                    .toLocaleLowerCase("es");

                const matches = searchableText.includes(term);

                row.style.display = matches ? "" : "none";

                if (matches)
                    visibleCount++;
            });

            noResultsRow.style.display =
                visibleCount === 0 ? "" : "none";

            if (countLabel) {
                countLabel.textContent =
                    `Mostrando ${visibleCount} de ${rows.length} registros`;
            }
        }

        // Buscar mientras se escribe.
        searchInput?.addEventListener("input", applySearch);

        // Ordenar al pulsar encabezados.
        headers.forEach((header, index) => {

            if (!header.hasAttribute("data-sortable"))
                return;

            header.classList.add("sortable-header");
            header.setAttribute("role", "button");
            header.setAttribute("tabindex", "0");

            function toggleSort() {
                const direction =
                    sortColumn === index && sortDirection === "asc"
                        ? "desc"
                        : "asc";

                sortTable(index, direction);
            }

            header.addEventListener("click", toggleSort);

            header.addEventListener("keydown", event => {
                if (event.key === "Enter" || event.key === " ") {
                    event.preventDefault();
                    toggleSort();
                }
            });
        });

        // Orden inicial: primera columna ascendente.
        const firstSortable = headers.findIndex(
            header => header.hasAttribute("data-sortable")
        );

        if (firstSortable >= 0)
            sortTable(firstSortable, "asc");
        else
            applySearch();
    });

});


// =============================================
// CAMBIO DE ESTADO DE HABITACIONES CON AJAX
// =============================================

document.addEventListener("DOMContentLoaded", function () {

    const feedback = document.querySelector("[data-state-feedback]");

    function showFeedback(message, success) {

        if (!feedback)
            return;

        feedback.textContent = message;
        feedback.className = success
            ? "alert alert-success"
            : "alert alert-danger";

        clearTimeout(feedback.hideTimer);

        feedback.hideTimer = setTimeout(() => {
            feedback.classList.add("d-none");
        }, 4000);
    }

    function getBadgeClass(state) {

        const classes = {
            Disponible: "bg-success",
            Reservada: "bg-primary",
            Ocupada: "bg-info text-dark",
            Limpieza: "bg-warning text-dark",
            Mantenimiento: "bg-secondary",
            FueraServicio: "bg-danger"
        };

        return classes[state] ?? "bg-dark";
    }

    document.querySelectorAll("[data-state-form]").forEach(form => {

        const select = form.querySelector("[data-state-select]");

        if (!select)
            return;

        // Guardar el estado original para poder revertirlo si falla.
        select.dataset.previousValue = select.value;

        select.addEventListener("change", async function () {

            const previousValue = select.dataset.previousValue;
            const newValue = select.value;

            if (newValue === previousValue)
                return;

            select.disabled = true;
            form.classList.add("is-saving");

            try {

                const response = await fetch(form.action, {
                    method: "POST",
                    body: new FormData(form),
                    headers: {
                        "X-Requested-With": "XMLHttpRequest",
                        "Accept": "application/json"
                    }
                });

                const result = await response.json();

                if (!response.ok || !result.success) {
                    throw new Error(
                        result.message || "No se pudo actualizar el estado."
                    );
                }

                // Actualizar visualmente la etiqueta.
                const row = form.closest("tr");
                const badge = row?.querySelector("[data-state-badge]");

                if (badge) {
                    badge.textContent = result.estadoNombre;
                    badge.className = `badge ${getBadgeClass(result.estado)}`;
                }

                // Guardar el nuevo estado como estado actual.
                select.dataset.previousValue = newValue;

                showFeedback(result.message, true);

                // Actualizar el filtro y el contador si hay búsqueda activa.
                const searchInput = form.closest(".card-body")
                    ?.querySelector("[data-table-search]");

                searchInput?.dispatchEvent(new Event("input"));

            }
            catch (error) {

                // Revertir el selector si el servidor rechaza el cambio.
                select.value = previousValue;

                showFeedback(error.message, false);

            }
            finally {

                select.disabled = false;
                form.classList.remove("is-saving");

            }

        });

    });

});

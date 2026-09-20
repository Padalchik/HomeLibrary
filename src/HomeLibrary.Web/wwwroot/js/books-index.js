(() => {
    const interactiveSelector = "a, button, form, input, textarea, select, [role='button'], [contenteditable='true']";

    document.querySelectorAll(".book-row[data-href]").forEach((row) => {
        row.addEventListener("click", (event) => {
            if (event.defaultPrevented || event.target.closest(interactiveSelector)) {
                return;
            }

            window.location.assign(row.dataset.href);
        });

        row.addEventListener("keydown", (event) => {
            if (event.key !== "Enter" || event.target !== row) {
                return;
            }

            window.location.assign(row.dataset.href);
        });
    });

    const form = document.querySelector(".search-form");
    const input = document.querySelector("#search");

    if (!form || !input) {
        return;
    }

    const focusKey = "books-search-focus";
    let debounceTimer;

    try {
        if (sessionStorage.getItem(focusKey) === "true") {
            sessionStorage.removeItem(focusKey);
            input.focus();
            input.setSelectionRange(input.value.length, input.value.length);
        }
    } catch {
        // Search still works when browser storage is unavailable.
    }

    const rememberFocus = () => {
        try {
            sessionStorage.setItem(focusKey, "true");
        } catch {
            // Focus restoration is an optional enhancement.
        }
    };

    const runSearch = () => {
        rememberFocus();

        if (input.value.trim() === "") {
            window.location.assign(form.action);
            return;
        }

        form.requestSubmit();
    };

    input.addEventListener("input", () => {
        window.clearTimeout(debounceTimer);
        debounceTimer = window.setTimeout(runSearch, 400);
    });

    form.addEventListener("submit", () => {
        window.clearTimeout(debounceTimer);
        rememberFocus();
    });
})();

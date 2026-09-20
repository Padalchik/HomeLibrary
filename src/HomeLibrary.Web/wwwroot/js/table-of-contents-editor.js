(() => {
    if (typeof window.Quill === "undefined") {
        return;
    }

    document.querySelectorAll("[data-html-editor]").forEach(container => {
        const textarea = container.querySelector("[data-html-editor-input]");
        const surface = container.querySelector("[data-html-editor-surface]");

        if (!textarea || !surface) {
            return;
        }

        try {
            surface.hidden = false;

            const editor = new window.Quill(surface, {
                theme: "snow",
                formats: ["header", "bold", "italic", "underline", "list"],
                modules: {
                    toolbar: [
                        [{ header: [1, 2, 3, 4, false] }],
                        ["bold", "italic", "underline"],
                        [{ list: "ordered" }, { list: "bullet" }]
                    ]
                }
            });

            if (textarea.value) {
                editor.clipboard.dangerouslyPasteHTML(textarea.value);
            }

            textarea.hidden = true;

            const form = container.closest("form");
            form?.addEventListener("submit", () => {
                textarea.value = editor.root.innerHTML;
            });
        } catch {
            surface.hidden = true;
            textarea.hidden = false;
        }
    });
})();

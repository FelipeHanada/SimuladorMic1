export function initializeProgramInput(textAreaId, lineNumbersId) {
    const textarea = document.getElementById(textAreaId);
    const lineNumbers = document.getElementById(lineNumbersId);

    function updateLineNumbers() {
        //const cols = parseInt(textarea.getAttribute("cols"))
        const lines = textarea.value.split("\n").length;

        lineNumbers.innerHTML = Array.from({ length: lines }, (_, i) => i + 1).join("<br>");
        lineNumbers.style.height = `${textarea.offsetHeight}px`;
    }

    textarea.addEventListener("input", updateLineNumbers);
    textarea.addEventListener("scroll", () => {
        lineNumbers.scrollTop = textarea.scrollTop;
    });
    textarea.addEventListener("mouseup", updateLineNumbers);
    updateLineNumbers();
}

export function getProgramInputContent(textAreaId) {
    const textarea = document.getElementById(textAreaId);
    return textarea.value;
}

export function showErrorModal(message, lineNumber, symbol) {
    const errorMessage = document.getElementById("errorMessage");

    if (errorMessage) {
        let errorContent = "";

        switch (message) {
            case "syntax_error":
                errorContent = `Não foi possível interpretar a linha ${lineNumber}.`;
                break;
            case "duplicatted_symbol":
                errorContent = `Foi encontrada uma dupla declaração do símbolo '${symbol}'.`;
                break;
            case "symbol_not_defined":
                errorContent = `Uso do símbolo '${symbol}' não declarado.`;
                break;
            case "opcode_not_defined":
                errorContent = `OPCODE '${symbol}' não reconhecido na linha ${lineNumber}.`;
                break;
            default:
                errorContent = "Não foi possível detectar o erro.";
                break;
        }

        errorMessage.textContent = errorContent;
    }

    const modalElement = document.getElementById("errorModal");
    if (modalElement) {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    }
}

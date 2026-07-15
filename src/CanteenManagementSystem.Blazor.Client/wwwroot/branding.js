// Trigger a file download from a byte array passed from Blazor
window.downloadFileFromBytes = function (filename, mimeType, bytesBase64) {
    const byteCharacters = atob(bytesBase64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

window.readFileAsBase64 = function (inputId) {
    return new Promise((resolve, reject) => {
        const input = document.getElementById(inputId);
        if (!input || !input.files || input.files.length === 0) {
            resolve('');
            return;
        }
        const file = input.files[0];
        if (file.size > 2 * 1024 * 1024) {
            reject('File size exceeds 2MB limit');
            return;
        }
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result);
        reader.onerror = () => reject('Failed to read file');
        reader.readAsDataURL(file);
    });
};

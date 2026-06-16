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

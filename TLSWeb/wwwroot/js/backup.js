// window.downloadFileFromBytes = (fileName, base64, mimeType) => {
//     const link = document.createElement('a');
//     link.href = `data:${mimeType};base64,${base64}`;
//     link.download = fileName;
//     document.body.appendChild(link);
//     link.click();
//     document.body.removeChild(link);
// };

// wwwroot/js/backup.js
export function downloadFileFromBytes(fileName, base64, mimeType) {
    const link = document.createElement('a');
    link.href = `data:${mimeType};base64,${base64}`;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
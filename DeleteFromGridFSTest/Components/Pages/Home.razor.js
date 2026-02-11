export function setImageSourceAsByteArray(elementId, byteArray, contentType) {
    const arrayBuffer = byteArray;
    let blobOptions = {};
    if (contentType) {
        blobOptions['type'] = contentType;
    }
    const blob = new Blob([arrayBuffer], blobOptions);
    const url = URL.createObjectURL(blob);
    const element = document.getElementById(elementId);

    element.onload = () => {
        URL.revokeObjectURL(url);
    }
    element.src = url;
}
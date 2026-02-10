function alertOnScreen(msg) {
    var alertItem = document.querySelector('#update-alert');
    var alertItemMessage = document.querySelector('#update-alert-message');
    alertItemMessage.innerHTML = msg;
    alertItem.className = 'fadein';
    return true;
}

function removeAlertOnScreen() {
    var alertItem = document.querySelector('#update-alert');
    if (alertItem != null && alertItem.className == 'fadein') {
        alertItem.className = 'fadeout';
        setTimeout(function () { alertItem.className = ''; }, 1000);
    }
    return true;
}

function blazorMenu() {
    document.querySelector('ilw-header').removeAttribute('compact');
    return true;
}

window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}
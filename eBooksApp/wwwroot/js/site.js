// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function zoomIn() {
    let iframeplus = document.getElementById("iframe");
    iframeplus.width = 1.2 * iframeplus.width;
    iframeplus.height = 1.2 * iframeplus.height;
}
function zoomOut() {
    let iframeminus = document.getElementById("iframe");
    iframeminus.width = iframeminus.width / 1.2;
    iframeminus.height = iframeminus.height / 1.2;
}

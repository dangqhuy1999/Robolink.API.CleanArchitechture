/**
 * Shows toast notifications using your preferred library
 * Change this based on your toast library (Toastr, SweetAlert2, etc.)
 */
window.showToast = function(type, message, duration) {
    // Example 1: Using Toastr (npm: toastr)
    if (typeof toastr !== 'undefined') {
        toastr.options = {
            "positionClass": "toast-top-right",
            "timeOut": duration,
            "progressBar": true
        };
        
        switch(type) {
            case 'success':
                toastr.success(message);
                break;
            case 'error':
                toastr.error(message);
                break;
            case 'warning':
                toastr.warning(message);
                break;
            case 'info':
                toastr.info(message);
                break;
        }
        return;
    }

    // Example 2: Using Bootstrap Toast (built-in)
    const toastContainer = document.getElementById('toastContainer') || createToastContainer();
    const toastElement = document.createElement('div');
    toastElement.className = `toast align-items-center text-white bg-${getBootstrapColor(type)} border-0`;
    toastElement.setAttribute('role', 'alert');
    toastElement.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${escapeHtml(message)}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;
    
    toastContainer.appendChild(toastElement);
    const bootstrapToast = new bootstrap.Toast(toastElement);
    bootstrapToast.show();
    
    // Remove from DOM after hidden
    toastElement.addEventListener('hidden.bs.toast', () => toastElement.remove());
};

function getBootstrapColor(type) {
    switch(type) {
        case 'success': return 'success';
        case 'error': return 'danger';
        case 'warning': return 'warning';
        case 'info': return 'info';
        default: return 'secondary';
    }
}

function createToastContainer() {
    const container = document.createElement('div');
    container.id = 'toastContainer';
    container.className = 'toast-container position-fixed top-0 end-0 p-3';
    document.body.appendChild(container);
    return container;
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
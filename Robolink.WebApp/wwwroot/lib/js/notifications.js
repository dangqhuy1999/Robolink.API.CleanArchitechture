window.showToast = (type, message, duration) => {
    // Ví dụ: Tạo một thông báo đơn giản (Bạn có thể dùng SweetAlert2 hoặc Toastr ở đây)
    console.log(`Toast: [${type}] ${message}`);

    // Tạo element thông báo (nếu bạn chưa dùng thư viện ngoài)
    const toast = document.createElement("div");
    toast.className = `custom-toast toast-${type}`;
    toast.innerText = message;

    document.body.appendChild(toast);

    setTimeout(() => {
        toast.remove();
    }, duration || 3000);
};
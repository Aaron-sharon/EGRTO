window.confirmDelete = function () {
    return Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "Cancel",
        target: '#alert-container' // 👈 Add this
    }).then((result) => {
        console.log("SweetAlert result:", result);
        return result.isConfirmed;
    });
};

window.showSuccessAlert = function () {
    Swal.fire({
        icon: 'success',
        title: 'Success!',
        text: 'Vehicle deleted successfully.',
        timer: 2000,
        showConfirmButton: false,
        target: '#alert-container' // 👈 Add this
    });
};

window.showErrorAlert = function () {
    Swal.fire({
        icon: 'error',
        title: 'Error!',
        text: 'Failed to delete the vehicle. Please try again.',
        timer: 3000,
        showConfirmButton: false,
        target: '#alert-container' // 👈 Add this
    });
};

console.log("Alert.js loaded successfully.");

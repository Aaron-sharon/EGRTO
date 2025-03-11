window.confirmDelete = function () {
    return Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "Cancel"
    }).then((result) => {
        console.log("SweetAlert result:", result); // 👈 Add this
        return result.isConfirmed;
    });
};

window.showSuccessAlert = function () {
    Swal.fire({
        icon: 'success',
        title: 'Success!',
        text: 'Vehicle deleted successfully.',
        timer: 2000,
        showConfirmButton: false
    });
};

window.showErrorAlert = function () {
    Swal.fire({
        icon: 'error',
        title: 'Error!',
        text: 'Failed to delete the vehicle. Please try again.',
        timer: 3000,
        showConfirmButton: false
    });
};

console.log("Alert.js loaded successfully.");

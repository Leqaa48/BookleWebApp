$(document).ready(function () {
    $('#myTable').DataTable({
        "scrolly": "450px",
        "scrollCollapse": true,
        "paging": true
    })
})
function formatPhoneNumber(event) {
    let input = event.target;
    // Start with '00962' and prevent any input other than digits
    if (input.value.startsWith('00962')) {
        input.value = '00962' + input.value.slice(5).replace(/\D/g, ''); // Remove non-digits
    } else {
        input.value = '00962'; // Reset to '00962' if the user deletes it
    }


function selectBarber(barberName, element) {
     // Alert to show which barber was selected (optional)
    //  alert('You selected: ' + barberName);
    // Remove 'selected' class from all barber sections
    var barbers = document.querySelectorAll('.shuffle-barber, .barber1, .barber2, .barber3, .barber4');
    barbers.forEach(function(barber) {
        barber.classList.remove('selected');
    });

    // Add 'selected' class to the parent div of the clicked <a>
    element.parentElement.classList.add('selected');

    // Update the barber's name in the right-side section
    document.getElementById('selected-barber-name').textContent = barberName;
}

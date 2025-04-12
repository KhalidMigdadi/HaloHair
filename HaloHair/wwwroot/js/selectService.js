// Filter services
function filterServices() {
    var filter = document.getElementById("service-filter").value;
    var services = document.querySelectorAll(".service");

    services.forEach(function (service) {
        if (filter === "all" || service.getAttribute("data-category") === filter) {
            service.style.display = "flex";
        } else {
            service.style.display = "none";
        }
    });
}


document.addEventListener("DOMContentLoaded", function () {
    const bookButtons = document.querySelectorAll(".book-btn");
    const selectedServicesContainer = document.querySelector(".selected-services");
    const totalPriceElement = document.getElementById("total-price");
    const totalTimeElement = document.getElementById("total-time");

    let selectedServices = new Set();
    let totalPrice = 0;
    let totalTime = 0;

    // Function to format time
    function formatTime(minutes) {
        if (minutes >= 60) {
            const hours = Math.floor(minutes / 60);
            const mins = minutes % 60;
            return `${hours} hour : ${mins} min`;
        } else {
            return `${minutes}min`;
        }
    }

    // Function to add selected services to the right box
    function addToSelectedServices(serviceName, duration, price) {
        // Check if service has already been selected
        if (selectedServices.has(serviceName)) {
            alert(`You have already selected "${serviceName}".`);
            return;
        }

        // Add service to selected set
        selectedServices.add(serviceName);

        // Create the service entry in the right-side selected services container
        const selectedService = document.createElement("div");
        selectedService.classList.add("booked-service");
        selectedService.innerHTML = `
            <div style="display: flex; justify-content: space-between; align-items: center; padding: 12px; border-bottom: 1px solid #ddd; width: 100%;">
                <div style="flex-grow: 1;">
                    <p class="service-name" style="font-weight: bold; margin: 0;">${serviceName}</p>
                    <p style="font-size: 12px; color: gray; margin: 5px 0;">${duration} mins</p>
                    <p style="margin: 0; font-weight: bold;">${price} JOD</p>
                </div>
                <button class="remove-btn" style="background: red; color: white; border: none; padding: 6px 12px; border-radius: 5px; cursor: pointer; margin-left: auto; font-size: 14px;">Remove</button>
            </div>
        `;

        selectedServicesContainer.appendChild(selectedService);

        // Update total price and total time
        totalPrice += price;
        totalTime += duration;

        totalPriceElement.innerText = totalPrice.toFixed(2);
        totalTimeElement.innerText = formatTime(totalTime);

        // Remove service when clicked
        selectedService.querySelector(".remove-btn").addEventListener("click", function () {
            selectedServicesContainer.removeChild(selectedService);
            selectedServices.delete(serviceName);
            totalPrice -= price;
            totalPriceElement.innerText = totalPrice.toFixed(2);

            totalTime -= duration;
            totalTimeElement.innerText = formatTime(totalTime);
        });
    }

    // Attach event listeners to each "Book Now" button
    bookButtons.forEach((button) => {
        button.addEventListener("click", function () {
            const serviceElement = this.closest(".service");
            const serviceName = serviceElement.querySelector("h3").innerText;
            const serviceTime = serviceElement.querySelector(".time").innerText;
            const servicePrice = parseFloat(serviceElement.querySelector("p:last-of-type").innerText.replace(" JOD", ""));

            // Extract time in minutes
            let timeInMinutes = 0;
            if (serviceTime.includes("hour")) {
                timeInMinutes = parseInt(serviceTime) * 60;
            } else if (serviceTime.includes("min")) {
                timeInMinutes = parseInt(serviceTime);
            }

            // Call the function to add service to selected
            addToSelectedServices(serviceName, timeInMinutes, servicePrice);
        });
    });

    // Handle continue button click to save the selected services
    const continueBtn = document.getElementById("continue-btn");

    continueBtn.addEventListener("click", function () {
        const services = [];

        document.querySelectorAll(".booked-service").forEach(serviceElement => {
            const name = serviceElement.querySelector(".service-name").innerText;
            const timeText = serviceElement.querySelector("p:nth-child(2)").innerText;
            const price = parseFloat(serviceElement.querySelector("p:nth-child(3)").innerText.replace(" JOD", ""));

            let duration = 0;
            if (timeText.includes("hour")) {
                const [h, m] = timeText.split(":");
                duration = parseInt(h) * 60 + parseInt(m);
            } else {
                duration = parseInt(timeText);
            }

            services.push({
                serviceName: name,
                duration: duration,
                price: price
            });
        });

        if (services.length === 0) {
            alert("Please select at least one service before continuing.");
            return;
        }

        const salonId = @Model.Id;

        fetch(`/YourController/SaveSelectedServices?salonId=${salonId}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify(services)
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    window.location.href = `/NextStep/SelectBarber?salonId=${salonId}`;
                } else {
                    alert("Something went wrong while saving your services.");
                }
            })
            .catch(err => {
                console.error(err);
                alert("An error occurred. Please try again.");
            });
    });
});


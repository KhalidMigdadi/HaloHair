// filter service

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

// add new service to the right box

// calcualte total price

document.addEventListener("DOMContentLoaded", function () {
    const bookButtons = document.querySelectorAll(".book-btn");
    const selectedServicesContainer = document.querySelector(".selected-services");
    const totalPriceElement = document.getElementById("total-price");
    const totalTimeElement = document.getElementById("total-time"); // Element to display the total time
    
    let selectedServices = new Set(); // Track booked services
    let totalPrice = 0;
    let totalTime = 0; // To keep track of total time in minutes
  
    // Function to format time
    function formatTime(minutes) {
      if (minutes >= 60) {
        const hours = Math.floor(minutes / 60);
        const mins = minutes % 60;
        return `${hours} hour : ${mins} min`; // Display hours and minutes
      } else {
        return `${minutes}min`; // Display minutes only if less than 60
      }
    }
  
    bookButtons.forEach((button) => {
      button.addEventListener("click", function () {
        const serviceElement = this.closest(".service");
        const serviceName = serviceElement.querySelector("h3").innerText;
        const serviceTime = serviceElement.querySelector(".time").innerText;
        const servicePrice = parseFloat(
          serviceElement.querySelector("p:last-of-type").innerText.replace(" JOD", "")
        );
  
        // Handle different time formats (e.g., "1 hour", "45mins")
        let timeInMinutes = 0;
        if (serviceTime.includes("hour")) {
          timeInMinutes = parseInt(serviceTime) * 60; // Convert hours to minutes
        } else if (serviceTime.includes("min")) {
          timeInMinutes = parseInt(serviceTime); // Already in minutes
        }
  
        // Check if service is already added
        if (selectedServices.has(serviceName)) {
          alert(`You have already selected "${serviceName}".`);
          return;
        }
  
        // Add to the set to prevent duplicates
        selectedServices.add(serviceName);
  
        // Create the service entry
        const selectedService = document.createElement("div");
        selectedService.classList.add("booked-service");
        selectedService.innerHTML = `
          <div style="display: flex; justify-content: space-between; align-items: center; padding: 12px; border-bottom: 1px solid #ddd; width: 100%;">
            <div style="flex-grow: 1;">
              <p class="service-name" style="font-weight: bold; margin: 0;">${serviceName}</p>
              <p style="font-size: 12px; color: gray; margin: 5px 0;">${serviceTime}</p>
              <p style="margin: 0; font-weight: bold;">${servicePrice} JOD</p>
            </div>
            <button class="remove-btn" style="background: red; color: white; border: none; padding: 6px 12px; border-radius: 5px; cursor: pointer; margin-left: auto; font-size: 14px;">Remove</button>
          </div>
        `;
  
        selectedServicesContainer.appendChild(selectedService);
  
        // Update total price
        totalPrice += servicePrice;
        totalPriceElement.innerText = totalPrice.toFixed(2);
  
        // Update total time
        totalTime += timeInMinutes;
        totalTimeElement.innerText = formatTime(totalTime); // Display formatted time
  
        // Remove service functionality
        selectedService.querySelector(".remove-btn").addEventListener("click", function () {
          selectedServicesContainer.removeChild(selectedService);
          selectedServices.delete(serviceName); // Remove from the set
          totalPrice -= servicePrice;
          totalPriceElement.innerText = totalPrice.toFixed(2);
  
          // Decrease total time
          totalTime -= timeInMinutes;
          totalTimeElement.innerText = formatTime(totalTime); // Display formatted time
        });
      });
    });
  });
  
  
  

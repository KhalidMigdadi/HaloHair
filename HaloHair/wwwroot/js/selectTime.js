document.querySelector(".dropdown-btn").addEventListener("click", function () {
  document.getElementById("dropdown-list").classList.toggle("show");
});

function selectBarber(name, imageSrc) {
  document.getElementById("selected-barber").querySelector("span").innerText =
    name;
  document.getElementById("barber-img").src = imageSrc;
  document.getElementById("dropdown-list").classList.remove("show");

  // ✅ Update the right-side box with the selected barber
  document.getElementById("selected-barber-name").innerText = name;
}

// Close dropdown when clicking outside
window.onclick = function (event) {
  if (!event.target.closest(".custom-dropdown")) {
    document.getElementById("dropdown-list").classList.remove("show");
  }
};

// Show the available time slots when a date is selected
document.addEventListener("DOMContentLoaded", function () {
  const dateInput = document.getElementById("date-select");
  const timeSlotsContainer = document.getElementById("time-slots");

  // Set min date (today) for selection
  let today = new Date().toISOString().split("T")[0];
  dateInput.setAttribute("min", today);

  // Function to generate available time slots
  function generateTimeSlots() {
    const availableSlots = [
      "09:00 AM",
      "10:00 AM",
      "11:00 AM",
      "12:00 PM",
      "01:00 PM",
      "02:00 PM",
      "03:00 PM",
      "04:00 PM",
    ];

    timeSlotsContainer.innerHTML = ""; // Clear previous slots

    availableSlots.forEach((time) => {
      const slot = document.createElement("div");
      slot.classList.add("time-slot");
      slot.innerText = time;
      slot.addEventListener("click", function () {
        document
          .querySelectorAll(".time-slot")
          .forEach((el) => el.classList.remove("selected"));
        slot.classList.add("selected");

        // ✅ Update the right-side box with the selected time
        document.getElementById("selected-time").innerText = time;
      });
      timeSlotsContainer.appendChild(slot);
    });
  }

  // ✅ Update the right-side box when a date is selected
  dateInput.addEventListener("change", function () {
    generateTimeSlots();
    document.getElementById("selected-date").innerText = dateInput.value;
  });
});


// filter service

function filterServices() {
    var filter = document.getElementById("service-filter").value;
    var services = document.querySelectorAll(".service");

    services.forEach(function(service) {
        if (filter === "all" || service.getAttribute("data-category") === filter) {
            service.style.display = "flex";
        } else {
            service.style.display = "none";
        }
    });
}


// Arrow for nex and previous 
// Arrow navigation functionality with dynamic visibility for the previous button
document.querySelectorAll(".container-withArrow").forEach((container) => {
    const prevBtn = container.querySelector(".prev");
    const nextBtn = container.querySelector(".next");
    const scrollable = container.querySelector(".arrows-Home");
  
    const scrollAmount = 300; 
  
    if (prevBtn && nextBtn && scrollable) {
      prevBtn.style.visibility = "hidden";
  
      prevBtn.addEventListener("click", () => {
        scrollable.scrollBy({
          left: -scrollAmount,
          behavior: "smooth",
        });
  
        nextBtn.style.visibility = "visible";
  
        if (scrollable.scrollLeft <= scrollAmount) {
          prevBtn.style.visibility = "hidden";
        }
      });
  
      nextBtn.addEventListener("click", () => {
        scrollable.scrollBy({
          left: scrollAmount,
          behavior: "smooth",
        });
  
        prevBtn.style.visibility = "visible";
  
        const maxScrollLeft = scrollable.scrollWidth - scrollable.clientWidth;
        if (scrollable.scrollLeft + scrollAmount >= maxScrollLeft) {
          nextBtn.style.visibility = "hidden";
        }
      });
  
      scrollable.addEventListener("scroll", () => {
        const maxScrollLeft = scrollable.scrollWidth - scrollable.clientWidth;
  
        if (scrollable.scrollLeft > 0) {
          prevBtn.style.visibility = "visible";
        } else {
          prevBtn.style.visibility = "hidden";
        }
  
        if (scrollable.scrollLeft < maxScrollLeft) {
          nextBtn.style.visibility = "visible";
        } else {
          nextBtn.style.visibility = "hidden";
        }
      });
    }
  });
  
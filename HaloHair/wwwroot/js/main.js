const images = document.querySelectorAll(".slider-image");
let currentIndex = 0;


function changeSlide() {
  images[currentIndex].classList.remove("active");
  currentIndex = (currentIndex + 1) % images.length;
  images[currentIndex].classList.add("active");
}

setInterval(changeSlide, 4000); // Change image every 3 seconds



// When the page is scrolled
window.onscroll = function() { changeNavbarColor() };

function changeNavbarColor() {
  var navbar = document.getElementById("navbar");
  
  // If the page is scrolled down more than 100px, change the navbar background to black
  if (document.body.scrollTop > 500 || document.documentElement.scrollTop > 500) {
    navbar.classList.add("navbar-scroll"); // Add the background color class
  } else {
    navbar.classList.remove("navbar-scroll"); // Remove the background color class
  }
}


// slide bar customer

const slider = document.querySelector(".testimonial-slider");
const prevBtn = document.getElementById("prevBtn");
const nextBtn = document.getElementById("nextBtn");

let scrollAmount = 0;
const cardWidth = document.querySelector(".testimonial-card").offsetWidth + 20; // 20px gap

nextBtn.addEventListener("click", () => {
  slider.scrollBy({ left: cardWidth * 2, behavior: "smooth" });
});

prevBtn.addEventListener("click", () => {
  slider.scrollBy({ left: -cardWidth * 2, behavior: "smooth" });
});






function filterSalons(category) {
    document.querySelectorAll(".salon-card").forEach((card) => {
      if (category === "all" || card.classList.contains(category)) {
        card.style.display = "block";
      } else {
        card.style.display = "none";
      }
    });
  
    document.querySelectorAll(".filter-buttons button").forEach((btn) => {
      btn.classList.remove("active");
    });
  
    document.querySelector(`button[onclick="filterSalons('${category}')"]`).classList.add("active");
  }
  
  filterSalons("all"); // Show all salons by default
  
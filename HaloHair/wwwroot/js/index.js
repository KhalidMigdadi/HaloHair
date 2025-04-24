// -------------------- Slider Change --------------------
const images = document.querySelectorAll(".slider-image");
let currentIndex = 0;

function changeSlide() {
    images[currentIndex].classList.remove("active");
    currentIndex = (currentIndex + 1) % images.length;
    images[currentIndex].classList.add("active");
}
setInterval(changeSlide, 4000); // Change image every 4 seconds

// -------------------- Navbar Color on Scroll --------------------
window.onscroll = function () { changeNavbarColor() };

function changeNavbarColor() {
    const navbar = document.getElementById("navbar");

    if (document.body.scrollTop > 500 || document.documentElement.scrollTop > 500) {
        navbar.classList.add("navbar-scroll");
    } else {
        navbar.classList.remove("navbar-scroll");
    }
}

// -------------------- Testimonials Slider (Cards) --------------------
const testimonialSlider = document.querySelector(".testimonial-slider");
const prevTestimonialBtn = document.getElementById("prevBtn");
const nextTestimonialBtn = document.getElementById("nextBtn");

if (testimonialSlider && prevTestimonialBtn && nextTestimonialBtn) {
    const cardWidth = document.querySelector(".testimonial-card").offsetWidth + 20;

    nextTestimonialBtn.addEventListener("click", () => {
        testimonialSlider.scrollBy({ left: cardWidth * 2, behavior: "smooth" });
    });

    prevTestimonialBtn.addEventListener("click", () => {
        testimonialSlider.scrollBy({ left: -cardWidth * 2, behavior: "smooth" });
    });
}

// -------------------- Salon Filter --------------------
function filterSalons(category) {
    document.querySelectorAll(".salon-card").forEach((card) => {
        card.style.display = (category === "all" || card.classList.contains(category)) ? "block" : "none";
    });

    document.querySelectorAll(".filter-buttons button").forEach((btn) => {
        btn.classList.remove("active");
    });

    document.querySelector(`button[onclick="filterSalons('${category}')"]`)?.classList.add("active");
}
filterSalons("all");

// -------------------- Home Page Search Form --------------------
document.getElementById('searchButton-home')?.addEventListener('click', () => {
    const location = document.getElementById('location-home')?.value;
    const service = document.getElementById('service-home')?.value;
    const date = document.getElementById('date-home')?.value;
    const time = document.getElementById('time-slot')?.value;

    if (location && service && date && time) {
        console.log(`Searching for ${service} in ${location} on ${date} at ${time}`);
    } else {
        alert('Please fill out all fields.');
    }
});

// -------------------- Home Page Arrow Navigation --------------------
document.querySelectorAll(".container-withArrow").forEach((container) => {
    const prevBtn = container.querySelector(".prev");
    const nextBtn = container.querySelector(".next");
    const scrollable = container.querySelector(".arrows-Home");

    const scrollAmount = 300;

    if (prevBtn && nextBtn && scrollable) {
        prevBtn.style.visibility = "hidden";

        prevBtn.addEventListener("click", () => {
            scrollable.scrollBy({ left: -scrollAmount, behavior: "smooth" });
            nextBtn.style.visibility = "visible";

            if (scrollable.scrollLeft <= scrollAmount) {
                prevBtn.style.visibility = "hidden";
            }
        });

        nextBtn.addEventListener("click", () => {
            scrollable.scrollBy({ left: scrollAmount, behavior: "smooth" });
            prevBtn.style.visibility = "visible";

            const maxScrollLeft = scrollable.scrollWidth - scrollable.clientWidth;
            if (scrollable.scrollLeft + scrollAmount >= maxScrollLeft) {
                nextBtn.style.visibility = "hidden";
            }
        });

        scrollable.addEventListener("scroll", () => {
            const maxScrollLeft = scrollable.scrollWidth - scrollable.clientWidth;

            prevBtn.style.visibility = scrollable.scrollLeft > 0 ? "visible" : "hidden";
            nextBtn.style.visibility = scrollable.scrollLeft < maxScrollLeft ? "visible" : "hidden";
        });
    }
});

const images = document.querySelectorAll(".slider-image");
let currentIndex = 0;


function changeSlide() {
    images[currentIndex].classList.remove("active");
    currentIndex = (currentIndex + 1) % images.length;
    images[currentIndex].classList.add("active");
}

setInterval(changeSlide, 4000); // Change image every 3 seconds




// When the page is scrolled
window.onscroll = function () { changeNavbarColor() };

function changeNavbarColor() {
    var navbar = document.getElementById("navbar");

    // If the page is scrolled down more than 100px, change the navbar background to black
    if (document.body.scrollTop > 500 || document.documentElement.scrollTop > 500) {
        navbar.classList.add("navbar-scroll"); // Add the background color class
    } else {
        navbar.classList.remove("navbar-scroll"); // Remove the background color class
    }
}


// Search form

document.getElementById('searchButton-home').addEventListener('click', () => {

    const location = document.getElementById('location-home').value;
    const service = document.getElementById('service-home').value;
    const date = document.getElementById('date-home').value;
    const time = document.getElementById('time-slot').value;

    if (location && service && date && time) {
        // Simulate a search request (replace with actual API call)
        console.log(`Searching for ${service} in ${location} on ${date} at ${time}`);
    } else {
        alert('Please fill out all fields.');
    }
});





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



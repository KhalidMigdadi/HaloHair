// Built by @traf
// https://tr.af

$(window).scroll(function() {
    var scroll = $(window).scrollTop();
    $(".hero").css({
      transform: 'translate3d(0, +'+(scroll/100)+'%, 0) scale('+(100 - scroll/100)/100+')'
    });
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
  
  
  
  
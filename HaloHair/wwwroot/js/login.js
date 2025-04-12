let emailInput = document.querySelector('input[type="email"]');
let passwordInput = document.querySelector('input[type="password"]');
let loginForm = document.getElementById("loginForm");

let emailRegx = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
let emailError = document.getElementById("emailCheck");

emailInput.addEventListener("input", function () {
  if (!emailInput.value.match(emailRegx)) {
    emailError.innerHTML = "Enter Valid Email";
  } else {
    emailError.innerHTML = "";
  }
});

loginForm.addEventListener("submit", (event) => {
  event.preventDefault();
  let existingUsers = JSON.parse(localStorage.getItem("users")) || [];

  const user = existingUsers.find(
    (user) =>
      user.email === emailInput.value.trim() &&
      user.password === passwordInput.value
  );

  if (user) {
    sessionStorage.setItem("LoggedUser", JSON.stringify(user));
    alert("Login successful! Redirecting...");
    window.location.href = "index.html";
  } else {
    alert("Invalid username or password. Please try again.");
  }
});

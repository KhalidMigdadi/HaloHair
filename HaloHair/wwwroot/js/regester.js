let regestrationForm = document.getElementById("regestrationForm");
let lNameInput = document.getElementById("lNameInput");


// handle form submition
regestrationForm.addEventListener("submit", (event) => {
  event.preventDefault();

  // check about all input are valid
  if (
    fNameRegx.test(fNameInput.value) &&
    emailRegx.test(emailInput.value) &&
    confirmPasswordInput.value == passwordInput.value
  ) {
    // Retrieve or get existing user data from localStorage
    const existingUsers = JSON.parse(localStorage.getItem("users")) || [];

    // Check if a user with the same email already exists

    // declare the user
    const userExist = existingUsers.some(
      (user) => user.email === emailInput.value
    );

    if (userExist) {
      alert("User already exists!");
    } else {
      // Create the new user object
      const newUser = {
        fName: fNameInput.value, // First name
        lName: lNameInput.value, // Last name
        email: emailInput.value, // Email
        password: passwordInput.value, // Password
      };

      // Add the new user to the stored data
      existingUsers.push(newUser);

      // Save updated user data to localStorage
      localStorage.setItem("users", JSON.stringify(existingUsers));

      // Save the new user's data to sessionStorage for the current session
      sessionStorage.setItem("loggedUser", JSON.stringify(newUser));

      alert("Successfully Rejecstraion, Redirect to login page...");
      window.location.href = "./loginB.html";
    }
  } else {
    alert("Please fill all the filed in the form.");
  }
});

// regex first name

let fNameInput = document.querySelector('input[type="text"]');
let fNameRegx = /^[a-zA-Z]+$/;
let fNameError = document.getElementById("fNameCheck");

fNameInput.addEventListener("input", function () {
  if (!fNameInput.value.match(fNameRegx)) {
    fNameError.innerHTML = "The First Name Should Be from letters";
  } else fNameError.innerHTML = "";
});

// The match method is used for matching a string with a regular expression

// regex for email

let emailInput = document.querySelector("input[type=email]");
let emailRegx = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
let emailError = document.getElementById("emailCheck");

emailInput.addEventListener("input", function () {
  if (!emailInput.value.match(emailRegx)) {
    emailError.innerHTML = "Enter Valid Email";
  } else emailError.innerHTML = "";
});

// regex for passwrod

let Password = document.querySelector('input[type="password"]');
let passwordRegx =
  /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
let passwordError = document.getElementById("passwordCheck");

Password.addEventListener("input", function () {
  if (!Password.value.match(passwordRegx)) {
    passwordError.innerHTML =
      "The password must contain a symbole, a cpaital and small letters and its must be 8 characters ";
  } else passwordError.innerHTML = "";
});

// chcek if the password match the confirm one

let passwordInput = document.getElementById("passwordInput");
let confirmPasswordInput = document.getElementById("confirmPassword");
let passwordConfirm = document.getElementById("passwordConfirm");

confirmPasswordInput.addEventListener("input", () => {
  if (passwordInput.value == confirmPasswordInput.value) {
    passwordConfirm.innerHTML = "";
  } else passwordConfirm.innerHTML = "The password didint match";
});
















// here another solution for the sort data in local storage but not inside object

// check if the email is exist or not and if not save it in local storage

// let create = document.getElementById("create"); // button

// create.addEventListener("click", function (event) {
//   event.preventDefault(); // Prevents page reload

//   // we must to declare them inside function to store the value in local storage not the key only
//   let userEmail = document.querySelector('input[type="email"]').value.trim(); // Get current email input and tirm to avoid spaces
//   let existingEmails = JSON.parse(localStorage.getItem("usersEmails") || "[]"); // the key in local storage is usersEmails
//   let password = document.querySelector('input[type="password"]').value; // get the password

//   if (!userEmail) {
//     // to prevent user from enter a empty or invalid email
//     alert("Please enter a valid email.");
//     return;
//   }

//   if (!existingEmails.includes(userEmail)) {
//     // if the emails in local storage didint include the user email that he input now
//     existingEmails.push(userEmail); // Add new email to the array   // we need to push the userEmail to the usersEmails
//     // the key (usersEmails) will appeare in local storage
//     localStorage.setItem("usersEmails", JSON.stringify(existingEmails)); // Store updated array in local storage
//     localStorage.setItem("Password", JSON.stringify(password));
//     alert("Email and password are Saved");
//   } else {
//     alert("Email Is Already Exist");
//   }

//   window.location.href = "loginB.html";
// });

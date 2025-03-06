CREATE DATABASE SalonBookingDB;

CREATE TABLE Users (
    ID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    Gender CHAR(1) CHECK (Gender IN ('M', 'F')) NOT NULL, 
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastLogin DATETIME DEFAULT NULL
);

CREATE TABLE Barber (
    ID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL, 
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    Gender CHAR(1) CHECK (Gender IN ('M', 'F')) NOT NULL, 
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastLogin DATETIME DEFAULT NULL,
	SalonID INT NOT NULL,
	FOREIGN KEY (SalonID) REFERENCES Salon(ID) 

);

CREATE TABLE SuperAdmins (
    ID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL, 
    Gender CHAR(1) CHECK (Gender IN ('M', 'F')) NOT NULL, 
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastLogin DATETIME NULL, -- To track last login time
    Permissions NVARCHAR(255) DEFAULT 'Full Access' -- Permissions could be more specific if needed
);

-- Each Salon Location
CREATE TABLE Salon (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL, -- Branch name
    Address NVARCHAR(255) NOT NULL, -- Full address
    PhoneNumber NVARCHAR(20) NOT NULL, -- Contact number
    OpeningTime TIME NOT NULL, -- Opening hour
    ClosingTime TIME NOT NULL, -- Closing hour
    SuperAdminID INT NULL, -- Each salon has a manager (SuperAdmin)
    FOREIGN KEY (SuperAdminID) REFERENCES SuperAdmins(ID)
);



-- services for salons 
CREATE TABLE Services (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL, -- Example: Haircut, Shaving, Beard Trim
    Description NVARCHAR(255) NULL, 
    DurationInMinutes INT NOT NULL -- How long the service takes
);


CREATE TABLE Appointments (
    ID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL, -- Foreign key to Users table
    BarberID INT NOT NULL, -- Foreign key to Admin table (for barber)
    SalonID INT NOT NULL,
    ServiceID INT NOT NULL, 
    AppointmentDate DATETIME NOT NULL, -- Date and time of the appointment
    Status NVARCHAR(50) DEFAULT 'Pending', -- Status of the appointment (e.g., Pending, Completed, Cancelled)
    CreatedAt DATETIME DEFAULT GETDATE(), -- When the appointment was made
    FOREIGN KEY (ServiceID) REFERENCES Services(ID),
    FOREIGN KEY (UserID) REFERENCES Users(ID),
    FOREIGN KEY (BarberID) REFERENCES Barber(ID),
    FOREIGN KEY (SalonID) REFERENCES Salon(ID)
);


-- Link Services to Salons
CREATE TABLE SalonServices (
    ID INT PRIMARY KEY IDENTITY(1,1),
    SalonID INT NOT NULL, -- Which salon offers this service
    ServiceID INT NOT NULL, -- Which service is offered
    Price DECIMAL(10,2) NOT NULL, -- Service price per salon
    FOREIGN KEY (SalonID) REFERENCES Salon(ID),
    FOREIGN KEY (ServiceID) REFERENCES Services(ID)
);

-- Link Barbers to the Services They Provide
CREATE TABLE BarberServices (
    ID INT PRIMARY KEY IDENTITY(1,1),
    BarberID INT NOT NULL,
    SalonServiceID INT NOT NULL, -- Service that the barber can perform
    FOREIGN KEY (BarberID) REFERENCES Barber(ID),
    FOREIGN KEY (SalonServiceID) REFERENCES SalonServices(ID)
);

CREATE TABLE Notifications (
    ID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Message NVARCHAR(255) NOT NULL,
    IsRead BIT DEFAULT 0, -- If the notification is read or not
    CreatedAt DATETIME DEFAULT GETDATE(), -- When the notification was created
    FOREIGN KEY (UserID) REFERENCES Users(ID)
);


CREATE TABLE PasswordResets (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) NOT NULL, -- Email for password reset
    Token NVARCHAR(255) NOT NULL, -- Unique token for password reset
    ExpiryDate DATETIME NOT NULL, -- When the token expires
    CreatedAt DATETIME DEFAULT GETDATE() -- When the reset request was made
);

CREATE TABLE BarberAvailability (
    ID INT PRIMARY KEY IDENTITY(1,1),
    BarberID INT NOT NULL, -- Foreign key to Admin table
    DayOfWeek NVARCHAR(10) NOT NULL, -- E.g., Monday, Tuesday, etc.
    StartTime TIME NOT NULL, -- When the barber starts working
    EndTime TIME NOT NULL, -- When the barber stops working
    FOREIGN KEY (BarberID) REFERENCES Barber(ID)
);

CREATE TABLE SubscriptionPlans (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL, -- E.g., "Gold Membership", "Silver Membership"
    Price DECIMAL(10,2) NOT NULL, -- Subscription fee
    DurationInMonths INT NOT NULL, -- Duration in months
    Benefits NVARCHAR(255) NULL, -- Benefits included in the plan
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- For how users subscribe 
CREATE TABLE UserSubscriptions (
    ID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL, -- Foreign key to Users table
    PlanID INT NOT NULL, -- Foreign key to SubscriptionPlans table
    StartDate DATETIME DEFAULT GETDATE(), -- When the subscription starts
    EndDate DATETIME NOT NULL, -- When the subscription expires
    Status NVARCHAR(50) DEFAULT 'Active', -- Status (Active, Expired, Cancelled)
    FOREIGN KEY (UserID) REFERENCES Users(ID),
    FOREIGN KEY (PlanID) REFERENCES SubscriptionPlans(ID)
);

CREATE TABLE Coupons (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(50) UNIQUE NOT NULL, -- Coupon code (e.g., "DISCOUNT20")
    DiscountPercentage INT CHECK (DiscountPercentage BETWEEN 1 AND 100), -- Discount percentage
    ExpiryDate DATETIME NOT NULL, -- Expiration date of the coupon
    IsActive BIT DEFAULT 1 -- Whether the coupon is active
);


CREATE TABLE Payments (
    ID INT PRIMARY KEY IDENTITY(1,1),
    AppointmentID INT NOT NULL,
    CouponID INT NULL, -- Nullable because not every payment has a coupon
    Amount DECIMAL(10, 2) NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Pending', -- Payment status (e.g., Completed, Failed)
    CreatedAt DATETIME DEFAULT GETDATE(), -- When the payment was made
    FOREIGN KEY (AppointmentID) REFERENCES Appointments(ID),
    FOREIGN KEY (CouponID) REFERENCES Coupons(ID)
);


-- The reason for cancellation
CREATE TABLE BookingCancellations (
    ID INT PRIMARY KEY IDENTITY(1,1),
    AppointmentID INT NOT NULL, -- Foreign key to Appointments table
    CancelledBy NVARCHAR(50) NOT NULL, -- Either "User" or "Admin"
    CancelledByUserID INT NULL, -- Store the user who canceled
    Reason NVARCHAR(255) NULL, -- Reason for cancellation
    CancelledAt DATETIME DEFAULT GETDATE(), -- When it was cancelled
    FOREIGN KEY (CancelledByUserID) REFERENCES Users(ID),
    FOREIGN KEY (AppointmentID) REFERENCES Appointments(ID)
);



--Users (1) ↔ (M) Appointments
--Users (1) ↔ (M) Notifications
--Users (1) ↔ (M) UserSubscriptions

--Users (1) ↔ (M) BookingCancellations

--Barbers (1) ↔ (M) Appointments
--Barbers (1) ↔ (M) BarberAvailability
--Barbers (1) ↔ (M) BarberServices

--Salon (1) ↔ (M) Appointments
--Salon (1) ↔ (M) SalonServices
--Salon (1) ↔ (M) Barbers

--SuperAdmins (1) ↔ (M) Salon

--Appointments (1) ↔ (1) Payments 
--Appointments (1) ↔ (M) BookingCancellations can canel the appoimtmtns one time or more 

--Services (1) ↔ (M) SalonServices
--SalonServices (1) ↔ (M) BarberServices each services can do it by many barbers

--Subscriptions (1) ↔ (M) UserSubscriptions

--Coupons (1) ↔ (M) Payments



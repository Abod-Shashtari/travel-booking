# Travel and Accommodation Booking Platform
This project is a backend API for a hotel and room booking platform, built using .NET Core with Clean Architecture.
It allows users to search, browse, review, and book hotels, with secure online payments via Stripe.
Admins can manage cities, hotels, rooms, and discounts through a dedicated admin panel.
The API follows RESTful principles and includes robust authentication, authorization, and testing practices.

## Setup Guid
#### 1. Clone the Repository
```
git clone https://github.com/Abod-Shashtari/travel-booking.git
cd travel-booking
```
#### 2. Set API Keys and Connection Strings
Create a `.env` file in the project root (next to `compose.yml`):
```
# Sentry Configuration
SENTRY_DSN=your-sentry-dsn

# Stripe Configuration
STRIPE_WEBHOOK_SECRET=whsec_your_stripe_webhook_secret_key
STRIPE_SECRET_KEY=your_stripe_secret_key

# SMTP Configuration
SMTP_USERNAME=your_smtp_username
SMTP_PASSWORD=your_smtp_password

# JWT Configuration
JWT_KEY=your_jwt_secret_key_at_least_32_characters_long

# Database Configuration
SQL_SA_PASSWORD=YourStrongPassword123!
CONNECTION_STRING=Server=sqlserver,1433;Database=TravelBookingDB;User ID=sa;Password=YourStrongPassword123!;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;

# Cloudinary Configuration
CLOUDINARY_API_SECRET=your_cloudinary_api_secret
CLOUDINARY_API_KEY=your_cloudinary_api_key
```
#### 3.Run the Project (Dev Environment)
```
docker compose up --build
```

#### 4. Access the API
Open your browser or API tool (like .http file or Postman) and go to:
```
http://localhost:8080
```

## Database Diagram
![DB-Diagram](https://github.com/user-attachments/assets/ac176eea-5637-418f-a7f3-feb43034e5c6)

## Endpoints
You can explore all available API endpoints using the Swagger UI:
[API Docs](https://abod-shashtari.github.io/travel-booking/)

## Project Structure
This project follows Clean Architecture with bunch of design patterns.
this project consists of 4 layers:
- Domain: Contains all entities, custome errors, interfaces. 
- Application: Implements business logic using CQRS (Commands/Queries).
- Infrastructure: Has Repositories for data access purpose and external services.
- Web/API: This is the presentaion layer that contains the API controllers to handel http requests. 

## Design Decisions
- Error Handling: For error handling, I used two methods: the Result pattern for expected/known errors, and middleware for global exception handling used for unexpected exceptions like 500 errors.
- Repository Pattern: For data access, I used the Repository pattern with a global interface for most repositories because most of them are similar to each other with focusing on CRUD operations. And it helps for simplifying the integration testing.
- JWT Token Management: Storing logged-in users' JWT tokens in a table called WhiteList is useful for sign-out functionality, as it allows the app to revoke tokens at any time.
- Image Uploading: I created one reusable image entity/table that can handle images for any entity that has access for uploading images. this for reusablity porpuses.
- Payments: For Payment I used Stripe service.

## API Deployment
The API is deployed on Azure Container Apps, As it simplify deployment while offering scalability and native support for Docker containers.
The docker image is published on Azure Container Reigistry which integrates well with .NET and Azure services.
You can access the deployed API at: [https://travel-booking-api.wonderfulbeach-0b104421.westeurope.azurecontainerapps.io](https://travel-booking-api.wonderfulbeach-0b104421.westeurope.azurecontainerapps.io)

## Testing the API
#### You can test the API using:
- [Postman Collection](https://www.postman.com/abodshashtari/public-apis/collection/c3kutxp/travel-and-accommodation-booking-platform-api?action=share&creator=29451978)
- `.http` files located in `TravelBooking.Web/API/.http/`

#### To Test Payment:
- Get Stripe `clientSecret` through POST `api/payment/bookings/{bookingId}/create-payment-intent`
- In front-end that uses stripe set the clientSecret and fill the payment detials (For testing you can use `4242 4242 4242 4242` card number with any future date and any CVC )
- If charge is succeeded then stipe will request the webhook that will confirm the hotel booking and send email to the user.  

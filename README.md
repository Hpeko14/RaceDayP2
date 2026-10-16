# RaceDayAPI

ASP.NET Core Web API (.NET 8) for managing race events, their categories, participant enrollments and race results.
It uses Entity Framework Core (Code-First) on SQL Server, with session-based authentication and two roles: **Organiser** and **Participant**.

## Features

- Organisers create and manage events, categories and results.
- Participants enrol themselves in categories and can cancel or move their own enrollments.
- Passwords are stored hashed.
- Role-based access is enforced with a `RoleAuthorize` attribute and the session.
- Swagger documents and tests every endpoint.

## Database

The database is called `RaceDay` and has six tables:

| Table | Purpose | Key columns |
|---|---|---|
| Users | Registered users | UserID, Name, Email, Password, Role |
| Events | Race events | EventID, Title, Date, Location, OrganiserID |
| Categories | Race categories within an event | CategoryID, EventID, Name, Distance |
| Enrollments | A user's entry into a category | EnrollmentID, UserID, CategoryID, RegistrationDate |
| Results | Finish time and position for an enrollment | ResultID, EnrollmentID, FinishTime, Position |
| Routes | Map and weather details for an event | RouteID, EventID, MapURL, WeatherInfo |

Relationships: an organiser (User) has many Events, an Event has many Categories and Routes, a User enrols in a Category, and an Enrollment has a Result.

## Getting started

### Requirements

- .NET 8 SDK
- SQL Server or SQL Server LocalDB (included with Visual Studio)

### Setup

1. Clone the repository.
2. Check the `RaceDay` connection string in `appsettings.Development.json`. The default uses LocalDB:

       Server=(localdb)\MSSQLLocalDB;Database=RaceDay;Trusted_Connection=True;

3. Create the database from the EF Core migrations. In Visual Studio's Package Manager Console run `Update-Database`, or run in a terminal:

       dotnet ef database update

   The app also applies migrations automatically when it starts.
4. Run the project:

       dotnet run

5. Open `/swagger` in your browser to explore and try the endpoints.

## Endpoints

| Controller | Routes | Access |
|---|---|---|
| Category | `POST api/Category/create`, `PUT api/Category/update/{id}`, `DELETE api/Category/delete/{id}` | Organiser |
| Category | `GET api/Category/all` | Anyone |
| Event | `POST api/Event/create`, `PUT api/Event/update/{id}`, `DELETE api/Event/delete/{id}` | Organiser |
| Event | `GET api/Event/all`, `GET api/Event/{id}` | Anyone |
| Enrollment | `POST api/Enrollment/enroll`, `PUT api/Enrollment/update/{id}`, `DELETE api/Enrollment/delete/{id}` | Participant (own enrollments) or Organiser |
| Enrollment | `GET api/Enrollment/all` | Anyone |
| Results | `POST api/Results/record`, `PUT api/Results/update/{id}`, `DELETE api/Results/delete/{id}` | Organiser |
| Results | `GET api/Results/all`, `GET api/Results/event/{eventId}` | Anyone |

## Business rules

- A category must belong to an existing event and have a distance greater than 0.
- A user cannot enrol in the same category twice.
- A participant can only enrol, move or cancel their own enrollment. An organiser can do this for anyone.
- An enrollment that already has a result cannot be cancelled.
- A result needs an existing enrollment and a position of 1 or more, and each enrollment can have only one result.
- A category with enrollments, or an event with categories or routes, cannot be deleted.

## Continuous integration

A GitHub Actions workflow (`.github/workflows/dotnet-ci.yml`) restores and builds the project on every push and pull request to `main`.

## Technologies

C#, ASP.NET Core Web API (.NET 8), Entity Framework Core 8, SQL Server, Swagger (Swashbuckle), GitHub Actions# RaceDayAPI

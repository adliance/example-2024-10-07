# ASP.NET Core MVC Example
This is a small example application built with ASP.NET Core MVC. It uses EF Core as O/R mapper (incl. code-first migrations) with SQL Server for a database backend.
We tried to stick quite closely to Microsoft's example applications, so the directory and code structure should be more or less familiar.

## Requirements
- .NET 7 SDK.
- SQL Server running locally (LocalDB is sufficient).
- Every OS supported by .NET should work fine; we tested it on Windows and macOS.
- The included `.sln` solution fine works in Visual Studio and Rider; but no IDE needed for this example, you can use any text editor if you like.

## Steps to run
- Checkout code
- Run `dotnet run` from the `src/Example.Web` directory to start the app locally.
- Run `dotnet test` from the `test/Example.Web.Tests` directory to run the unit/integration tests.

## Your task
Our app provides a simple registration form that allows a user to enter first and last name and those will be stored to the database. Nothing more ;)

Now we want you to extend this example, so that every user can only register once. For this, we ask the user for his/her e-mail address and we store the e-mail address into the database as well. 
But we don't want to store the e-mail address in clear text due to data privacy reasons, as we only need it to check for duplicates. Therefore, we only want to store a strong hash into the database.

Please follow these rough outline:
1. Extend the `Registrations` table with columns `EmailHash` and `EmailHashSalt`. Use a EF Core code-first migration.
2. Extend the registration form with a new e-mail input field.
3. Check if the entered e-mail already exists in the database. If not, save the registration as before. If it already exists, show a nice error message to the user. Don't forget to only store a hashed e-mail address. 
   You can either use .NET standard functionality for hashing, or you can use a small helper library we wrote (https://www.nuget.org/packages/Adliance.Buddy.Crypto and https://github.com/adliance/Buddy/tree/master/src/Adliance.Buddy.Crypto).
4. Extend the unit/integration test suite to fully test the new functionality.
5. Create a pull request for us to review and merge your changes into the existing codebase.

Tips:
1. Try to follow the existing code style and structure. There are many different ways to add the new requirements, we're looking for one that fits best to the existing code base.
2. We always prefer readable code over "cool" code. Writing code is easy, reading code is hard - please try to make your changes nice to read and understand.

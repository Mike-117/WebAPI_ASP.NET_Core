# RESTful WebAPI with ASP.NET Core

This is a WebAPI which provides the logic for a movie theatre website. I built it with ASP.NET Core MVC, using C# and SQL Server. It employs Entity Framework and code-first migrations, as well as performing all CRUD operations on the data.

My previous WebAPI focused on the front end, using Angular 10. This one focuses on the back end, and has no front end styling or functionality. There are three controllers, one for movies, another for reservations, and the third for application users, both ticket buyers and cinema administrators. The program creates three databases, using a one-to-many relationship between them.

This api handles exceptions and provides Http status codes for RESTful operations. Model validation, attribute routing, and paging are incorporated into the program, as well as basis search functionality. I used JSON Web Tokens for authentication and authorization.

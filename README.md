# SimpleCRUD

# Steps to install and run this simple ASP .Net Core Web API CRUD (Create, Read, Update, Delete) operation application

#	1.	Clone the application into your local environment

#	2.	Changed the connection string to your own database

#	3.	Open the package manager console in the visual studio, and type these command (to create the table in your database), line by line.
#			Add-Migration -Context -DataContext
#			Enable-Migrations -ContextTypeName DataContext
#			Update-Database -Verbose

#	4.	Once the above commands had been run, a new database table (as you setup in connection string step 2 above) will be created. 
#		The table consist of column Id, username, Email, PhoneNumber, Skillsets, Hobby, Status, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate

#	5.	Run the application

#	6.	This ASP .NET Core Web API we can beused and connected by another front-end application (such as ASP .Net Core MVC), for the front-end CRUD operations.

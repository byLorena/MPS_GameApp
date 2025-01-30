In order for the project to run, you will to install these 2 libraries using NuGet Package Console:

Install-Package Microsoft.EntityFrameworkCore.Design -v 7.0.5
Install-Package Microsoft.EntityFrameworkCore.SqlServer -v 7.0.5

and then you need to use the command Update-Database with the same console.

On first build of the project faced 2 issues, I needed to fix:

1. If you run into a problem "File Paths Over 260 Characters" on first build of project. To resolve it go to regedit.exe, 
   find the key HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem\LongPathsEnabled and change the value from 0 to 1
   and restart computer.

2. Fix failing tests by returning NotFound(); status code when there is no records for given id.


Improvements to the project:

1. Refactor all the code to separate it to the layers, for example all the database calls and models should be in DAL layer.
2. Remove all the business logic from the controller to the Service layer.
3. Use real db instead of one in memory.
4. Use EntityFrameworkCore for easier and more readable data access code.
5. Use AutoMapper to Map all the DBModels to response Models

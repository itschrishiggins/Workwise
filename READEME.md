# Workwise - Coworking Space Booking App

A WPF application that allows users to browse and reserve available office workspaces and view existing bookings.

## Features
- Browse available workspaces
- Make reservations for office spaces
- View existing bookings

## Requirements
- **.NET Framework 4.x** (Ensure your system has the required version installed)
- **SQL Server Express or SQL Server** (for database management)
- **Entity Framework** (Ensure the required EF dependencies are installed)

## Setup Instructions
### 1. Clone the Repository
```sh
 git clone https://github.com/yourusername/Workwise.git
 cd Workwise
```

### 2. Restore the Database
The database backup file (**CoworkingDB.bak**) is located in the root folder. To restore it:

1. Open **SQL Server Management Studio (SSMS)**.
2. Connect to your local SQL Server instance.
3. Right-click on **Databases** > Select **Restore Database**.
4. Choose **Device**, click **Add**, and select `CoworkingDB.bak`.
5. Click **OK** to restore the database.

### 3. Update Connection String in workwise_app.sln
In `App.config`, replace the connection string inside `<connectionStrings>` with your own SQL Server instance.

Example connection string:
```xml
<add name="BookingModelContainer" connectionString="metadata=res://*/BookingModel.csdl|res://*/BookingModel.ssdl|res://*/BookingModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=YOUR_SERVER_NAME;initial catalog=CoworkingDB;integrated security=True;encrypt=False;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
```
Replace `YOUR_SERVER_NAME` with your actual SQL Server instance name.

### 4. Build and Run the Application
1. Open the project in **Visual Studio**.
2. Restore NuGet packages if needed.
3. Build and run the application.

## Contributing
Feel free to fork this repository and submit pull requests for improvements.
# ActiveDirectory.NET library for .NET 

Active Directory Group and User listings, Authentication and Other Helper Classes

Library Version: v1.0.1


## Installation

```powershell
Install-Package ActiveDirectory.NET
```

## Usage

### Create AD object
```
var ad = new ActiveDirectory.NET.AD("192.168.0.1");
```

### Create AD object with User impersonation
```
var ad = new ActiveDirectory.NET.AD("192.168.0.1", "user", "password", "domain");
```

### Get All Groups
```
var groups = ad.GetGroups();
foreach (var group in groups)
{
    Console.WriteLine(group);
}
```

### Get Groups by Name
```
var groups = ad.GetGroups("IT");
foreach (var group in groups)
{
    Console.WriteLine(group);
}
```

### Get All Users
```
var users = ad.GetUsers();
foreach (var user in users)
{
    Console.WriteLine(user);
}
```

### Get Users by Group Name
```
var users = ad.GetUsersByGroupName("IT");
foreach (var user in users)
{
    Console.WriteLine(user);
}
```

### Get Users by User Name (partial user name or complete)
```
var users = ad.GetUsersByUserName("user", Partial: true);
foreach (var user in users)
{
    Console.WriteLine(user);
}
```

### Authenticate a User
```
bool isAuth = ad.Authenticate("user", "password", "domain");
```

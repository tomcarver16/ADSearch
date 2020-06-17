# ADSearch
A tool written in C# to help query AD more effectively.

## Usage
```
-d, --domain       The domain to read information from. If blank we will attempt to determine the current domain the computer is joined to.

-G, --groups       Enumerate and return all groups from AD

-C, --computers    Enumerate and return all computers joined to the AD

-S, --spns         Enumerate and return all SPNS from AD

-s, --search       Perform a custom search on the AD server

-f, --full         If set will show all attributes for the returned item

Example Usage:

ADSearch -d ldap.example.net --groups --computers --spns
```

## Screenshots
![alt text](https://github.com/tomcarver16/ADSearch/blob/master/Images/all-groups.png "All Groups")
![alt text](https://github.com/tomcarver16/ADSearch/blob/master/Images/all-users.png "All Users")

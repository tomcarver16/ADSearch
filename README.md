# ADSearch
A tool written for cobalt-strike's `execute-assembly` command that allows for more efficent querying of AD.

## Usage
```
ADSearch 1.0.0.0
Copyright c  2020
USAGE:
Query Active Directory remotely or locally:
  ADSearch --domain ldap.example.com --password AdminPass1 --username admin --users

  -f, --full          If set will show all attributes for the returned item.

  --supress-banner    When set banner will be disabled.

  -G, --groups        Enumerate and return all groups from AD.

  -U, --users         Enumerate and return all users from AD.

  -C, --computers     Enumerate and return all computers joined to the AD.

  -S, --spns          Enumerate and return all SPNS from AD.

  -s, --search        Perform a custom search on the AD server.

  -u, --username      Attempts to authenticate to AD with the given username.

  -p, --password      Attempts to authenticate to AD with the given password.

  -i, --ip            If set will attempt a remote bind to the ip address. This option requires the domain option to be set to a valid DC on the IP address

  -p, --port          (Default: 389) If set will attempt a remote bind to the port based on the IP.

  -d, --domain        The domain controller we are connecting to in the FQDN format. If left blank then all other connection options are ignored and the lookups are done locally.

  --help              Display this help screen.

  --version           Display version information.
```

## Screenshots
![alt text](https://github.com/tomcarver16/ADSearch/blob/master/Images/all-groups.png "All Groups")
![alt text](https://github.com/tomcarver16/ADSearch/blob/master/Images/all-users.png "All Users")

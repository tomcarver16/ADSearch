using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace ADSearch {
    interface IConnectionOptions {
        [Option('u', "username", Default = null, HelpText = "Attempts to authenticate to AD with the given username.")]
        string Username { get; set; }

        [Option('p', "password", Default = null, HelpText = "Attempts to authenticate to AD with the given password.")]
        string Password { get; set; }

        [Option('i', "ip", Default = null, 
            HelpText = "If set will attempt a remote bind to the ip address. This option requires the domain option to be set to a valid DC on the IP address", SetName = "bind")]
        string IP { get; set; }

        [Option('p', "port", Default = "389", HelpText = "If set will attempt a remote bind to the port based on the IP.", SetName = "bind")]
        string Port { get; set; }

        [Option('d', "domain", Default = null,
            HelpText = "The domain controller we are connecting to in the FQDN format. If left blank then all other connection options are ignored and the lookups are done locally.", SetName = "bind")]
        string Domain { get; set; }
    }

    interface IOutputOptions {
        [Option('f', "full", HelpText = "If set will show all attributes for the returned item.")]
        bool Full { get; set; }

        [Option("supress-banner", HelpText = "When set banner will be disabled.")]
        bool SupressBanner { get; set; }
    }

    interface IQueryOptions {
        [Option('G', "groups", HelpText = "Enumerate and return all groups from AD.")]
        bool Groups { get; set; }

        [Option('U', "users", HelpText = "Enumerate and return all users from AD.")]
        bool Users { get; set; }

        [Option('C', "computers", HelpText = "Enumerate and return all computers joined to the AD.")]
        bool Computers { get; set; }

        [Option('S', "spns", HelpText = "Enumerate and return all SPNS from AD.")]
        bool Spns { get; set; }

        [Option('s', "search", HelpText = "Perform a custom search on the AD server.")]
        string Search { get; set; }
    }

    class Options : IOutputOptions, IQueryOptions, IConnectionOptions {
        public string Domain { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string IP { get; set; }

        public string Port { get; set; }

        public bool Groups { get; set; }

        public bool Users { get; set; }

        public bool Computers { get; set; }

        public bool Spns { get; set; }

        public string Search { get; set; }

        public bool Full { get; set; }

        public bool Local { get; set; }

        public bool SupressBanner { get; set; }

        [Usage(ApplicationAlias = "ADSearch")]
        public static IEnumerable<Example> Examples {
            get {
                return new List<Example>() {
                    new Example("Query Active Directory remotely or locally", new Options { Domain = "ldap.example.com", Username = "admin", Password = "AdminPass1", Users = true })
                };
            }
        }
    }
}

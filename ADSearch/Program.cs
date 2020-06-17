using System;
using CommandLine;

namespace ADSearch {

    class Program {
        class Options {
            [Option('d', "domain", HelpText = "The domain to read information from. If blank we will attempt to determine the current domain the computer is joined to.")]
            public string Domain { get; set; }

            [Option('G', "groups", HelpText = "Enumerate and return all groups from AD")]
            public bool Groups { get; set; }

            [Option('U', "users", HelpText = "Enumerate and return all users from AD")]
            public bool Users { get; set; }

            [Option('C', "computers", HelpText = "Enumerate and return all computers joined to the AD")]
            public bool Computers { get; set; }

            [Option('S', "spns", HelpText = "Enumerate and return all SPNS from AD")]
            public bool Spns { get; set; }

            [Option('s', "search", HelpText = "Perform a custom search on the AD server")]
            public string Search { get; set; }

            [Option('f', "full", HelpText = "If set will show all attributes for the returned item")]
            public bool Full { get; set; }
        }

        static void Main(string[] args) {
            var cmdOptions = Parser.Default.ParseArguments<Options>(args);
            cmdOptions.WithParsed(
                options => {
                    Entry(options);
                });
        }

        static void Entry(Options options) {
            ADWrapper AD;
            if (options.Domain == null) {
                AD = new ADWrapper();
            } else {
                AD = new ADWrapper(options.Domain);
            }

            OutputFormatting.PrintVerbose(AD.GetCurrentDomainPath());

            if (options.Groups) {
                OutputFormatting.PrintVerbose("ALL GROUPS: ");
                AD.ListAllGroups(options.Full);
            }
            
            if (options.Users) {
                OutputFormatting.PrintVerbose("ALL USERS: ");
                AD.ListAllUsers(options.Full);
            }

            if (options.Computers) {
                OutputFormatting.PrintVerbose("ALL COMPUTERS: ");
                AD.ListAllComputers(options.Full);
            }

            if (options.Search != null) {
                OutputFormatting.PrintVerbose("ALL SPNS: ");
                AD.ListCustomSearch(options.Search, options.Full);
            }

            if (options.Spns) {
                OutputFormatting.PrintVerbose("ALL SPNS: ");
                AD.ListAllSpns();
            }
        }

        private static void GetHelp() {
            Console.WriteLine("Please enter valid arguments");
        }
    }
}

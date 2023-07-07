using System;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CommandLine;

namespace ADSearch {

    class Program {

        static void Main(string[] args) {
            var cmdOptions = Parser.Default.ParseArguments<Options>(args);
            cmdOptions.WithParsed(
                options => {
                    Entry(options);
                });
        }

        static void PrintBanner() {
            Console.WriteLine(@"
    ___    ____  _____                      __  
   /   |  / __ \/ ___/___  ____ ___________/ /_ 
  / /| | / / / /\__ \/ _ \/ __ `/ ___/ ___/ __ \
 / ___ |/ /_/ /___/ /  __/ /_/ / /  / /__/ / / /
/_/  |_/_____//____/\___/\__,_/_/   \___/_/ /_/  
                                           
Twitter: @tomcarver_
GitHub: @tomcarver16
            ");
        }

        static void Entry(Options options) {
            ADWrapper AD;
            SearchResultCollection srCollection;
            ConsoleFileOutput cf = null;

            if (!options.SupressBanner) {
                PrintBanner();
            }

            if (options.Output != null) {
                cf = new ConsoleFileOutput(options.Output, Console.Out);
                Console.SetOut(cf);
            }

            if (options.Insecure) {
                options.Port = "389";
            }

            if (options.Hostname == null && options.Domain != null) {
                //No IP but domains set
                AD = new ADWrapper(options.Domain, options.Username, options.Password, options.Insecure, options.JsonOut);
            } else if (options.Hostname != null && options.Domain != null) {
                //This requires the domain so it can be converted into a valid LDAP URI
                AD = new ADWrapper(options.Domain, options.Hostname, options.Port, options.Username, options.Password, options.Insecure, options.JsonOut);
            } else {
                //When no domain is supplied it has to be done locally even if the ip is set otherwise the bind won't work
                OutputFormatting.PrintVerbose("No domain supplied. This PC's domain will be used instead");
                AD = new ADWrapper(options.JsonOut);
            }

            if (options.Attribtues != null && !options.Full) {
                AD.attributesToReturn = options.Attribtues.Split(',');
            }

            OutputFormatting.PrintVerbose(AD.LDAP_URI);

            if (options.Groups) {
                srCollection = AD.GetAllGroups();
                OutputFormatting.PrintVerbose("ALL GROUPS:");
                OutputFormatting.PrintVerbose(String.Format("TOTAL NUMBER OF GROUPS: {0}", srCollection.Count));
                AD.ListAllGroups(srCollection, options.Full);
            }
            
            if (options.Users) {
                srCollection = AD.GetAllUsers();
                OutputFormatting.PrintVerbose("ALL USERS: ");
                OutputFormatting.PrintVerbose(String.Format("TOTAL NUMBER OF USERS: {0}", srCollection.Count));
                AD.ListAllUsers(srCollection, options.Full);
            }

            if (options.Computers) {
                srCollection = AD.GetAllComputers();
                OutputFormatting.PrintVerbose("ALL COMPUTERS: ");
                OutputFormatting.PrintVerbose(String.Format("TOTAL NUMBER OF COMPUTERS: {0}", srCollection.Count));
                AD.ListAllComputers(srCollection, options.Full);
            }

            if (options.Search != null) {
                srCollection = AD.GetCustomSearch(options.Search);
                OutputFormatting.PrintVerbose("CUSTOM SEARCH: ");
                OutputFormatting.PrintVerbose(String.Format("TOTAL NUMBER OF SEARCH RESULTS: {0}", srCollection.Count));
                AD.ListCustomSearch(srCollection, options.Full);
            }

            if (options.Spns) {
                srCollection = AD.GetAllSpns();
                OutputFormatting.PrintVerbose("ALL SPNS: ");
                OutputFormatting.PrintVerbose(String.Format("TOTAL NUMBER OF SPNS: {0}", srCollection.Count));
                AD.ListAllSpns(srCollection, options.Full);
            }

            if (options.DomainAdmins) {
                srCollection = AD.GetAllDomainAdmins();
                OutputFormatting.PrintVerbose("ALL DOMAIN ADMINS: ");
                OutputFormatting.PrintVerbose(String.Format("TOTAL NUMBER OF DOMAIN ADMINS: {0}", srCollection.Count));
                AD.ListAllDomainAdmins(srCollection, options.Full);
            }

            if (options.Output != null) {
                //Close out file handle
                cf.Close();
            }
        }

        private static void GetHelp() {
            Console.WriteLine("Please enter valid arguments");
        }
    }
}

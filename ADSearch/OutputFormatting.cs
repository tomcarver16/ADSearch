using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ADSearch {
    class OutputFormatting {

        enum OUTPUT_TYPE { 
            SUCCESS = '+',
            VERBOSE = '*',
            ERROR = '!'
        };

        public static void PrintADProperties(DirectoryEntry directoryEntry) {
            string border = new String('-', 100);
            string cn = directoryEntry.Properties["cn"].Value.ToString();
            PrintVerbose(String.Format("     |-> {0,-30} | {1}", String.Format("NAME ({0})", cn), "VALUE"));
            PrintVerbose(border);
            foreach (var prop in directoryEntry.Properties.PropertyNames) {
                PrintSuccess(String.Format("     |-> {0,-30} | {1}", prop.ToString(), directoryEntry.Properties[prop.ToString()].Value));
            }
            PrintVerbose(border);
        }

        public static void PrintJson(object obj) {
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }

        public static void PrintSuccess(string msg, int indentation = 0) {
            Print(OUTPUT_TYPE.SUCCESS, msg, indentation);
        }

        public static void PrintVerbose(string msg, int indentation = 0) {
            Print(OUTPUT_TYPE.VERBOSE, msg, indentation);
        }

        public static void PrintError(string msg, int indentation = 0) {
            Print(OUTPUT_TYPE.ERROR, msg, indentation);
        }

        private static void Print(OUTPUT_TYPE msgType, string msg, int indentation = 0) {
            if (indentation != 0) {
                string tabs = new String('\t', indentation);
                Console.WriteLine("{0}[{1}] {2}", tabs, (char)msgType, msg);
            } else {
                Console.WriteLine("[{0}] {1}", (char)msgType, msg);
            }
        }
    }
}

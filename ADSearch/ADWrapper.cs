﻿using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;

namespace ADSearch {
    class ADWrapper {
        private string m_domain;
        private string m_ldapString;
        private DirectoryEntry m_directoryEntry;

        public ADWrapper() {
            this.m_ldapString = GetCurrentDomainPath();
            this.m_directoryEntry = new DirectoryEntry(this.m_ldapString);
        }

        public ADWrapper(string domain) {
            this.m_domain = domain;
            this.m_ldapString = GetDomainPathFromURI(this.m_domain);
            this.m_directoryEntry = new DirectoryEntry(this.m_ldapString);
        }

        private static string GetDomainPathFromURI(string domainURI) {
            StringBuilder sb = new StringBuilder();
            sb.Append("LDAP://");
            foreach (string entry in domainURI.Split('.')) {
                sb.Append("DC=" + entry + ",");
            }
            return sb.ToString().TrimEnd(',');
        }

        public string GetCurrentDomainPath() {
            DirectoryEntry de = new DirectoryEntry("LDAP://RootDSE");

            return "LDAP://" + de.Properties["defaultNamingContext"][0].ToString();
        }

        public void ListAllSpns() {
            SearchResultCollection results = this.CustomSearch("(servicePrincipalName=*)");
            if (results == null) {
                OutputFormatting.PrintError("Unable to obtain any Service Principal Names");
                return;
            }

            ListAttribute(results, "servicePrincipalName");
        }

        public void ListAllGroups(bool full = false) {
            SearchResultCollection results = this.CustomSearch("(objectCategory=group)");
            if (results == null) {
                OutputFormatting.PrintError("Unable to obtain any groups");
                return;
            }

            if (full) {
                ListAll(results);
            } else {
                ListAttribute(results, "cn");
            }
            
        }

        public void ListAllUsers(bool full = false) {
            SearchResultCollection results = this.CustomSearch("(&(objectClass=user)(objectCategory=person))");
            if (results == null) {
                OutputFormatting.PrintError("Unable to obtain any users");
                return;
            }

            if (full) {
                ListAll(results);
            } else {
                ListAttribute(results, "cn");
            }
        }

        public void ListAllComputers(bool full = false) {
            SearchResultCollection results = this.CustomSearch("(objectCategory=computer)");
            if (results == null) {
                OutputFormatting.PrintError("Unable to obtain any computers");
                return;
            }

            if (full) {
                ListAll(results);
            } else {
                ListAttribute(results, "cn");
            }
        }

        public void ListCustomSearch(string query, bool full = false) {
            SearchResultCollection results = this.CustomSearch(query);
            if (results == null) {
                OutputFormatting.PrintError("Unable to carry out custom search");
                return;
            }

            if (full) {
                ListAll(results);
            } else {
                ListAttribute(results, "cn");
            }
        }

        private void ListAll(SearchResultCollection results) {
            foreach (SearchResult result in results) {
                DirectoryEntry userEntry = result.GetDirectoryEntry();
                OutputFormatting.PrintADProperties(userEntry);
            }
        }

        private void ListAttribute(SearchResultCollection results, string attr) {
            foreach (SearchResult result in results) {
                DirectoryEntry userEntry = result.GetDirectoryEntry();
                foreach (Object obj in userEntry.Properties[attr]) {
                    OutputFormatting.PrintSuccess(obj.ToString(), 1);
                }
            }
        }

        private SearchResultCollection CustomSearch(string query) {
            SearchResultCollection results = null;
            try {
                DirectorySearcher searcher = new DirectorySearcher(this.m_directoryEntry);
                searcher.Filter = query;

                results = searcher.FindAll();
            } catch (Exception ex) {
                Console.WriteLine("[!] Unable to communicate with AD server: {0}\nException: {1}", this.m_domain, ex.ToString());
            }
            return results;
        }
    }
}
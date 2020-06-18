using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace ADSearch {
    class ADWrapper {
        private string m_domain;
        private string m_ldapString;
        private string m_domainPath;
        private bool m_json;
        private DirectoryEntry m_directoryEntry;
        private string[] m_attributes;
        private const string PROTOCOL_PREFIX = "LDAP://";

        //Default constructor attempts to detrmine domain from current machine
        public ADWrapper(bool json) {
            this.m_ldapString = GetCurrentDomainPath();
            this.m_domainPath = this.m_ldapString.Substring(PROTOCOL_PREFIX.Length);
            this.m_json = json;
            this.m_directoryEntry = new DirectoryEntry(this.m_ldapString);
        }

        //Bind to FQDN with authentication if creds set else attempt anon bind
        public ADWrapper(string domain, string username, string password, bool insecure, bool json) {
            this.m_domain = domain;
            this.m_json = json;
            this.m_domainPath = GetDCListFromURI(this.m_domain);
            this.m_ldapString = GetDomainPathFromURI(this.m_domainPath);
            
            if (username != null && password != null) {
                if (insecure) {
                    this.m_directoryEntry = new DirectoryEntry(m_ldapString, username, password);
                } else {
                    this.m_directoryEntry = new DirectoryEntry(m_ldapString, username, password, AuthenticationTypes.SecureSocketsLayer | AuthenticationTypes.Secure);
                }
            } else {
                this.m_directoryEntry = new DirectoryEntry(this.m_ldapString);
            }
        }

        //Bind to remote server with authentication if creds set else attempt anon bind
        public ADWrapper(string domain, string hostname, string port, string username, string password, bool insecure, bool json) {
            this.m_domain = domain;
            this.m_json = json;
            this.m_domainPath = GetDCListFromURI(this.m_domain);
            this.m_ldapString = GetDomainPathFromHostname(this.m_domainPath, hostname, port);

            if (username != null && password != null) {
                if (insecure) {
                    this.m_directoryEntry = new DirectoryEntry(m_ldapString, username, password);
                } else {
                    this.m_directoryEntry = new DirectoryEntry(m_ldapString, username, password, AuthenticationTypes.SecureSocketsLayer | AuthenticationTypes.Secure);
                }
            } else {
                this.m_directoryEntry = new DirectoryEntry(m_ldapString);
            }
        }

        public string LDAP_URI {
            get { return m_ldapString; }
        } 

        public string[] attributesToReturn {
            set { this.m_attributes = value; }
        }

        private string GetDomainPathFromURI(string domainPath) {
            return String.Format("{0}{1}", PROTOCOL_PREFIX, domainPath);
        }

        private static string GetDomainPathFromHostname(string domainPath, string hostname, string port) {
            return String.Format("{0}{1}:{2}/{3}", PROTOCOL_PREFIX, hostname, port, domainPath);
        }

        private static string GetDCListFromURI(string uri) {
            StringBuilder sb = new StringBuilder();
            foreach (string entry in uri.Split('.')) {
                sb.Append("DC=" + entry + ",");
            }
            return sb.ToString().TrimEnd(',');
        }

        public string GetCurrentDomainPath() {
            DirectoryEntry de = new DirectoryEntry("LDAP://RootDSE");

            return "LDAP://" + de.Properties["defaultNamingContext"][0].ToString();
        }

        #region Get Functions
        public SearchResultCollection GetAllUsers() {
            SearchResultCollection results = this.CustomSearch("(&(objectClass=user)(objectCategory=person))");
            if (results == null) {
                OutputFormatting.PrintError("Unable to obtain any users");
                throw new Exception("Custom search came back null");
            }
            return results;
        }

        public SearchResultCollection GetAllComputers() {
            SearchResultCollection results = this.CustomSearch("(objectCategory=computer)");
            if (results == null) {
                OutputFormatting.PrintError("Unable to obtain any computers");
                throw new Exception("Custom search came back null");
            }
            return results;
        }

        public SearchResultCollection GetAllGroups() {
            SearchResultCollection results = this.CustomSearch("(objectCategory=group)");
            if (results == null) {
                OutputFormatting.PrintError("Unable to obtain any groups");
                throw new Exception("Custom search came back null");
            }
            return results;
        }

        public SearchResultCollection GetAllSpns() {
            SearchResultCollection results = this.CustomSearch("(servicePrincipalName=*)");
            if (results == null) {
                OutputFormatting.PrintError("Unable to obtain any Service Principal Names");
                throw new Exception("Custom search came back null");
            }
            return results;
        }

        public SearchResultCollection GetAllDomainAdmins() {
            string filter = String.Format("(&(objectCategory=user)(memberOf=CN=Domain Admins,CN=Users,{0}))", this.m_domainPath);
            SearchResultCollection results = this.CustomSearch(filter);
            if (results == null) {
                OutputFormatting.PrintError("Unable to get domain admins");
                throw new Exception("Custom search came back null");
            }
            return results;
        }

        public SearchResultCollection GetCustomSearch(string query) {
            SearchResultCollection results = this.CustomSearch(query);
            if (results == null) {
                OutputFormatting.PrintError("Unable to carry out custom search");
                throw new Exception("Custom search came back null");
            }
            return results;
        }
        #endregion

        #region List Functions
        public void ListAllSpns(SearchResultCollection results, bool full = false) {
            if (full) {
                ListAll(results);
            } else {
                ListAttributes(results, new[] { "servicePrincipalName" });
            }
        }

        public void ListAllGroups(SearchResultCollection results, bool full = false) {
            if (full) {
                ListAll(results);
            } else {
                ListAttributes(results, this.m_attributes);
            }
            
        }

        public void ListAllUsers(SearchResultCollection results, bool full = false) {
            if (full) {
                ListAll(results);
            } else {
                ListAttributes(results, this.m_attributes);
            }
        }

        public void ListAllComputers(SearchResultCollection results, bool full = false) {
            if (full) {
                ListAll(results);
            } else {
                ListAttributes(results, this.m_attributes);
            }
        }

        public void ListAllDomainAdmins(SearchResultCollection results, bool full = false) { 
            if (full) {
                ListAll(results);
            } else {
                ListAttributes(results, this.m_attributes);
            }
        }

        public void ListCustomSearch(SearchResultCollection results, bool full = false) {
            if (full) {
                ListAll(results);
            } else {
                ListAttributes(results, this.m_attributes);
            }
        }

        private void ListAll(SearchResultCollection results) {
            if (this.m_json) {
                OutputFormatting.PrintJson(results);
            } else {
                foreach (SearchResult result in results) {
                    DirectoryEntry userEntry = result.GetDirectoryEntry();
                    OutputFormatting.PrintADProperties(userEntry);
                }
            }
        }

        private void ListAttributes(SearchResultCollection results, string[] attrs) {
            Dictionary<string, Object>[] attributedResults = new Dictionary<string, object>[results.Count];
            for (int i = 0; i < results.Count; i++) {
                attributedResults[i] = new Dictionary<string, object>();
                DirectoryEntry userEntry = results[i].GetDirectoryEntry();
                foreach (string key in attrs) {
                    attributedResults[i].Add(key, userEntry.Properties[key].Value);
                }
            }

            if (!this.m_json) {
                int formatLen = OutputFormatting.GetFormatLenSpecifier(attributedResults[0].Keys.ToArray<string>());
                foreach (Dictionary<string, Object> attributedResult in attributedResults) { 
                    foreach (KeyValuePair<string, object> kvp in attributedResult) {
                        OutputFormatting.PrintSuccess(String.Format("{0,-" + formatLen + "} : {1}", kvp.Key, kvp.Value.ToString()), 1);
                    }
                }
            } else {
                OutputFormatting.PrintJson(attributedResults);
            }
        }
        #endregion

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

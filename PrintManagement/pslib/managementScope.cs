using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.pslib
{
    public sealed class managementScope
    {
        private ManagementScope ms;
        private static readonly managementScope instance = new managementScope();
        static managementScope()
        {
        }
        private managementScope()
        {
        }
        public static managementScope Instance
        {
            get
            {
                return instance;
            }
        }
        public ManagementScope Get()
        {
            if(ms != null && ms.IsConnected)
            {
                return ms;
            }
            var wmiConnectionOptions = new ConnectionOptions();
            wmiConnectionOptions.Impersonation = ImpersonationLevel.Impersonate;
            wmiConnectionOptions.Authentication = AuthenticationLevel.Default;
            wmiConnectionOptions.EnablePrivileges = true; // required to load/install the driver.
                                                          // Supposed equivalent to VBScript objWMIService.Security_.Privileges.AddAsString "SeLoadDriverPrivilege", True 

            string path = "root\\cimv2";
            ms = new ManagementScope(path, wmiConnectionOptions);
            ms.Connect();

            return ms;
        }
    }
}

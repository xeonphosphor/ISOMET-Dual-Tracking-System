using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace ISOMET_Dual_Tracking_System
{
    internal class ConfigClass
    {
        // Network strings
        public string? serverName;
        public string? networkName;
        public string? networkPassword;

        // Database strings
        public string? database1;
        public string? database2;
        public string? database3;

        // User strings
        public string? name;
        public string? location;

        private void saveConfigChanges()
        {
            // Set network values
            Preferences.Set("Server", serverName);
            //Preferences.Set("NetworkName", networkName);
            //Preferences.Set("NetworkPassword", networkPassword);

            // Set database values
            Preferences.Set("Database", database1);
            Preferences.Set("Database2", database2);
            Preferences.Set("Database3", database3);

            // Set user values
            Preferences.Set("Name", name);
            Preferences.Set("Location", location);
        }

        public void updateConfig()
        {
            saveConfigChanges();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CScan.Components.HiJackThis
{
    class O21 : Component
    {
        public bool Run(ref CScan.Report report, List<Dictionary<string, string>> list)
        {
            bool hasEntries = false;

            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\ShellServiceObjectDelayLoad"))
            {
                string[] valueNames = key.GetValueNames();

                foreach (string valueName in valueNames)
                {
                    string value = (string) key.GetValue(valueName);
                    string dll = DllFromClsid(value);

                    list.Add(new Dictionary<string, string>() {
                        { "token", "O21" },
                        { "clsid", value },
                        { "name", valueName },
                        { "dll", dll == null ? "(file not found)" : dll },
                    });

                    hasEntries = true;
                }
            }

            if (hasEntries)
            {
                report.Add(list);
            }

            return true;
        }

        public string DllFromClsid(string clsid)
        {
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Classes\CLSID\" + clsid))
            {
                if (key == null)
                {
                    return null;
                }

                return (string) key.GetValue(null);
            }
        }
    }
}
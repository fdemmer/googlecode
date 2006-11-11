using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PasteToFile
{

    class RegistryHandler
    {
        private Program Prog;

        private RegistryKey hkcu;       // HKEY_CURRENT_USER
        private RegistryKey hkcr;       // HKEY_CLASSES_ROOT

        private String PathText;        // path of registry key for contextmenu text
        private String PathCommand;     // path of registry key for contextmenu command 
        private String PathSoftware;    // path of registry key for configuration

        internal RegistryKey Config;    // registry key for configuration key/values
        private RegistryKey RegMenu;    // registry key for contextmenu text
        private RegistryKey RegCommand; // registry key for contextmenu command

        public RegistryHandler(Program prog)
        {
            Prog = prog;

            hkcu = Registry.CurrentUser;
            hkcr = Registry.ClassesRoot;

            PathText = "Folder\\shell\\" + Ressource.Title;
            PathCommand = "Folder\\shell\\" + Ressource.Title + "\\command";
            PathSoftware = "Software\\";

            // open or create configuration key, writeable
            Config = openOrCreateKey(PathSoftware + Ressource.Title, hkcu);
        }

        // open read only or writeable
        private Microsoft.Win32.RegistryKey openKey(String name, RegistryKey hierachy, bool writeable)
        {
            RegistryKey key = null;

            try
            {
                // open subkey
                key = hierachy.OpenSubKey(name, writeable);
            }
            catch (System.ObjectDisposedException)
            {
                // registry key is closed and connot be opened
            }
            catch (System.Security.SecurityException)
            {
                // insufficient rights to access key
            }

            return key;
        }

        // create or open writeable
        private RegistryKey openOrCreateKey(String name, RegistryKey hierachy)
        {
            RegistryKey key = null;

            try
            {
                // open/create *key* with read/write in *hierachy*
                key = hierachy.CreateSubKey(name);
            }
            catch (System.ObjectDisposedException)
            {
                // registry key is closed and connot be opened
            }
            catch (System.Security.SecurityException)
            {
                // insufficient rights to access key
            }
            catch (System.IO.IOException)
            {
                // systemerror, boxing > 510, wrong tree (local machine)
            }
            catch (System.UnauthorizedAccessException)
            {
                // insufficient rights to access key
            }

            return key;
        }

        internal void registerContextItem(String text, String command)
        {
            try
            {
                RegMenu = Registry.ClassesRoot.CreateSubKey(PathText);
                if (RegMenu != null)
                    RegMenu.SetValue("", text);
                RegCommand = Registry.ClassesRoot.CreateSubKey(PathCommand);
                if (RegCommand != null)
                    RegCommand.SetValue("", command);
            }
            catch (Exception ex)
            {
                Prog.doBalloon(ex.ToString(), Ressource.Title, ToolTipIcon.Error);
            }
            finally
            {
                if (RegMenu != null)
                    RegMenu.Close();
                if (RegCommand != null)
                    RegCommand.Close();
            }
        }

        internal void deleteContextItem()
        {
            RegistryKey RegKey = null;
            try
            {
                RegKey = hkcr.OpenSubKey(PathCommand);
                if (RegKey != null)
                {
                    RegKey.Close();
                    hkcr.DeleteSubKey(PathCommand);
                }
                RegKey = Registry.ClassesRoot.OpenSubKey(PathText);
                if (RegKey != null)
                {
                    RegKey.Close();
                    hkcr.DeleteSubKey(PathText);
                }
            }
            catch (Exception ex)
            {
                Prog.doBalloon(ex.ToString(), Ressource.Title, ToolTipIcon.Error);
            }
        }

        internal bool existingContextItem()
        {
            bool retval = false;

            try
            {
                RegCommand = hkcr.OpenSubKey(PathCommand);
                RegMenu = hkcr.OpenSubKey(PathText);
                if (RegCommand != null && RegMenu != null)
                    retval = true;
            }
            catch (Exception ex)
            {
                Prog.doBalloon(ex.ToString(), Ressource.Title, ToolTipIcon.Error);
            }

            return retval;
        }

    } // end of class

}

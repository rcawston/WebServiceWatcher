/*
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System.Configuration.Install;
using System.Reflection;

namespace WebServiceWatcher.Util
{
    /// <summary>
    /// Creates or removes a windows service for the executing assembly.
    /// </summary>
    public static class SelfServiceInstaller
    {
        private static readonly string ExePath = Assembly.GetExecutingAssembly().Location;

        /// <summary>
        /// Creates/installs the service
        /// </summary>
        /// <returns>Installation success</returns>
        public static bool InstallService()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new[] { ExePath });
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Removes/uninstalls the service
        /// </summary>
        /// <returns>Uninstallation success</returns>
        public static bool UninstallService()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new[] { "/u", ExePath });
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}

using ADUserManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;
using System.Reflection;

namespace ADUserManagement.API
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var info = new IndexModel();
            var assemblyAll = Assembly.GetEntryAssembly()?.GetName();
            info.IP = GetIPAddress();
            info.AssemblyName = assemblyAll?.Name;
            info.AssemblyVersion = assemblyAll?.Version?.ToString();
            info.AspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            info.DateModified = GetDateModified();
            return View(info);
        }

        private string GetIPAddress()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                string HostName_ = Dns.GetHostEntry(Dns.GetHostName()).HostName;
                return HostName_;
            }
            catch
            {
                return "";
            }
        }

        private string GetDateModified()
        {
            var assemblyExe = Assembly.GetExecutingAssembly();
            FileInfo file = new FileInfo(assemblyExe.Location);
            return file.LastWriteTime.ToString("dd/MM/yyy : HH:mm:ss", new CultureInfo("en-EN", false));
        }
    }
}

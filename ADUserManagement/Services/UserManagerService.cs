using ADUserManagement.Models;
//using System.Reflection.PortableExecutable;
using System.DirectoryServices;


namespace ADUserManagement.Services
{
    public class UserManagerService
    {
        private readonly IConfiguration _configuration;

        public UserManagerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void CreateUser(string username, string password, string domainName)
        {
            var domains = _configuration.GetSection("Domains").Get<DomainConfigModel[]>();
            foreach (var domainConfig in domains)
            {
                if (domainConfig.DomainName == domainName)
                {
                    using (var entry = new DirectoryEntry($"LDAP://{domainName}", domainConfig.Username, domainConfig.Password))
                    {
                        using (var newUser = entry.Children.Add($"CN={username}", "user"))
                        {
                            newUser.Properties["samAccountName"].Value = username;
                            newUser.CommitChanges();
                        }
                    }
                    Console.WriteLine($"User {username} created in {domainName}.");
                    return;
                }
            }
            Console.WriteLine("Domain not found.");
        }

        public void LockUser(string username, string domainName)
        {
            ModifyUserAccountControl(username, domainName, 0x0002); // 0x0002 = ACCOUNTDISABLE
        }

        public void UnlockUser(string username, string domainName)
        {
            ModifyUserAccountControl(username, domainName, 0x0000); // 0x0000 = Account enabled
        }

        public void ChangePassword(string username, string domainName, string newPassword)
        {
            var domains = _configuration.GetSection("Domains").Get<DomainConfigModel[]>();
            foreach (var domainConfig in domains)
            {
                if (domainConfig.DomainName == domainName)
                {
                    using (var entry = new DirectoryEntry($"LDAP://{domainName}/{username}", domainConfig.Username, domainConfig.Password))
                    {
                        entry.Invoke("SetPassword", new object[] { newPassword });
                        entry.CommitChanges();
                    }
                    Console.WriteLine($"Password for user {username} has been changed.");
                    return;
                }
            }
            Console.WriteLine("Domain or user not found.");
        }

        private void ModifyUserAccountControl(string username, string domainName, int accountControlFlag)
        {
            var domainConf = _configuration.GetSection("Domains").Get<DomainConfigModel[]>()?.Where(x => x.DomainName.ToLower().Equals(domainName.ToLower())).FirstOrDefault();
            if (domainConf != null)
            {
                using (var entry = new DirectoryEntry($"LDAP://{domainName}/{username}", domainConf.Username, domainConf.Password))
                {
                    // Get the current user account control
                    int userAccountControl = (int)entry.Properties["userAccountControl"].Value;

                    // Modify the user account control
                    userAccountControl = (userAccountControl & ~0x0002) | accountControlFlag; // Disable or enable the account

                    entry.Properties["userAccountControl"].Value = userAccountControl;
                    entry.CommitChanges();
                }
                Console.WriteLine($"User {username} has been " + (accountControlFlag == 0x0002 ? "locked." : "unlocked."));
                return;
            }
            else
            {
                Console.WriteLine("Domain or user not found.");
                return;
            }


            ////----- Current Code -----////
            //var domains = _configuration.GetSection("Domains").Get<DomainConfigModel[]>();
            //foreach (var domainConfig in domains)
            //{
            //    if (domainConfig.DomainName == domainName)
            //    {
            //        using (var entry = new DirectoryEntry($"LDAP://{domainName}/{username}", domainConfig.Username, domainConfig.Password))
            //        {
            //            // Get the current user account control
            //            int userAccountControl = (int)entry.Properties["userAccountControl"].Value;

            //            // Modify the user account control
            //            userAccountControl = (userAccountControl & ~0x0002) | accountControlFlag; // Disable or enable the account

            //            entry.Properties["userAccountControl"].Value = userAccountControl;
            //            entry.CommitChanges();
            //        }
            //        Console.WriteLine($"User {username} has been " + (accountControlFlag == 0x0002 ? "locked." : "unlocked."));
            //        return;
            //    }
            //}
            //Console.WriteLine("Domain or user not found.");
        }
    }
}

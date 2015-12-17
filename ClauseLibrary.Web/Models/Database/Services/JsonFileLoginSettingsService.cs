using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using ClauseLibrary.Web.Models.Database.LoginSettings;
using Newtonsoft.Json;

namespace ClauseLibrary.Web.Models.Database.Services
{
    /// <summary>
    /// Provides access to the login settings.
    /// </summary>
    public class JsonFileLoginSettingsService : ILoginSettingsService
    {
        private const string DATA_PATH = @"App_Data\";
        private const int RETRY_COUNT = 10;
        private readonly string _root;
        private Settings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFileLoginSettingsService"/> class.
        /// </summary>
        public JsonFileLoginSettingsService()
        {
            _root = HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~") : GetProjectDirectory();
            _settings = LoadSettings();
        }

        private static string GetProjectDirectory()
        {
            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            return directoryInfo != null ? directoryInfo.FullName : null;
        }

        private Settings LoadSettings()
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(EnsureSettingsFile()))
                {
                    string contents = streamReader.ReadToEnd();
                    Settings settings = JsonConvert.DeserializeObject<Settings>(contents) ?? new Settings();

                    return settings;
                }
            }
            catch (IOException)
            {
                return new Settings();
            }
        }
       
        private void SaveSettings(int retry)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(EnsureSettingsFile()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(_settings));
                }
            }
            catch (IOException)
            {
                if (retry < RETRY_COUNT)
                    SaveSettings(++retry);
            }
        }

        /// <summary>
        /// Generates a .json file in the project root App_Data directory
        /// </summary>
        /// <returns>Returns the created filepath</returns>
        private string EnsureSettingsFile()
        {
            var filepath = GetSettingsFilePath();
            if (!Directory.Exists(GetDataPath()))
            {
                Directory.CreateDirectory(GetDataPath());
            }
            if (!File.Exists(GetSettingsFilePath()))
            {
                File.Create(filepath).Close();
            }
            return filepath;
        }

        /// <summary>
        /// Builds a path from the root directory to the logs directory
        /// </summary>
        /// <returns>Returns the path to the logs directory as a string</returns>
        private string GetSettingsFilePath()
        {
            var filename = "LoginSettings.json";
            string directoryPath = GetDataPath();
            return Path.Combine(directoryPath, filename);
        }

        private string GetDataPath()
        {
            return Path.Combine(_root, DATA_PATH);
        }

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public User GetUserById(Guid userId)
        {
            User user = _settings.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                user.DefaultLibrary = _settings.Libraries.FirstOrDefault(l => l.LibraryId == user.DefaultLibraryId);
                user.Tenant = GetTenantById(user.TenantId);
            }

            return user;
        }

        /// <summary>
        /// Gets the tenant by identifier.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        public Tenant GetTenantById(Guid tenantId)
        {
            Tenant tenant = _settings.Tenants.FirstOrDefault(t => t.TenantId == tenantId);
            if (tenant != null)
            {
                tenant.Libraries = _settings.Libraries.Where(library => library.TenantId == tenant.TenantId).ToList();
            }
            return tenant;
        }

        /// <summary>
        /// Gets the library by identifier.
        /// </summary>
        /// <param name="libraryId">The library identifier.</param>
        public Library GetLibraryById(Guid libraryId)
        {
            return _settings.Libraries.FirstOrDefault(l => l.LibraryId == libraryId);
        }

        /// <summary>
        /// Adds the specified new entity.
        /// </summary>
        public void Add<T>(T newEntity) where T : class
        {
            if (newEntity is User)
            {
                _settings.Users.Add(newEntity as User);
            }
            else if (newEntity is Tenant)
            {
                _settings.Tenants.Add(newEntity as Tenant);
            }
            else if (newEntity is Library)
            {
                _settings.Libraries.Add(newEntity as Library);
            }
        }

        /// <summary>
        /// Deletes the specified entity to delete.
        /// </summary>
        public void Delete<T>(T entityToDelete) where T : class
        {
            if (entityToDelete is User)
            {
                User userToRemove = _settings.Users.FirstOrDefault(item => item.UserId == (entityToDelete as User).UserId);
                _settings.Users.Remove(userToRemove);
            }
            else if (entityToDelete is Tenant)
            {
                Tenant tenantToRemove = _settings.Tenants.FirstOrDefault(item => item.TenantId == (entityToDelete as Tenant).TenantId);
                _settings.Tenants.Remove(tenantToRemove);
            }
            else if (entityToDelete is Library)
            {
                Library libraryToRemove = _settings.Libraries.FirstOrDefault(item => item.LibraryId == (entityToDelete as Library).LibraryId);
                _settings.Libraries.Remove(libraryToRemove);
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            SaveSettings(RETRY_COUNT);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        private class Settings
        {
            public Settings()
            {
                Libraries = new List<Library>();
                Tenants = new List<Tenant>();
                Users = new List<User>();
            }

            public List<Library> Libraries { get; set; }
            public List<Tenant> Tenants { get; set; }
            public List<User> Users { get; set; }
        }
    }
}
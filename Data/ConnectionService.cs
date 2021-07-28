using System;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BlogProject.Data
{
    public class ConnectionService
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseURL = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseURL) ? connectionString : BuildConnectionString(databaseURL);
        }

        private static string BuildConnectionString(string databaseURL)
        {
            var databaseURI = new Uri(databaseURL);
            var userInfo = databaseURI.UserInfo.Split(':');

            return new NpgsqlConnectionStringBuilder
            {
                Host = databaseURI.Host,
                Port = databaseURI.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseURI.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            }.ToString();
        }
    }
}
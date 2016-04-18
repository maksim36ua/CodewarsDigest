using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
//using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace cw_itkpi.Models
{
    public class UserInfo
    {
        private string _username;
        private string _clan;
        private string _vkLink;
        private int _honor;
        private int _thisWeekHonor;
        private int _lastWeekHonor;

        public UserInfo()
        {
            username = "";
            lastWeekHonor = 0;
        }

        public UserInfo(string name)
        {
            username = name;
            lastWeekHonor = 0;
        }

        [Key]
        public string username
        {
            get { return _username; }
            set { _username = value; }
        }

        public int honor
        {
            get { return _honor; }
            set { _honor = value; }
        }

        public int thisWeekHonor
        {
            get { return _thisWeekHonor; }
            set { _thisWeekHonor = value; }
        }

        public int lastWeekHonor
        {
            get { return _lastWeekHonor; }
            set { _lastWeekHonor = value; }
        }

        public string clan
        {
            get { return _clan; }
            set { _clan = value; }
        }

        [JsonIgnore]
        public string vkLink
        {
            get { return _vkLink; }
            set { _vkLink = value; }
        }

        public string RetrieveValues()
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

            var json = GetJsonResponse(username).Result;

            if (string.IsNullOrEmpty(json))
            {
                return "";
            }
            else
            {
                // Deserialize JSON into itself object class
                var jsonObject = JsonConvert.DeserializeObject<UserInfo>(json, jsonSerializerSettings);

                honor = jsonObject.honor;
                clan = jsonObject.clan;
                lastWeekHonor = RetrieveLastWeekPoints(username);
                thisWeekHonor = honor - lastWeekHonor;

                return "Ok";
            }
        }


        private async Task<string> GetJsonResponse(string username)
        {
            using (var request = new HttpClient())
            {
                request.DefaultRequestHeaders.Add("Authorization", "c7T7urhMy8ycrT7TxTsC");
                var baseUri = "https://www.codewars.com/api/v1/users/" + username;
                request.BaseAddress = new Uri(baseUri);

                var response = await request.GetAsync(baseUri);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                else
                    return "";
            }
        }

        public void ClearVkLink()
        {
            if (!string.IsNullOrEmpty(vkLink))
            {
                var lastIndex = vkLink.LastIndexOf('/');
                if (lastIndex != -1)
                {
                    vkLink = vkLink.Substring(lastIndex + 1, vkLink.Length - (lastIndex + 1));
                }
                else
                    vkLink = "";
            }
        }

        public int RetrieveLastWeekPoints(string username)
        {
            var builder = new ConfigurationBuilder()
                                .AddUserSecrets();
            IConfigurationRoot Configuration = builder.Build();

            using (SqlConnection connection = new SqlConnection(Configuration["Data:DefaultConnection"]))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    int weekNumber = -1;
                    bool exists = false;

                    // Check if Week0 exists
                    command.CommandText = $"select case when exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Week0') then 1 else 0 end";
                    if ((int)command.ExecuteScalar() == 0)
                    {
                        //_lastWeekHonor = 0;
                        return 0;
                    }

                    do // Figuring out latest weekly rating
                    {
                        weekNumber++;
                        command.CommandText = $"select case when exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Week{weekNumber}') then 1 else 0 end";
                        exists = (int)command.ExecuteScalar() == 1;
                        if (!exists) break;
                    } while (exists);

                    weekNumber = weekNumber - 1;

                    command.CommandText = $"select honor from Week{weekNumber} where username = '{username}';";
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                return reader.GetInt32(0);
                            }                        
                    }
                    return 0;
                }
            }
        }
    }
}

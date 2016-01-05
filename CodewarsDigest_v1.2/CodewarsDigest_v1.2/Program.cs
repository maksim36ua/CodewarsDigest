using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using System.IO;
using CodewarsDigest_v1;


namespace CodewarsDigest_v1._2
{
    class Program
    {
        public static void ExtractUsersFromHTML(List<UserInfo> userList)
        {     
            HtmlDocument page = new HtmlDocument();
            page.Load(@"C:\Users\maksi\Documents\GitHub\CodewarsDigest\CodewarsDigest_v1.2\CodewarsDigest_v1.2\Data\1.html");
            List<HtmlNode> root = page.DocumentNode.Descendants()
                .Where(n => (n.Name == "div" && n.Attributes["class"] != null && n.Attributes["class"].Value.Contains("leaderboard pan"))).ToList();

            var trTag = root[0].Descendants("tr").ToList(); // first and main "div" with "leaderboard pan" class
          

            foreach (var userBlock in trTag)
            {
                var aTag = userBlock.Descendants("a").ToList(); // adding nickname
                var user = new UserInfo(aTag[0].InnerText);

                var tdTag = userBlock.Descendants("td").ToList();
                user.CurrentPoints = int.Parse(tdTag[2].InnerText);

                userList.Add(user);
            }
            
        }

        public static void ExtractUsersFromTXT(List<UserInfo> userList)
        {
            FileStream file = new FileStream(@"C:\Users\maksi\Documents\GitHub\CodewarsDigest\CodewarsDigest_v1.2\CodewarsDigest_v1.2\Data\WeeklyRatings", FileMode.Open);
            StreamReader stream = new StreamReader(file);

            List<string[]> listOfUsers = new List<string[]>();

            while (!stream.EndOfStream) // Separating input file to fields with user info
            {
                string[] userInfo = stream.ReadLine().Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                listOfUsers.Add(userInfo);
            }

            foreach (var user in listOfUsers)  // Check from end
                foreach(var userObject in userList)
                    if(user[0] == userObject.Name)


          }

        public static void ExtractVkLinks()
        {

        }

        public static void GenerateWeekRating()
        {

        }

        public static void PrintCurrentPointsToTXT()
        {

        }

        public static void PrintWeekRatingInHTML()
        {

        }

        public static void PrintTotalRatingInConsole()
        {

        }

        static void Main(string[] args)
        {
            List<UserInfo> userList = new List<UserInfo>();

            ExtractUsersFromHTML(userList);
            ExtractUsersFromTXT(userList);

            foreach (var user in userList)
                Console.WriteLine(user.Name + " " + user.CurrentPoints);

            Console.Read();
        }

    }
}

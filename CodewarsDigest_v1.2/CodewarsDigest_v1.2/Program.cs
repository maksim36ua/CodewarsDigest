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
            //page.Load(@"C:\Users\maksi\Documents\GitHub\CodewarsDigest\CodewarsDigest_v1.2\CodewarsDigest_v1.2\Data\1.html");
            page.Load(@"Data\1.html");
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

        public static void ExtractUsersFromTXT(List<UserInfo> userList, int lastWeekNumber)
        {
            FileStream file = new FileStream(@"Data\WeeklyRating\totalRating" + lastWeekNumber + ".txt", FileMode.Open);
            StreamReader stream = new StreamReader(file);

            List<string[]> listOfUsers = new List<string[]>();

            while (!stream.EndOfStream) // Separating input file to fields with user info
            {
                string[] userInfo = stream.ReadLine().Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                listOfUsers.Add(userInfo);
            }

            foreach (var user in listOfUsers)  // For each in list from .txt
                foreach (var userObject in userList) // For each in list of user objects
                {
                    if (2 == user.Length) // If username consist of 1 word
                    {
                        if (user[0] == userObject.Name)
                            userObject.LastWeekPoints = int.Parse(user[1]);
                    }
                    else if (3 == user.Length)
                    {
                        if ((user[0]+" "+user[1]) == userObject.Name)
                            userObject.LastWeekPoints = int.Parse(user[2]);
                    }
                }
                    
        }

        public static void GenerateWeekRating(List<UserInfo> userList)
        {
            foreach (var user in userList)
                user.PointsForThisWeek = user.CurrentPoints - user.LastWeekPoints;
        }

        public static void PrintInTXT(List<UserInfo> userList, int thisWeekNumber)
        {
            using (FileStream file = new FileStream(@"Data\WeeklyRating\totalRating" + thisWeekNumber + ".txt", FileMode.Append))
            {
                using (StreamWriter stream = new StreamWriter(file))
                {
                    foreach (var user in userList)
                        stream.WriteLine(user.Name + " " + user.CurrentPoints);
                }
            }

            using (FileStream file = new FileStream(@"Data\ForSite.txt", FileMode.Open))
            {
                using (StreamWriter stream = new StreamWriter(file))
                {
                    for (int i = 0; i < userList.Count; i++)
                        stream.WriteLine($"{i+1}.  {userList[i].Name}  ({userList[i].CurrentPoints}, { userList[i].PointsForThisWeek})");

                }
            }
        }

        public static List<string[]> ExtractVkLinksFromTXT()
        {
            using (FileStream fileNicks = new FileStream(@"Data\nicknames.txt", FileMode.Open))
            {
                using (StreamReader streamNicks = new StreamReader(fileNicks))
                {
                    List<string[]> listOfNicknames = new List<string[]>();
                    while (!streamNicks.EndOfStream) // Separating nickname file to user field
                    {
                        string[] nickAndLink = streamNicks.ReadLine()
                            .Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                        listOfNicknames.Add(nickAndLink);
                    }

                    foreach (var nickAndLink in listOfNicknames) // remove "vk.com"
                    {
                        var lastIndex = nickAndLink[1].LastIndexOf('/');
                        if (lastIndex != -1)
                        {
                            nickAndLink[1] = nickAndLink[1].Substring(lastIndex + 1,
                                nickAndLink[1].Length - (lastIndex + 1));
                        }
                    }

                    return listOfNicknames;
                }
            }
        }

        public static List<UserInfo> SortActiveUsersOfThisWeek(List<UserInfo> userList)
        {
            userList.RemoveAll(user => user.PointsForThisWeek == 0);            
            List<UserInfo> sortedList = userList.OrderByDescending(user => user.PointsForThisWeek).ToList();
            sortedList.ForEach(user => Console.WriteLine(user.Name + " " + user.PointsForThisWeek));
            return sortedList;
        }

        public static void PrintInConsoleAndHTML(List<UserInfo> userList)
        {
            List<string[]> listOfNicknamesAndVKLinks = ExtractVkLinksFromTXT();
            List<UserInfo> activeUserList = SortActiveUsersOfThisWeek(userList);

            //foreach (var user in activeUserList)
            for(int i = 0; i < activeUserList.Count; i++)
                foreach (var nick in listOfNicknamesAndVKLinks)
                {
                    if (nick.Length == 2) // If username consists of 1 word
                    {
                        if (nick[0] == activeUserList[i].Name)
                            Console.WriteLine($"{i + 1}.  @{nick[1]}({activeUserList[i].Name})  ({activeUserList[i].PointsForThisWeek})");
                    }

                    if (nick.Length == 3) // If username consists of 2 words
                    {
                        if ((nick[0] + " " + nick[1]) == activeUserList[i].Name) // (i+1) + ".   @" + nick[1] + ()
                            Console.WriteLine($"{i + 1}.  @{nick[1]}({activeUserList[i].Name})  ({activeUserList[i].PointsForThisWeek})");
                    }
                }


        } // TODO: HTML OUTPUT, users without links printed incorrectly
        

        static void Main(string[] args)
        {
            List<UserInfo> userList = new List<UserInfo>();
            int thisWeekNumber = 0;

            for (int i = 0; i < 100; i++) // Checking what is the number of current week
            {
                if (!File.Exists(@"Data\WeeklyRating\totalRating" + i + ".txt"))
                {
                    thisWeekNumber = i;
                    break;
                }
                else continue;
            }

            ExtractUsersFromHTML(userList);
            ExtractUsersFromTXT(userList, thisWeekNumber - 1);
            GenerateWeekRating(userList);
            PrintInTXT(userList, thisWeekNumber);
            PrintInConsoleAndHTML(userList);

            //foreach (var user in userList)
            //    Console.WriteLine(user.Name + " " + user.LastWeekPoints 
            //        + " " + user.CurrentPoints + " : " +user.PointsForThisWeek);

            Console.Read();
        }

    }
}

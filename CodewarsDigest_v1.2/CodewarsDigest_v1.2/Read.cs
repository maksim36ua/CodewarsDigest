﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using UserInfoClass;
using System.IO;

namespace CodewarsDigest_v1._2
{
    class Read
    {
        private static void ExtractUsersFromHTML(List<UserInfo> userList)
        {
            HtmlDocument page = new HtmlDocument();
            page.Load(@"Data\1.html");

            List<HtmlNode> root = page.DocumentNode.Descendants() // Extracting info of other users
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

            UserInfo myUserInfo = new UserInfo("maksim36ua"); // My user object

            List<HtmlNode> myInfo = page.DocumentNode.Descendants() // Extract my info
                .Where(n => (n.Name == "div" && n.Attributes["class"] != null && n.Attributes["class"].Value.Contains("honor"))).ToList();

            var spanTag = myInfo[0].Descendants().ToList(); // Extracting my points
            myUserInfo.CurrentPoints = int.Parse(spanTag[2].InnerText);

            userList.Add(myUserInfo);

            var tempList = userList.OrderByDescending(user => user.CurrentPoints).ToList(); // Sorting total rating 

            userList.Clear(); // Substituting old list with sorted
            userList.AddRange(tempList);

            //tempList.ForEach(user => Console.WriteLine($"{user.Name} {user.CurrentPoints}"));

        }

        private static void ExtractUsersFromTXT(List<UserInfo> userList, int lastWeekNumber)
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
                        if ((user[0] + " " + user[1]) == userObject.Name)
                            userObject.LastWeekPoints = int.Parse(user[2]);
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
                        var lastIndex = nickAndLink[nickAndLink.Length - 1].LastIndexOf('/');
                        if (lastIndex != -1)
                        {
                            nickAndLink[nickAndLink.Length - 1] = nickAndLink[nickAndLink.Length - 1].Substring(lastIndex + 1,
                                nickAndLink[nickAndLink.Length - 1].Length - (lastIndex + 1));
                        }
                    }

                    return listOfNicknames;
                }
            }
        }

        public static void AllInfo(List<UserInfo> userList, int lastWeekNumber)
        {
            ExtractUsersFromHTML(userList);
            ExtractUsersFromTXT(userList, lastWeekNumber);

            foreach (var user in userList) // Calculating points for this week
                user.PointsForThisWeek = user.CurrentPoints - user.LastWeekPoints;
        }
    }
}
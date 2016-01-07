using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInfoClass
{
    class UserInfo
    {

        private string name;
        private int lastWeekPoints;
        private int currentPoints;
        private int pointsForThisWeek;

        public UserInfo(string name, int currentPoints = 0, int lastWeekPoints = 0)
        {
            this.name = name;
            this.lastWeekPoints = lastWeekPoints;
            this.currentPoints = currentPoints;
            pointsForThisWeek = 0;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int CurrentPoints
        {
            get { return currentPoints; }
            set { currentPoints = value; }
        }

        public int LastWeekPoints
        {
            get { return lastWeekPoints; }
            set { lastWeekPoints = value; }
        }

        public int PointsForThisWeek
        {
            get { return pointsForThisWeek; }
            set { pointsForThisWeek = value; }
        }
        
    }
}

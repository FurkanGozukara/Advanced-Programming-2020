using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace week_5
{
    public static class csPublicVars
    {
        public class csLoggedUser
        {
            public csUserRanks UserRankInfo;
            public string srUserFirstName;
            public string srUserSurname;
        }

        public static List<csUserRanks> lstUserRanks;

        public class csUserRanks
        {
            public int irRank { get; set; }
            public string srRankTitle { get; set; }
        }

        public static csLoggedUser LoggedUserInfo;

        public static string returnUserRankTitle(string srRank)
        {
            foreach (var item in lstUserRanks)
            {
                if (item.irRank.ToString() == srRank)
                    return item.srRankTitle;
            }

            return "Undefined Rank";
        }
    }
}

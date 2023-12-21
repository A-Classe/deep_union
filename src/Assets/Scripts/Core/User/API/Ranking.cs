using System.Collections.Generic;

namespace Core.User.API
{
    public struct Ranking
    {
        public List<RankingUser> users;
    }

    public struct RankingUser
    {
        public string ID;

        public string Name;

        public Dictionary<StageData.Stage, int> Stages;
    }
}
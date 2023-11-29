using System;
using System.Collections.Generic;
using System.Linq;
using Core.User;
using Core.User.API;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace Module.UI.Title.Ranking
{
    public class RankingManager: UIManager
    {
        public enum Nav
        {
            Quit
        }

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private FadeInOutButton quit;
        [SerializeField] private RankingRow rankingRow1;
        [SerializeField] private RankingRow rankingRow2;
        [SerializeField] private RankingRow rankingRow3;
        [SerializeField] private RankingRow rankingRow4;
        [SerializeField] private RankingRow rankingRow5;
        [SerializeField] private RankingRow myRankingRow;

        private List<RankingRow> lists = new();

        private Dictionary<StageData.Stage, List<RankingUser>> rankings = new();

        private Nav? current;

        private void Start()
        {
            cursor.AddPoint(Nav.Quit, quit.rectTransform);
            current = Nav.Quit;

            SetState(Nav.Quit); 
        }

        public override void Clicked()
        {
            OnBack?.Invoke();
        }

        public override void Select(Vector2 direction)
        {
            if (!current.HasValue)
            {
                SetState(Nav.Quit);
                return;
            }
            
        }


        public event Action OnBack;


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }

        public void SetStage(StageData.Stage stage)
        {
            lists.Clear();
            lists.Add(rankingRow1);
            lists.Add(rankingRow2);
            lists.Add(rankingRow3);
            lists.Add(rankingRow4);
            lists.Add(rankingRow5);
            List<RankingUser> currents = rankings[stage];
            for (var i = 0; i < currents.Count; i++)
            {
                if (currents[i].ID == userId)
                {
                    SetRankingRef(myRankingRow, i + 1, currents[i], stage);
                }
                if (i > lists.Count - 1) return;
                SetRankingRef(lists[i], i + 1, currents[i], stage);
            }
            for (var i = currents.Count; i < lists.Count; i++)
            {
                SetRankingRef(
                    lists[i], 
                    i + 1, 
                    new RankingUser{
                        ID = "",
                        Name = "",
                        Stages = new()
                    },
                    stage
                );
            }
        }

        private void SetRankingRef(RankingRow row, int rank, RankingUser user, StageData.Stage stage)
        {
            row.SetRank(rank);
            row.SetRef(user.Name, user.Stages.GetValueOrDefault(stage, 0));
        }

        private string userId;

        public void SetRanking(Core.User.API.Ranking ranking, string userId)
        {
            this.userId = userId;
            
            // ステージごとのランキングをクリア
            rankings.Clear();

            // ステージごとにユーザーを分類
            foreach (var user in ranking.users) {
                foreach (var stage in user.Stages) {
                    if (!rankings.ContainsKey(stage.Key)) {
                        rankings[stage.Key] = new List<RankingUser>();
                    }
                    rankings[stage.Key].Add(user);
                }
            }

            // 各ステージでユーザーをスコア順にソート
            foreach (var stage in rankings.Keys.ToList()) {
                rankings[stage] = rankings[stage].OrderByDescending(user => user.Stages[stage]).ToList();
            }
            
            // foreach (var stage in rankings) {
            //     string data = $"Stage: {stage.Key}\n";
            //     foreach (var user in stage.Value) {
            //         data += $"{user.Name} - Score: {user.Stages[stage.Key]}\n";
            //     }
            //     Debug.Log(data);
            // }
        }
    }
}
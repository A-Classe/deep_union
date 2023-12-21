using System;
using System.Collections.Generic;
using System.Linq;
using Core.User;
using Core.User.API;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using TMPro;
using UnityEngine;

namespace Module.UI.Title.Ranking
{
    public class RankingManager : UIManager
    {
        public enum Nav
        {
            Reload,
            Input,
            Quit
        }

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private FadeInOutButton quit;
        [SerializeField] private FadeInOutButton input;
        [SerializeField] private ValidButton reload;

        [SerializeField] private TMP_InputField inputName;
        [SerializeField] private RankingRow rankingRow1;
        [SerializeField] private RankingRow rankingRow2;
        [SerializeField] private RankingRow rankingRow3;
        [SerializeField] private RankingRow rankingRow4;
        [SerializeField] private RankingRow rankingRow5;
        [SerializeField] private RankingRow myRankingRow;
        [SerializeField] private PopupWindow popupWindow;

        private readonly List<RankingRow> lists = new();

        private readonly Dictionary<StageData.Stage, List<RankingUser>> rankings = new();

        private Nav? current;

        public event Action<string> OnChangedName;

        public event Action OnNeedLoad;

        private void Start()
        {
            cursor.AddPoint(Nav.Reload, reload.rectTransform);
            cursor.AddPoint(Nav.Quit, quit.rectTransform);
            cursor.AddPoint(Nav.Input, input.rectTransform);
            current = Nav.Quit;

            SetState(Nav.Quit);
            Clear();
        }

        private string currentName = "";

        public override void Clicked()
        {
            if (popupWindow.IsVisible)
            {
                popupWindow.SetVisible(false);
                return;
            }

            switch (current)
            {
                case Nav.Quit:
                    OnBack?.Invoke();
                    break;
                case Nav.Reload:
                    if (!reload.IsValid)
                    {
                        Clear();
                        reload.SetValidVisual(true);
                        OnNeedLoad?.Invoke();
                    }

                    break;
                case Nav.Input:
                    if (currentName != inputName.text)
                    {
                        OnChangedName?.Invoke(inputName.text);
                        inputName.text = currentName;
                    }

                    inputName.Select();
                    break;
            }
        }

        public override void Select(Vector2 direction)
        {
            if (popupWindow.IsVisible)
            {
                popupWindow.SetVisible(false);
            }

            if (!current.HasValue)
            {
                SetState(Nav.Quit);
                return;
            }

            if (inputName.isFocused)
            {
                return;
            }

            Nav nextNav;

            switch (direction.y)
            {
                // 上向きの入力
                case > 0:
                    if (current.Value == Nav.Reload)
                    {
                        return;
                    }

                    nextNav = current.Value - 1;
                    break;
                // 下向きの入力
                case < 0:
                    if (current.Value == Nav.Quit)
                    {
                        return;
                    }

                    nextNav = current.Value + 1;
                    break;
                default:
                    return; // Y軸の入力がない場合、何もしない
            }

            SetState(nextNav);
        }


        public event Action OnBack;


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
            inputName.DeactivateInputField();
            if (currentName != inputName.text)
            {
                OnChangedName?.Invoke(inputName.text);
                inputName.text = currentName;
            }
        }

        private StageData.Stage currentStage;

        public void SetStage(StageData.Stage stage)
        {
            currentStage = stage;
        }

        public void SetName(string name)
        {
            currentName = name;
            inputName.text = name;
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
            foreach (var user in ranking.users)
            {
                foreach (var stage in user.Stages)
                {
                    if (!rankings.ContainsKey(stage.Key))
                    {
                        rankings[stage.Key] = new List<RankingUser>();
                    }

                    rankings[stage.Key].Add(user);
                }
            }

            // 各ステージでユーザーをスコア順にソート
            foreach (var stage in rankings.Keys.ToList())
            {
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

        public void Reload()
        {
            reload.SetValidVisual(false);
            lists.Clear();
            lists.Add(rankingRow1);
            lists.Add(rankingRow2);
            lists.Add(rankingRow3);
            lists.Add(rankingRow4);
            lists.Add(rankingRow5);
            for (var i = 0; i < lists.Count; i++)
            {
                SetRankingRef(
                    lists[i],
                    i + 1,
                    new RankingUser
                    {
                        ID = "",
                        Name = "",
                        Stages = new Dictionary<StageData.Stage, int>()
                    },
                    currentStage
                );
            }

            if (!rankings.ContainsKey(currentStage) || rankings.Count == 0)
            {
                popupWindow.SetVisible(true);
                return;
            }

            List<RankingUser> currents = rankings[currentStage];
            for (var i = 0; i < currents.Count; i++)
            {
                if (currents[i].ID == userId)
                {
                    SetRankingRef(myRankingRow, i + 1, currents[i], currentStage);
                }

                if (i > lists.Count - 1)
                {
                    return;
                }

                SetRankingRef(lists[i], i + 1, currents[i], currentStage);
            }
        }

        private void Clear()
        {
            rankings.Clear();
            reload.SetValidVisual(false);
            lists.Clear();
            lists.Add(rankingRow1);
            lists.Add(rankingRow2);
            lists.Add(rankingRow3);
            lists.Add(rankingRow4);
            lists.Add(rankingRow5);
            for (var i = 0; i < lists.Count; i++)
            {
                SetRankingRef(
                    lists[i],
                    i + 1,
                    new RankingUser
                    {
                        ID = "",
                        Name = "",
                        Stages = new Dictionary<StageData.Stage, int>()
                    },
                    currentStage
                );
            }
        }
    }
}
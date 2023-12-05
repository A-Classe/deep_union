using System;
using AnimationPro.RunTime;
using Core.Model.User;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using TMPro;
using UnityEngine;

namespace Module.UI.Title.Stats
{
    public class StatsManager: UIManager
    {
        public enum Nav
        {
            Quit
        }

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private FadeInOutButton quit;
        [SerializeField] private TextMeshProUGUI assign;
        [SerializeField] private TextMeshProUGUI release;
        [SerializeField] private TextMeshProUGUI addWorker;
        [SerializeField] private TextMeshProUGUI delWorker;
        [SerializeField] private TextMeshProUGUI movePlayer;
        [SerializeField] private TextMeshProUGUI moveWorkers;
        [SerializeField] private TextMeshProUGUI play;
        [SerializeField] private TextMeshProUGUI clear;
        [SerializeField] private TextMeshProUGUI gameOver;

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

        public void SetReports(Report report)
        {
            assign.text = report.assignCount.ToString();
            release.text = report.releaseCount.ToString();
            addWorker.text = report.addWorkerCount.ToString();
            delWorker.text = report.delWorkerCount.ToString();
            movePlayer.text = ((int)report.movePlayerDistance).ToString();
            moveWorkers.text = ((int)report.moveWorkersDistance).ToString();
            play.text = report.gamePlayCount.ToString();
            clear.text = report.gameClear.ToString();
            gameOver.text = report.gameOver.ToString();
        }
    }
}
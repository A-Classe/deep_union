using System;
using Core.Model.Scene;
using Core.User;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Core.Scenes
{
    /// <summary>
    ///     シーン間を遷移する時に必ず使う
    ///     パラメーターもここで管理
    /// </summary>
    public class SceneChanger
    {
        public enum Route
        {
            Title,
            InGame,
            Result,
        }

        private Route next;
        
        private const string TitleRoute = "Scenes/Other/Titles_Test";
        private const string InGameRoute = "Scenes/Stages/Stage1hara/Stage1hara";
        // :todo チュートリアル用のシーンを用意する
        private const string TutorialRoute = "Scenes/Stages/StageTutorial/StageTutorial";
        private const string ResultRoute = "Scenes/Other/Result_Test";
        private const string BeforeInGameRoute = "Scenes/Other/BeforeInGame";
        private const string AfterInGameRoute = "Scenes/Other/AfterInGame";

        [CanBeNull] private string currentRoute;


        private StageData.Stage inGameRoute = StageData.Stage.Stage1;

        private GameResult results;

        private TitleNavigation route = TitleNavigation.Title;

        public event Action OnBeforeChangeScene;

        /// <summary>
        ///     タイトル画面への遷移
        /// </summary>
        /// <param name="routeNav"></param>
        /// <returns>遷移できなければfalseを返す</returns>
        public bool LoadTitle(TitleNavigation routeNav = TitleNavigation.Title)
        {
            OnBeforeChangeScene?.Invoke();
            route = routeNav;
            SceneManager.LoadScene(TitleRoute);
            currentRoute = TitleRoute;

            return true;
        }

        /// <summary>
        ///     タイトルに遷移する時の初期画面のnav
        /// </summary>
        /// <returns>navgation</returns>
        public TitleNavigation GetTitle()
        {
            return route;
        }

        /// <summary>
        ///     :TODO 引数見て、各stageに遷移
        /// </summary>
        /// <param name="routeNav"></param>
        /// <returns>遷移できなければfalseを返す</returns>
        public bool LoadInGame(StageData.Stage routeNav)
        {
            try
            {
                OnBeforeChangeScene?.Invoke();
                inGameRoute = routeNav;
                currentRoute = inGameRoute switch
                {
                    StageData.Stage.Stage1 => InGameRoute,
                    StageData.Stage.Tutorial => TutorialRoute,
                    _ => InGameRoute
                };
                SceneManager.LoadScene(currentRoute);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public StageData.Stage GetInGame()
        {
            return inGameRoute;
        }

        private GameResult currentResult;
        /// <summary>
        ///     Resultへの遷移
        /// </summary>
        /// <param name="param"></param>
        /// <returns>遷移できなければfalseを返す</returns>
        public bool LoadResult(
            GameResult param
        )
        {
            OnBeforeChangeScene?.Invoke();
            results = param;
            SceneManager.LoadScene(ResultRoute);
            currentRoute = ResultRoute;
            return false;
        }

        public GameResult GetResultParam()
        {
            return results;
        }
        
        
        public bool LoadBeforeMovieInGame(StageData.Stage routeNav)
        {
            try
            {
                OnBeforeChangeScene?.Invoke();
                next = Route.InGame;
                inGameRoute = routeNav;
                SceneManager.LoadScene(BeforeInGameRoute);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public bool LoadAfterMovieInGame(GameResult result)
        {
            try
            {
                OnBeforeChangeScene?.Invoke();
                next = Route.Result;
                currentResult = result;
                SceneManager.LoadScene(AfterInGameRoute);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Next()
        {
            try 
            {
                switch (next)
                {
                    case Route.Result:
                        LoadResult(currentResult);
                        break;
                    case Route.Title:
                        LoadTitle();
                        break;
                    case Route.InGame:
                        LoadInGame(inGameRoute);
                        break;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}
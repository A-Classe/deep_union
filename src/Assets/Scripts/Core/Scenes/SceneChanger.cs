using System;
using Core.Model.Scene;
using Core.User;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Core.Scenes
{
    /// <summary>
    /// シーン間を遷移する時に必ず使う
    /// パラメーターもここで管理
    /// </summary>
    public class SceneChanger
    {
        private const string TitleRoute = "Scenes/Other/Titles_Test";
        private const string InGameRoute = "Scenes/Other/InGameUI_Test/InGameUI_Test";
        private const string ResultRoute = "Scenes/Other/Result_Test";
        
        private TitleNavigation route = TitleNavigation.Title;

        [CanBeNull] private string currentRoute = null;
        
        /// <summary>
        /// タイトル画面への遷移
        /// </summary>
        /// <param name="routeNav"></param>
        /// <returns>遷移できなければfalseを返す</returns>
        public bool LoadTitle(TitleNavigation routeNav = TitleNavigation.Title)
        {
            route = routeNav;
            SceneManager.LoadScene(TitleRoute);
            currentRoute = TitleRoute;
            
            return true;
        }

        /// <summary>
        /// タイトルに遷移する時の初期画面のnav
        /// </summary>
        /// <returns>navgation</returns>
        public TitleNavigation GetTitle() => route;
        

        private StageData.Stage inGameRoute = StageData.Stage.Stage1;
        /// <summary>
        /// :TODO 引数見て、各stageに遷移
        /// </summary>
        /// <param name="routeNav"></param>
        /// <returns>遷移できなければfalseを返す</returns>
        public bool LoadInGame(StageData.Stage routeNav)
        {
            try
            {
                inGameRoute = routeNav;
                SceneManager.LoadScene(InGameRoute);
                currentRoute = InGameRoute;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        
        public StageData.Stage GetInGame() => inGameRoute;

        private GameResult results;
        /// <summary>
        /// Resultへの遷移
        /// </summary>
        /// <param name="param"></param>
        /// <returns>遷移できなければfalseを返す</returns>
        public bool LoadResult(
            GameResult param
        )
        {
            results = param;
            SceneManager.LoadScene(ResultRoute);
            currentRoute = ResultRoute;
            return false;
        }

        // private void CurrentUnload()
        // {
        //     if (currentRoute != null)
        //     {
        //         SceneManager.UnloadSceneAsync(currentRoute);
        //     }
        // }

        public GameResult GetResultParam() => results;
    }
}
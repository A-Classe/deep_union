using Core.Model.Scene;
using GameMain.System.Scenes;
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
        
        private TitleNavigation route = TitleNavigation.Title;
        
        /// <summary>
        /// タイトル画面への遷移
        /// </summary>
        /// <param name="routeNav"></param>
        /// <returns>遷移できなければfalseを返す</returns>
        public bool LoadTitle(TitleNavigation routeNav = TitleNavigation.Title)
        {
            route = routeNav;
            SceneManager.LoadScene(TitleRoute);
            return true;
        }

        /// <summary>
        /// タイトルに遷移する時の初期画面のnav
        /// </summary>
        /// <returns>navgation</returns>
        public TitleNavigation GetTitle() => route;
        

        private StageNavigation inGameRoute = StageNavigation.Stage1;
        /// <summary>
        /// :TODO 引数見て、各stageに遷移
        /// </summary>
        /// <param name="routeNav"></param>
        /// <returns>遷移できなければfalseを返す</returns>
        public bool LoadInGame(StageNavigation routeNav)
        {
            inGameRoute = routeNav;
            SceneManager.LoadScene(InGameRoute);
            return true;
        }

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
            SceneManager.LoadScene("");
            return false;
        }

        public GameResult GetResultParam() => results;
    }
}
namespace Core.Model.Scene
{
    public struct GameResult
    {
        public int WorkerCount;

        public int Hp;

        public int Resource;

        public int stageCode;
    }

    public static class GameResultEx
    {
        public static uint GetScore(this GameResult result)
        {
            /* todo: スコアを計算する */
            return (uint)(result.WorkerCount + result.Hp + result.Resource);
        }
    }
}
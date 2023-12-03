using System;
using System.Collections.Generic;

namespace Core.Model.User
{
    [Serializable]
    public struct Report: IDefaultable<Report>
    {
        public uint assignCount;
        
        public uint releaseCount;

        public uint addWorkerCount;

        public uint delWorkerCount;

        public float moveWorkersDistance;

        public float movePlayerDistance;

        public uint gamePlayCount;

        public uint gameClear;

        public uint gameOver;
        
        public static Report GenerateToEvents(LinkedList<GameEvent> events)
        {
            Report report = new Report();
            foreach (GameEvent gameEvent in events)
            {
                switch (gameEvent.EventType)
                {
                    case GameEventType.Assign:
                        report.assignCount++;
                        break;
                    case GameEventType.Release:
                        report.releaseCount++;
                        break;
                    case GameEventType.AddWorker:
                        report.addWorkerCount++;
                        break;
                    case GameEventType.DelWorker:
                        report.delWorkerCount++;
                        break;
                    case GameEventType.MovePlayer:
                        report.movePlayerDistance +=
                            TryFloat(gameEvent.GetParameter(MovePlayer.ParamType.Distance.ToString()));
                        break;
                    case GameEventType.MoveWorkers:
                        report.moveWorkersDistance +=
                            TryFloat(gameEvent.GetParameter(MoveWorkers.ParamType.Distance.ToString()));
                        break;
                    case GameEventType.GamePlay:
                        report.gamePlayCount++;
                        break;
                    case GameEventType.GameOver:
                        report.gameOver++;
                        break;
                    case GameEventType.GameClear:
                        report.gameClear++;
                        break;
                }
            }

            return report;
        }

        private static float TryFloat(object value)
        {
            try
            {
                return Convert.ToSingle(value);
            }
            catch (Exception)
            {
                return 0f;
            }
        }

        public Report DefaultInstance()
        {
            return new Report();
        }

        public static Report operator +(Report a, Report b)
        {
            return new Report
            {
                assignCount = a.assignCount + b.assignCount,
                releaseCount = a.releaseCount + b.releaseCount,
                addWorkerCount = a.addWorkerCount + b.addWorkerCount,
                delWorkerCount = a.delWorkerCount + b.delWorkerCount,
                movePlayerDistance = a.movePlayerDistance + b.movePlayerDistance,
                moveWorkersDistance = a.moveWorkersDistance + b.moveWorkersDistance,
                gamePlayCount = a.gamePlayCount + b.gamePlayCount,
                gameClear = a.gameClear + b.gameClear,
                gameOver = a.gameOver + b.gameOver
            };
        }
    }
}
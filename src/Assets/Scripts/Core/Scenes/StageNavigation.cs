using System;
using Core.User;
using UnityEngine;

namespace Core.Scenes
{
    
    public enum StageNavigation
    {
        Tutorial,
        Stage1,
        Stage1Ranking,
        Back
    }

    public static class StageNavEx
    {
        public static StageData.Stage ToStage(this StageNavigation nav)
        {
            return nav switch
            {
                StageNavigation.Stage1 => StageData.Stage.Stage1,
                StageNavigation.Tutorial => StageData.Stage.Tutorial,
                _ => StageData.Stage.Stage1
            };
        }

        public static StageNavigation Move(this StageNavigation nav, float x, float y)
        {
            if (Math.Abs(x) > Math.Abs(y))
            {
                return x switch
                {
                    > 0 => nav.ToRanking(),
                    < 0 => nav.ToBase(),
                    _ => nav
                };
            }

            StageNavigation nextNav = nav.ToBase();
            switch (y)
            {
                // 上向きの入力
                case > 0:
                    nextNav = nextNav switch
                    {
                        StageNavigation.Back => StageNavigation.Stage1,
                        StageNavigation.Stage1 => StageNavigation.Tutorial,
                        _ => nextNav
                    };
                    break;
                // 下向きの入力
                case < 0:
                    nextNav = nextNav switch
                    {
                        StageNavigation.Tutorial => StageNavigation.Stage1,
                        StageNavigation.Stage1 => StageNavigation.Back,
                        _ => nextNav
                    };
                    break;
            }
            return nextNav;
        }

        public static StageNavigation ToggleOpt(this StageNavigation nav)
        {
            return nav switch
            {
                StageNavigation.Stage1 => StageNavigation.Stage1Ranking,
                StageNavigation.Stage1Ranking => StageNavigation.Stage1,
                _ => nav
            };
        }
        
        public static StageNavigation ToBase(this StageNavigation nav)
        {
            return nav switch
            {
                StageNavigation.Stage1Ranking => StageNavigation.Stage1,
                _ => nav
            };
        }

        public static StageNavigation ToRanking(this StageNavigation nav)
        {
            return nav switch
            {
                StageNavigation.Stage1 => StageNavigation.Stage1Ranking,
                _ => nav
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Utility.User
{
    [Serializable]
    public class KeyValue<TKey, TValue>
    {
        public TKey Key { get; private set; }
        public TValue value;

        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            this.value = value;
        }
    }

    [Serializable]
    public class UserData: IDefaultable<UserData>
    {
        public static readonly Expression<Func<UserData, object>> FullScreen = data => data.fullScreen;
        public KeyValue<string, bool> fullScreen = new("fullScreen", true);
        public static readonly Expression<Func<UserData, object>> BrightVal = data => data.bright;
        public KeyValue<string, int> bright = new("brightVal", 0);
        public static readonly Expression<Func<UserData, object>> MasterVol = data => data.masterVolume;
        public KeyValue<string, int> masterVolume = new("masterVol", 0);
        public static readonly Expression<Func<UserData, object>> MusicVol = data => data.musicVolume;
        public KeyValue<string, int> musicVolume = new("musicVol", 0);
        public static readonly Expression<Func<UserData, object>> EffectVol = data => data.effectVolume;
        public KeyValue<string, int> effectVolume = new("musicVol", 0);

        public UserData DefaultInstance()
        {
            return new UserData();
        }
    }

    [Serializable]
    public class StageData: IDefaultable<StageData>
    {
        public Dictionary<Stage, uint> stage = new ();
        public StageData DefaultInstance()
        {
            return new StageData();
        }
        
        public enum Stage
        {
            Stage1,
            Stage2,
            Stage3,
            Stage4,
            Stage5
        }
    }
    
    
    [Serializable]
    public class SerializableDictionary
    {
        public List<string> keys = new();
        public List<string> values = new();

        public SerializableDictionary() { }

        public SerializableDictionary(Dictionary<string, string> dict)
        {
            foreach (var pair in dict)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
            {
                dict[keys[i]] = values[i];
            }
            return dict;
        }
    }
}
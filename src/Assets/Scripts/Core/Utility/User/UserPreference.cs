using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;

namespace Core.Utility.User
{
    public class UserPreference
    {
        private static readonly string RootPath = Application.persistentDataPath;
        private static readonly string SaveDirPath = Path.Combine(RootPath, "preference");
        private readonly string saveFilePath = "preference.json";

        private readonly DataManager dataManager = new();

        public UserPreference()
        {
            EnsureDirectoryExists();
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(SaveDirPath))
            {
                Directory.CreateDirectory(SaveDirPath);
            }
        }

        public void Load()
        {
            dataManager.Load(GetFilePath());
        }
        
        public UserData GetUserData()
        {
            return dataManager.Get<UserData>();
        }

        public void SaveUserData(UserData data)
        {
            dataManager.Set(data);
            dataManager.Save();
        }

        public Dictionary<StageData.Stage, uint> GetStageData()
        {
            return dataManager.Get<StageData>().stage;
        }

        public void SaveStageData(StageData data)
        {
            dataManager.Set(data);
            dataManager.Save();
        }
        
        public void SaveStageData(StageData.Stage stage, uint score)
        {
            var data = dataManager.Get<StageData>();
            data.stage[stage] = score;
            dataManager.Set(data);
            dataManager.Save();
        }

        public void Delete()
        {
            dataManager.Delete(GetFilePath());
        }

        public TValue GetSpecificField<T, TValue>(Expression<Func<T, object>> selector) where T : IDefaultable<T>, new()
        {
            return dataManager.GetSpecificField<T, TValue>(selector);
        }

        private string GetFilePath()
        {
            return Path.Combine(SaveDirPath, saveFilePath);
        }
    }
}
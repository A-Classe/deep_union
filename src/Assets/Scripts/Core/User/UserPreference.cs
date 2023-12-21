using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Core.Model.User;
using UnityEngine;

namespace Core.User
{
    /// <summary>
    ///     データ保存の中継クラス
    ///     絶対にこれを介して保存する
    /// </summary>
    public class UserPreference
    {
        private static readonly string RootPath = Application.persistentDataPath;
        private static readonly string SaveDirPath = Path.Combine(RootPath, "preference");

        private readonly DataManager dataManager = new();
        private readonly string saveFilePath = "preference.json";

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

        public void SetUserData(UserData data)
        {
            dataManager.Set(data);
        }

        public void SetMasterVolume(int vol)
        {
            var data = dataManager.Get<UserData>();
            data.masterVolume.value = vol;
            dataManager.Set(data);
        }

        public void SetMusicVolume(int vol)
        {
            var data = dataManager.Get<UserData>();
            data.musicVolume.value = vol;
            dataManager.Set(data);
        }

        public void SetEffectVolume(int vol)
        {
            var data = dataManager.Get<UserData>();
            data.effectVolume.value = vol;
            dataManager.Set(data);
        }

        /// <summary>
        ///     例外的にここだけ必須でセーブしておく
        /// </summary>
        public void CompletedFirst()
        {
            var data = dataManager.Get<UserData>();
            data.isFirst.value = true;
            dataManager.Set(data);
            Save();
        }

        /// <summary>
        ///     デバッグ用
        ///     todo: そのうち消したい
        /// </summary>
        public void ResetIsFirst()
        {
            var data = dataManager.Get<UserData>();
            data.isFirst.value = false;
            dataManager.Set(data);
            Save();
        }

        public bool GetIsFirst()
        {
            return dataManager.Get<UserData>().isFirst.value;
        }

        public Dictionary<StageData.Stage, uint> GetStageData()
        {
            return dataManager.Get<StageData>().stage;
        }

        public void SetStageData(StageData data)
        {
            dataManager.Set(data);
        }

        public void SetStageData(StageData.Stage stage, uint score)
        {
            var data = dataManager.Get<StageData>();
            data.stage[stage] = score;
            dataManager.Set(data);
        }

        public void Delete()
        {
            dataManager.Delete(GetFilePath());
        }

        public void Save()
        {
            dataManager.Save();
        }

        public TValue GetSpecificField<T, TValue>(Expression<Func<T, object>> selector) where T : IDefaultable<T>, new()
        {
            return dataManager.GetSpecificField<T, TValue>(selector);
        }

        private string GetFilePath()
        {
            return Path.Combine(SaveDirPath, saveFilePath);
        }

        public Report GetReport()
        {
            return dataManager.Get<Report>();
        }

        public void SetReport(Report reporter)
        {
            dataManager.Set(reporter);
        }
    }
}
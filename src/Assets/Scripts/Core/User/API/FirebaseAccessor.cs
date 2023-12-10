using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using VContainer;

namespace Core.User.API
{
    public class FirebaseAccessor
    {
        private DatabaseReference reference;

        private UserPreference preference;

        private UserData userData;

        /// <summary>
        /// -- Sample
        ///  --- Ranking:
        ///   --- UUID: string
        ///    --- Name: string
        ///    --- Stage: Dictionary
        ///     --- Tutorial: int
        ///     --- Stage1: int
        ///     --- Stage2: int
        /// </summary>
        private const string Root = "Sample";

        private const string Ranking = "Ranking";
        
        private const string Uuid = "UUID";

        private const string Name = "Name";

        private const string Stage = "Stage";
        
        

        [Inject]
        public FirebaseAccessor(
            UserPreference preference
        )
        {
            this.preference = preference;
            preference.Load();
            reference = FirebaseDatabase.DefaultInstance.RootReference;
            userData = preference.GetUserData();
            if (userData.uuid.value == "")
            {
                WriteNewUser();
            }
        }
        private void WriteNewUser()
        {
       
            userData.uuid.value = Guid.NewGuid().ToString();
            Debug.Log("generate UUID: " + userData.uuid.value);
            preference.SetUserData(userData);
            preference.Save();
            preference.Load();
            userData = preference.GetUserData();
        }

        private DatabaseReference GetUserRef()
        {
            return GetDbRef().Child(userData.uuid.value);
        }

        private DatabaseReference GetDbRef()
        {
            return reference.Child(Root).Child(Ranking);
        }

        public void SetName(string name)
        {
            GetUserRef().Child(Name).SetValueAsync(name);
        }

        public void SetStageScore(StageData.Stage stage, uint score)
        {
            Debug.Log(stage.ToString() + "==" + score);
            GetUserRef().Child(Stage).Child(stage.ToString()).SetValueAsync(score);
        }

        public void GetAllData(Action<Ranking> callback)
        {
            // sec
            int timeoutSeconds = 10;
            Ranking ranking = new Ranking
            {
                users = new List<RankingUser>()
            };

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("Not Connection Network");
                callback?.Invoke(ranking);
                return;
            }

            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                Task.Run(async () =>
                {

                    var databaseTask = GetDbRef().GetValueAsync();
                    var delayTask = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds), cts.Token);

                    var completedTask = await Task.WhenAny(databaseTask, delayTask);

                    if (completedTask == delayTask && !databaseTask.IsCompleted)
                    {
                        Debug.Log("Operation timed out");
                        cts.Cancel();
                        callback?.Invoke(ranking);
                        return;
                    }
                    cts.Cancel();

                    if (databaseTask.IsFaulted || databaseTask.IsCanceled)
                    {
                        Debug.Log("Error in database operation");
                        callback?.Invoke(ranking);
                        return;
                    }

                    Debug.Log("Database operation completed");
                    DataSnapshot snapshot = databaseTask.Result;
                    foreach (var userData in snapshot.Children)
                    {
                        if (userData.Key == Uuid) continue;
                        RankingUser user = new RankingUser();
                        // UUID: string
                        user.ID = userData.Key;
                        user.Stages = new();
                        foreach (var parameters in userData.Children)
                        {
                            if (parameters.Key == Name)
                            {
                                // Name: string
                                user.Name = parameters.Value.ToString();
                            } else if (parameters.Key == Stage)
                            {
                                foreach (var stages in parameters.Children)
                                {
                                    // Map <Stage: string to Score: int>
                                    StageData.Stage stg = (StageData.Stage)Enum.Parse(typeof(StageData.Stage), stages.Key);
                                    user.Stages[stg] = int.Parse(stages.Value.ToString());
                                }
                            }
                        }
                        ranking.users.Add(user);
                    }
                    callback?.Invoke(ranking);
                }, cts.Token).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        // 何らかのエラーが発生した場合の処理
                        Debug.Log(task.Exception);
                    }
                    cts.Dispose();
                }, cts.Token);
            } 
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                throw;
            }
        }
    }
}
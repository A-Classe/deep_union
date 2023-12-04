using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Model.User;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.User.API
{
    public class FirebaseAccessor : IStartable
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
            // Debug.Log(reference);
            userData = preference.GetUserData();
            if (userData.uuid.value == "")
            {
                WriteNewUser();
            }
        }
        public void Start()
        {
            FirebaseDatabase.DefaultInstance.GetReference(".info/connected").ValueChanged += (sender, args) => {
                if (args.DatabaseError != null) {
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }

                if (args.Snapshot.Value is bool connected && connected)
                {
                    // データベースに接続されている
                    Debug.Log("Connected");
                }
                else
                {
                    // データベースに接続されていない
                    Debug.Log("Disconnected");
                }
            };
        }

        private void WriteNewUser()
        {
       
            userData.uuid.value = System.Guid.NewGuid().ToString();
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

        public void GetAllData(Action<Ranking> OnCallback)
        {
            Ranking ranking = new Ranking();
            ranking.users = new List<RankingUser>();
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("Not Connection Network");
                OnCallback?.Invoke(ranking);
            }
            GetDbRef()
                .GetValueAsync()
                .ContinueWithOnMainThread(task => {
                    if (task.IsFaulted || task.IsCanceled) {
                        // Handle the error...
                        Debug.Log("catch Error:");
                        Debug.Log(task);
                        OnCallback?.Invoke(ranking);
                    }
                    else if (task.IsCompleted) {
                        try
                        {
                            DataSnapshot snapshot = task.Result;
                            // Do something with snapshot...
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
                            
                            OnCallback?.Invoke(ranking);
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e.ToString());
                        }
                    }
                });
        }
        
    }
}
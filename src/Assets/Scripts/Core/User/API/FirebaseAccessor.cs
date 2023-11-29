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
            WriteNewUser();
        }
        public void Start()
        {
 
        }

        private void WriteNewUser()
        {
            if (userData.uuid.value == "")
            {
                userData.uuid.value = System.Guid.NewGuid().ToString();
                Debug.Log("generate UUID: " + userData.uuid.value);
                preference.SetUserData(userData);
                preference.Save();
                preference.Load();
                userData = preference.GetUserData();
            }

            SetName(userData.name.value);
            SetStageScore(StageData.Stage.Stage1, 3000);
            GetAllData();
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

        public void SetStageScore(StageData.Stage stage, int score)
        {
            GetUserRef().Child(Stage).Child(stage.ToString()).SetValueAsync(score);
        }

        public void GetAllData()
        {
            GetDbRef()
                .GetValueAsync().ContinueWithOnMainThread(task => {
                    if (task.IsFaulted) {
                        // Handle the error...
                        Debug.Log(task);
                    }
                    else if (task.IsCompleted) {
                        DataSnapshot snapshot = task.Result;
                        // Do something with snapshot...
                        foreach (var VARIABLE in snapshot.Children)
                        {
                            if (VARIABLE.Key == Uuid) continue;
                            
                            Debug.Log("UUID: " + VARIABLE.Key);
                            foreach (var value in VARIABLE.Children)
                            {
                                if (value.Key == Name)
                                {
                                    Debug.Log(value.Key + ":" + value.Value);
                                } else if (value.Key == Stage)
                                {
                                    foreach (var child in value.Children)
                                    {
                                        Debug.Log(child.Key + ":" + child.Value);
                                    }
                                }
                            }
                        }
                    }
                });
        }
        
    }
}
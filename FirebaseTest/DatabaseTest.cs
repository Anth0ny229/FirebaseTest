using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Util;
using Firebase;
using Firebase.Database;

namespace FirebaseTest
{
    [Activity(Label = "DatabaseTest", MainLauncher = true)]
    public class DatabaseTest : Activity, IValueEventListener
    {
        private const string Tag = "DatabaseTest";
        private DatabaseReference mDatabase;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Setup our firebase options then init
            FirebaseOptions o = new FirebaseOptions.Builder()
                .SetApiKey(GetString(Resource.String.ApiKey))
                .SetApplicationId(GetString(Resource.String.ApplicationId))
                .SetDatabaseUrl(GetString(Resource.String.DatabaseUrl))
                .Build();
            FirebaseApp fa = FirebaseApp.InitializeApp(this, o, Application.PackageName);

            // Get a database reference
            var db = FirebaseDatabase.GetInstance(fa);
            mDatabase = db.GetReference("favorites");
            mDatabase.AddListenerForSingleValueEvent(this);

            mDatabase.SetValue("Hello, World!");
        }

        private void SubmitPost()
        {
           // string userId = GetUid();
            mDatabase.AddListenerForSingleValueEvent(this);
        }

        public void OnDataChange(DataSnapshot dataSnapshot)
        {
            Log.Debug(Tag, "Database Change: " + dataSnapshot.Value);
        }

        public void OnCancelled(DatabaseError databaseError)
        {
            Log.Wtf(Tag, "getUser:onCancelled", databaseError.ToException());
        }
    }
}

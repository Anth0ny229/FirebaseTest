using System.Diagnostics;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Iid;
using Firebase.Messaging;

//using Firebase.Messaging;

namespace FirebaseTest
{
    [Activity(Label = "FcmTest")]
    public class FcmTest : Activity, View.IOnClickListener
    {
        public const string Tag = "FcmTest";

        #region View Controls
#pragma warning disable 649
        private Button btnSubscribe, btnLogToken;
#pragma warning restore 649
        #endregion

        private FirebaseInstanceId mInstanceId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "Fcm" layout resource
            SetContentView(Resource.Layout.Fcm);
            this.WireUpViews();
            btnSubscribe.SetOnClickListener(this);
            btnLogToken.SetOnClickListener(this);

            // Setup our firebase options then init
            FirebaseOptions o = new FirebaseOptions.Builder()
                .SetApiKey(GetString(Resource.String.ApiKey))
                .SetApplicationId(GetString(Resource.String.ApplicationId))
                .SetDatabaseUrl(GetString(Resource.String.DatabaseUrl))
                .SetGcmSenderId(GetString(Resource.String.SenderId))
                .Build();
            FirebaseApp fa = FirebaseApp.InitializeApp(this, o);

            // Get our instance ID
            mInstanceId = FirebaseInstanceId.GetInstance(fa);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.btnSubscribe: Subscribe(); break;
                case Resource.Id.btnLogToken: Task.Factory.StartNew(LogToken); break;
                default:

                    // Not handled or unkonwn
                    Log.Debug(Tag, "OnClick:" + v.Id);
                    Debugger.Break();
                    break;
            }
        }

        private void Subscribe()
        {
            FirebaseMessaging.Instance.SubscribeToTopic("news");
            Log.Debug(Tag, "Subscribed to news topic");
        }
        private void LogToken()
        {
            var token2 = mInstanceId.Token;
            var token = mInstanceId.GetToken(GetString(Resource.String.SenderId), "FCM");
            Log.Debug(Tag, "Firebase Token : " + token + " token 2" + token2);
        }
    }

    [Service(Exported = true), IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseInstanceIdService : FirebaseInstanceIdService
    {
        public override void OnTokenRefresh()
        {
            string refreshedToken = FirebaseInstanceId.Instance.Token;
            if (!string.IsNullOrEmpty(refreshedToken))
            {
                System.Console.WriteLine("Refreshed Firebase Token : " + refreshedToken);
                SendRegistrationToken(refreshedToken);
            }

            base.OnTokenRefresh();
        }

        private void SendRegistrationToken(string token)
        {
            // TODO: Function to store token / send to server
        }
    }

    [Service(Exported = true), IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            string title = message.GetNotification().Title;
            string text = message.GetNotification().Body;
            string image = message.GetNotification().Icon;
            string sound = message.GetNotification().Sound;

            Log.Debug(FcmTest.Tag, "From: " + message.From);
            Log.Debug(FcmTest.Tag, "Notification Message Body: " + message.GetNotification().Body);

            base.OnMessageReceived(message);
        }

        public override void OnMessageSent(string msgId)
        {
            base.OnMessageSent(msgId);
        }
    }
}
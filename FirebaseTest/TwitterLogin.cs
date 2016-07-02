using System;
using System.Diagnostics;
using Android.App;
using Android.Gms.Tasks;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Xamarin.Auth;

namespace FirebaseTest
{
    [Activity(Label = "TwitterLogin")]
    public class TwitterLogin : Activity, FirebaseAuth.IAuthStateListener, View.IOnClickListener
    {
        private const string Tag = "Twitter";
        private FirebaseAuth mAuth;

        #region View Controls
#pragma warning disable 649
        private Button btnSignIn, btnSignOut, btnRevokeAccess;
        private TextView textViewStatus, textViewDetail;
#pragma warning restore 649
        #endregion

        private void LoginTwitter(bool allowCancel = true)
        {
            var auth = new OAuth1Authenticator(
                consumerKey: GetString(Resource.String.TwitterConsumerKey),
                consumerSecret: GetString(Resource.String.TwitterConsumerSecret),
                requestTokenUrl: new Uri(GetString(Resource.String.TwitterRequestTokenUrl)),
                authorizeUrl: new Uri(GetString(Resource.String.TwitterAuthorizeUrl)),
                accessTokenUrl: new Uri(GetString(Resource.String.TwitterAccessTokenUrl)),
                callbackUrl: new Uri(GetString(Resource.String.TwitterCallbackUrl))
                )
            { AllowCancel = allowCancel };
            auth.Completed += TwitterAuthComplete;

            StartActivity(auth.GetUI(this));
        }

        private void TwitterAuthComplete(object sender, AuthenticatorCompletedEventArgs e)
        {
            Log.Debug(Tag, "TwitterAuthComplete:" + e.IsAuthenticated);

            if (e.IsAuthenticated)
            {
                var token = e.Account.Properties["oauth_token"];
                var secret = e.Account.Properties["oauth_token_secret"];

                AuthCredential credential = TwitterAuthProvider.GetCredential(token, secret);
                mAuth.SignInWithCredential(credential);
            }
        }

        public void OnComplete(Task task)
        {
            Log.Debug(Tag, "SignInWithCredential:OnComplete:" + task.IsSuccessful);

            // If sign in fails, display a message to the user. If sign in succeeds
            // the auth state listener will be notified and logic to handle the
            // signed in user can be handled in the listener.
            if (!task.IsSuccessful)
            {
                Log.Wtf(Tag, "SignInWithCredential", task.Exception);
                Toast.MakeText(this, "Authentication failed.", ToastLength.Long).Show();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "Login" layout resource
            SetContentView(Resource.Layout.Login);

            // Call to "wire up" all our controls autoamticlly
            this.WireUpViews();
            btnSignIn.SetOnClickListener(this);
            btnSignOut.SetOnClickListener(this);
            btnRevokeAccess.SetOnClickListener(this);

            // Setup our firebase options then init
            FirebaseOptions o = new FirebaseOptions.Builder()
                .SetApiKey(GetString(Resource.String.ApiKey))
                .SetApplicationId(GetString(Resource.String.ApplicationId))
                .SetDatabaseUrl(GetString(Resource.String.DatabaseUrl))
                .Build();
            FirebaseApp fa = FirebaseApp.InitializeApp(this, o, Application.PackageName);

            // Get the auth instance so we can add to it
            mAuth = FirebaseAuth.GetInstance(fa);
        }

        protected override void OnStart()
        {
            base.OnStart();
            mAuth.AddAuthStateListener(this);
        }

        protected override void OnStop()
        {
            base.OnStop();
            mAuth.RemoveAuthStateListener(this);
        }

        public void OnAuthStateChanged(FirebaseAuth auth)
        {
            var user = auth.CurrentUser;
            Log.Debug(Tag, "onAuthStateChanged:" + (user != null ? "signed_in:" + user.Uid : "signed_out"));
            UpdateUi(user);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.btnSignIn: LoginTwitter(); break;
                case Resource.Id.btnSignOut: SignOut(); break;
                case Resource.Id.btnRevokeAccess: RevokeAccess(); break;
                default:

                    // Not handled or unkonwn
                    Log.Debug(Tag, "OnClick:" + v.Id);
                    Debugger.Break();
                    break;
            }
        }

        private void SignOut()
        {
            // Firebase sign out
            mAuth.SignOut();
        }

        private void RevokeAccess()
        {
            // Firebase sign out
            mAuth.SignOut();
        }

        private void UpdateUi(FirebaseUser user = null)
        {
            // Check if null so we don't have to rewrite everything twice
            var b = user != null;
            textViewStatus.SetText(b ? user.Email ?? "No Email" : "Signed Out");
            textViewDetail.SetText(b ? user.Uid : "");
            btnSignIn.Visibility = b ? ViewStates.Gone : ViewStates.Visible;
            btnSignOut.Visibility = b ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}
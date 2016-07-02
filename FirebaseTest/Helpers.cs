using System;
using System.Linq;
using System.Reflection;
using Android.App;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace FirebaseTest
{
    public static class ActivityExtensions
    {
        public static void WireUpViews(this FragmentActivity activity)
        {
            WireUpViews(activity as Activity);
        }
        public static void WireUpViews(this Activity activity)
        {
            //Get all the View fields from the activity
            var members = from m in activity.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                          where m.FieldType.IsSubclassOf(typeof(View))
                          select m;

            members.ToList().ForEach(m =>
            {
                try
                {
                    //Find the android identifier with the same name
                    var id = activity.Resources.GetIdentifier(m.Name, "id", activity.PackageName);

                    //Set the activity field's value to the view with that identifier
                    m.SetValue(activity, activity.FindViewById(id));
                }
                catch (Exception ex) { throw new MissingFieldException("Failed to wire up the field " + m.Name + " to a View in your layout with a corresponding identifier", ex); }
            });
        }

        public static void SetText(this TextView view, string text) { if (text != null) view.SetText(text.ToCharArray(), 0, text.Length); }

        public static string Rs(this Activity activity, string stringName)
        {
            //Get all the View fields from the activity
            int resourceId = (int)typeof(Resource.String).GetFields(BindingFlags.Public).First(x => x.Name == stringName).GetRawConstantValue();
            return activity.GetString(resourceId);
        }

        public static string Rs(this Activity activity, int stringId) { return activity.GetString(stringId); }
    }
}
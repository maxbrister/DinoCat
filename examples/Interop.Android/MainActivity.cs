using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using DinoCat.Android;
using DinoCat.Elements;
using static DinoCat.Elements.Factories;
using static System.Diagnostics.Debug;

namespace Interop.Android
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Host host = new(BaseContext);
            host.RootElement = () => Column(
                HorizontalAlignment.Center,
                Text("Hello World").Margin(3),
                Button("Click Me (not working yet)", () => WriteLine("zzz"))
                );
            SetContentView(host);

            // Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.activity_main);
        }
    }
}
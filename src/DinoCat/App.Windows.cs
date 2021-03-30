#if WINDOWS
using DinoCat.Elements;
using DinoCat.Wpf;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace DinoCat
{
    public static class App
    {
        public static void Run(Func<Element> root)
        {
            try
            {
                // Per monitor DPI aware
                SetProcessDpiAwarenessContext(new(2));
            }
            catch (EntryPointNotFoundException)
            {
                try
                {
                    // Regular DPI aware
                    SetProcessDPIAware();
                }
                catch (EntryPointNotFoundException)
                { }
            }

            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                Host host = new() { RootElement = root };
                Window window = new()
                {
                    Content = host,
                    Title = AppDomain.CurrentDomain.FriendlyName
                };
                window.ShowDialog();
                return;
            }

            // Wpf requires STA and we can't change the main thread's apartment state.
            // To reduce the amount of code users have to write in Main, just create a new thread.
            // The plan is to eventually move away from Wpf for window hosting, but for now it's an
            // easy way to get input/accessiblity in standalone apps.
            Thread appThread = new(() => Run(root));
            appThread.Name = "App Thread";
            appThread.SetApartmentState(ApartmentState.STA);
            appThread.Start();
            appThread.Join();
        }

        [DllImport("User32.dll")]
        static extern bool SetProcessDpiAwarenessContext(IntPtr value);

        [DllImport("User32.dll", SetLastError= true)]
        static extern bool SetProcessDPIAware();
    }
}
#endif
using PMCBackend.DropBox;
using System;

namespace PMCBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Run();
            _ = Console.ReadLine();
        }

        public static async void Run()
        {
            var appKey = Properties.Settings.Default.AppKey;
            var appSecret = Properties.Settings.Default.AppSecret;
            var accessToken = Properties.Settings.Default.AccessToken;
            var dropBoxClient = new DropBoxClient(appKey, appSecret).SetAccessToken(accessToken);
            try
            {
                var file = await dropBoxClient.ListFolder();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}

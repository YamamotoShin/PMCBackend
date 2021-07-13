using System;
using System.Threading.Tasks;

namespace PMCBackend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                _ = await Task.Run(() => true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            _ = Console.ReadLine();
        }
    }
}

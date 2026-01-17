using System;
using System.Threading.Tasks;
using FinancialPlanner.ConsoleApp.Services;
using FinancialPlanner.ConsoleApp.UI;

namespace FinancialPlanner.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            var app = new ConsoleApplication();
            await app.RunAsync();
        }
    }
}

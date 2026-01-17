using System;
using System.Threading.Tasks;
using FinancialPlanner.ConsoleApp.Services;
using FinancialPlanner.ConsoleApp.Menus;

namespace FinancialPlanner.ConsoleApp.UI
{
    public class ConsoleApplication
    {
        private readonly DataService _dataService;
        private readonly CurrencyService _currencyService;
        private readonly ConsoleRenderer _renderer;
        private readonly MainMenu _mainMenu;

        public ConsoleApplication()
        {
            _dataService = new DataService();
            _currencyService = new CurrencyService();
            _renderer = new ConsoleRenderer();
            _mainMenu = new MainMenu(_dataService, _currencyService, _renderer);
        }

        public async Task RunAsync()
        {
            _renderer.ShowWelcome();
            await Task.Delay(2000);
            
            while (true)
            {
                try
                {
                    _renderer.Clear();
                    _mainMenu.Show();
                    var choice = _mainMenu.GetChoice();
                    
                    if (choice == "0")
                    {
                        _renderer.Write("До свидания! ✨", ConsoleColor.Cyan);
                        break;
                    }

                    await _mainMenu.Handle(choice);
                    _renderer.Write("\nНажмите любую клавишу...", ConsoleColor.Gray);
                    Console.ReadKey(true);
                }
                catch (Exception ex)
                {
                    _renderer.Error($"Ошибка: {ex.Message}");
                    Console.ReadKey(true);
                }
            }
        }
    }
}

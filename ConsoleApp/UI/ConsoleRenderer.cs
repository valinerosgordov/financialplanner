using System;
using System.Linq;
using FinancialPlanner.Models;

namespace FinancialPlanner.ConsoleApp.UI
{
    public class ConsoleRenderer
    {
        public void Clear()
        {
            Console.Clear();
            Console.ResetColor();
        }

        public void ShowWelcome()
        {
            Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════╗
║                                                              ║
║     ✨  ФИНАНСОВЫЙ ПЛАНИРОВЩИК - ANIME EDITION  ✨           ║
║                                                              ║
║              💰 Daily Tracker & Level System 💰             ║
║                                                              ║
╚══════════════════════════════════════════════════════════════╝
");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Загрузка данных...");
            Console.ResetColor();
        }

        public void Header(string title)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n{new string('═', 70)}");
            Console.WriteLine($"  {title}");
            Console.WriteLine($"{new string('═', 70)}\n");
            Console.ResetColor();
        }

        public void Menu(string[] options)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("\n" + new string('─', 50));
            Console.ForegroundColor = ConsoleColor.White;
            
            for (int i = 0; i < options.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"[{i + 1}] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(options[i]);
            }
            
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(new string('─', 50));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nВыберите опцию: ");
            Console.ResetColor();
        }

        public void Write(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public void Error(string msg) => Write($"❌ {msg}", ConsoleColor.Red);
        public void Success(string msg) => Write($"✅ {msg}", ConsoleColor.Green);
        public void Warning(string msg) => Write($"⚠️  {msg}", ConsoleColor.Yellow);

        public void ShowTransaction(Transaction t, int index = -1)
        {
            var color = t.Type == TransactionType.Income ? ConsoleColor.Green : ConsoleColor.Red;
            var icon = t.Type == TransactionType.Income ? "💰" : "💸";
            var prefix = index > 0 ? $"{index}. " : "";

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write($"{prefix}┌─ ");
            Console.ForegroundColor = color;
            Console.Write($"{icon} {t.Description}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ─┐");
            
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("│  ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Категория: {t.Category}");
            var pad = Math.Max(0, 30 - t.Category.Length);
            Console.Write(new string(' ', pad));
            Console.ForegroundColor = color;
            Console.Write($"{t.Amount:N2} {t.Currency}");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(" │");
            
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("│  ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"Дата: {t.Date:dd.MM.yyyy HH:mm}");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(new string(' ', 25) + "│");
            Console.WriteLine("└" + new string('─', 68) + "┘");
            Console.ResetColor();
        }

        public void ShowLevel(LevelSystem level)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n" + new string('═', 70));
            Console.WriteLine("  ⚡ HUNTER LEVEL SYSTEM ⚡");
            Console.WriteLine(new string('═', 70) + "\n");
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  Уровень: {level.Level}");
            Console.WriteLine($"  Всего XP: {level.TotalXP:N0}");
            
            var progress = level.XPToNextLevel > 0 ? (double)level.CurrentLevelXP / level.XPToNextLevel * 100 : 0;
            Console.Write($"  Прогресс: {level.CurrentLevelXP:N0} / {level.XPToNextLevel:N0} XP ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"({progress:F1}%)");
            
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("  [");
            var filled = (int)(progress / 2);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new string('█', filled));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('░', 50 - filled));
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("]");
            Console.ResetColor();
        }

        public void ShowAchievement(Achievement a)
        {
            var color = a.IsUnlocked ? ConsoleColor.Green : ConsoleColor.DarkGray;
            var icon = a.IsUnlocked ? "✓" : "○";
            var progress = a.TargetValue > 0 ? (double)a.CurrentValue / a.TargetValue * 100 : 0;

            Console.ForegroundColor = color;
            Console.Write($"{icon} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{a.Icon} {a.Title}: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{a.CurrentValue}/{a.TargetValue}");
            
            if (a.IsUnlocked)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" ✓ РАЗБЛОКИРОВАНО!");
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        public void ShowDailyEntry(DailyEntry entry)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n📅 {entry.Date:dd MMMM yyyy}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Настроение: {entry.Mood}/10");
            
            if (entry.Habits.Any())
            {
                Console.WriteLine("\n🎯 Привычки:");
                foreach (var h in entry.Habits)
                {
                    var icon = h.IsCompleted ? "✓" : "○";
                    var color = h.IsCompleted ? ConsoleColor.Green : ConsoleColor.Gray;
                    Console.ForegroundColor = color;
                    Console.WriteLine($"  {icon} {h.HabitName}");
                }
            }

            if (entry.Tasks.Any())
            {
                Console.WriteLine("\n✅ Задачи:");
                foreach (var task in entry.Tasks)
                {
                    var icon = task.IsCompleted ? "✓" : "○";
                    var color = task.IsCompleted ? ConsoleColor.Green : ConsoleColor.Gray;
                    Console.ForegroundColor = color;
                    Console.WriteLine($"  {icon} {task.Description} (+{task.XP} XP)");
                }
            }

            if (!string.IsNullOrEmpty(entry.Notes))
            {
                Console.WriteLine($"\n📝 Заметки: {entry.Notes}");
            }
            Console.ResetColor();
        }

        public string Read(string prompt, ConsoleColor color = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = color;
            Console.Write(prompt);
            Console.ResetColor();
            return Console.ReadLine() ?? "";
        }

        public decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                var input = Read(prompt);
                if (decimal.TryParse(input, out var value))
                    return value;
                Error("Неверный формат числа.");
            }
        }

        public int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                var input = Read(prompt);
                if (int.TryParse(input, out var value) && value >= min && value <= max)
                    return value;
                Error($"Введите число от {min} до {max}.");
            }
        }
    }
}

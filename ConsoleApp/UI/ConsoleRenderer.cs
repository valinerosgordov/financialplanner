using System;
using System.Linq;
using FinancialPlanner.Models;

namespace FinancialPlanner.ConsoleApp.UI
{
    public class ConsoleRenderer
    {
        private const ConsoleColor NeonBlue = ConsoleColor.Cyan;
        private const ConsoleColor NeonPurple = ConsoleColor.Magenta;
        private const ConsoleColor NeonPink = ConsoleColor.Magenta;
        private const ConsoleColor DarkBg = ConsoleColor.Black;
        private const ConsoleColor Glow = ConsoleColor.Cyan;
        private const ConsoleColor Accent = ConsoleColor.DarkCyan;
        private const ConsoleColor Gold = ConsoleColor.Yellow;

        public void Clear()
        {
            Console.Clear();
            Console.BackgroundColor = DarkBg;
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void PrintLine(char left, char fill, char right, int width, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(left);
            Console.Write(new string(fill, width));
            Console.WriteLine(right);
        }

        private void PrintCentered(string text, int width, ConsoleColor color)
        {
            var padding = (width - text.Length) / 2;
            Console.ForegroundColor = color;
            Console.Write(new string(' ', padding));
            Console.Write(text);
            Console.WriteLine(new string(' ', width - padding - text.Length));
        }

        public void ShowWelcome()
        {
            Clear();
            
            // Top decorative border
            Console.ForegroundColor = NeonBlue;
            Console.Write("╔");
            for (int i = 0; i < 78; i++) Console.Write("═");
            Console.WriteLine("╗");
            
            Console.Write("║");
            Console.Write(new string(' ', 78));
            Console.WriteLine("║");
            
            // SOLO LEVELING ASCII Art - Enhanced
            var soloLeveling = new[]
            {
                "  ███████╗ ██████╗ ██╗      ██████╗     ██╗     ███████╗██╗   ██╗███████╗██╗███╗   ██╗ ██████╗ ",
                "  ██╔════╝██╔═══██╗██║     ██╔═══██╗    ██║     ██╔════╝██║   ██║██╔════╝██║████╗  ██║██╔════╝ ",
                "  ███████╗██║   ██║██║     ██║   ██║    ██║     █████╗  ██║   ██║█████╗  ██║██╔██╗ ██║██║  ███╗",
                "  ╚════██║██║   ██║██║     ██║   ██║    ██║     ██╔══╝  ╚██╗ ██╔╝██╔══╝  ██║██║╚██╗██║██║   ██║",
                "  ███████║╚██████╔╝███████╗╚██████╔╝    ███████╗███████╗ ╚████╔╝ ███████╗██║██║ ╚████║╚██████╔╝",
                "  ╚══════╝ ╚═════╝ ╚══════╝ ╚═════╝     ╚══════╝╚══════╝  ╚═══╝  ╚══════╝╚═╝╚═╝  ╚═══╝ ╚═════╝ "
            };

            foreach (var line in soloLeveling)
            {
                Console.Write("║");
                Console.Write(new string(' ', 10));
                Console.ForegroundColor = Glow;
                Console.Write(line);
                Console.ForegroundColor = NeonBlue;
                Console.Write(new string(' ', 10));
                Console.WriteLine("║");
                if (line != soloLeveling[soloLeveling.Length - 1])
                {
                    Console.Write("║");
                    Console.Write(new string(' ', 78));
                    Console.WriteLine("║");
                }
            }
            
            Console.Write("║");
            Console.Write(new string(' ', 78));
            Console.WriteLine("║");
            
            // Financial Planner subtitle with decorative box
            Console.Write("║");
            Console.Write(new string(' ', 15));
            Console.ForegroundColor = NeonPurple;
            Console.Write("┏");
            Console.Write(new string('━', 48));
            Console.WriteLine("┓");
            
            Console.Write("║");
            Console.Write(new string(' ', 15));
            Console.ForegroundColor = NeonPurple;
            Console.Write("┃");
            Console.ForegroundColor = Gold;
            Console.Write("  ╔═══════════════════════════════════════════╗");
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = NeonBlue;
            Console.WriteLine("║");
            
            Console.Write("║");
            Console.Write(new string(' ', 15));
            Console.ForegroundColor = NeonPurple;
            Console.Write("┃");
            Console.ForegroundColor = Gold;
            Console.Write("  ║");
            Console.ForegroundColor = Glow;
            Console.Write("  💰 FINANCIAL PLANNER SYSTEM 💰");
            Console.ForegroundColor = Gold;
            Console.Write("  ║");
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = NeonBlue;
            Console.WriteLine("║");
            
            Console.Write("║");
            Console.Write(new string(' ', 15));
            Console.ForegroundColor = NeonPurple;
            Console.Write("┃");
            Console.ForegroundColor = Gold;
            Console.Write("  ╚═══════════════════════════════════════════╝");
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = NeonBlue;
            Console.WriteLine("║");
            
            Console.Write("║");
            Console.Write(new string(' ', 15));
            Console.ForegroundColor = NeonPurple;
            Console.Write("┗");
            Console.Write(new string('━', 48));
            Console.WriteLine("┛");
            
            Console.Write("║");
            Console.Write(new string(' ', 78));
            Console.WriteLine("║");
            
            // Enhanced Notification box
            Console.Write("║");
            Console.Write(new string(' ', 12));
            Console.ForegroundColor = Accent;
            Console.Write("┏");
            Console.Write(new string('━', 54));
            Console.WriteLine("┓");
            
            Console.Write("║");
            Console.Write(new string(' ', 12));
            Console.ForegroundColor = Accent;
            Console.Write("┃");
            Console.ForegroundColor = Gold;
            Console.Write("  ⚡");
            Console.ForegroundColor = Glow;
            Console.Write(" NOTIFICATION ");
            Console.ForegroundColor = Gold;
            Console.Write("⚡");
            Console.ForegroundColor = Accent;
            Console.Write("                                    ┃");
            Console.ForegroundColor = NeonBlue;
            Console.WriteLine("║");
            
            Console.Write("║");
            Console.Write(new string(' ', 12));
            Console.ForegroundColor = Accent;
            Console.Write("┣");
            Console.Write(new string('━', 54));
            Console.WriteLine("┫");
            
            Console.Write("║");
            Console.Write(new string(' ', 12));
            Console.ForegroundColor = Accent;
            Console.Write("┃");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("  You have acquired the qualifications to be a");
            Console.ForegroundColor = Accent;
            Console.Write("  ┃");
            Console.ForegroundColor = NeonBlue;
            Console.WriteLine("║");
            
            Console.Write("║");
            Console.Write(new string(' ', 12));
            Console.ForegroundColor = Accent;
            Console.Write("┃");
            Console.ForegroundColor = Glow;
            Console.Write("  Financial Planner.");
            Console.ForegroundColor = Accent;
            Console.Write("                                      ┃");
            Console.ForegroundColor = NeonBlue;
            Console.WriteLine("║");
            
            Console.Write("║");
            Console.Write(new string(' ', 12));
            Console.ForegroundColor = Accent;
            Console.Write("┃");
            Console.ForegroundColor = Gold;
            Console.Write("  Will you accept?");
            Console.ForegroundColor = Accent;
            Console.Write("                                      ┃");
            Console.ForegroundColor = NeonBlue;
            Console.WriteLine("║");
            
            Console.Write("║");
            Console.Write(new string(' ', 12));
            Console.ForegroundColor = Accent;
            Console.Write("┗");
            Console.Write(new string('━', 54));
            Console.WriteLine("┛");
            
            Console.Write("║");
            Console.Write(new string(' ', 78));
            Console.WriteLine("║");
            
            Console.Write("╚");
            for (int i = 0; i < 78; i++) Console.Write("═");
            Console.WriteLine("╝");
            
            Console.ForegroundColor = Glow;
            Console.Write("\n  ");
            Console.Write(new string('░', 20));
            Console.Write(" Loading system data... ");
            Console.WriteLine(new string('░', 20));
            Console.ResetColor();
        }

        public void Header(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = NeonBlue;
            Console.Write("╔");
            Console.Write(new string('═', 78));
            Console.WriteLine("╗");
            
            Console.Write("║");
            var titlePadding = (78 - title.Length - 4) / 2;
            Console.Write(new string(' ', titlePadding));
            Console.ForegroundColor = Gold;
            Console.Write("◆ ");
            Console.ForegroundColor = Glow;
            Console.Write(title);
            Console.ForegroundColor = Gold;
            Console.Write(" ◆");
            Console.Write(new string(' ', 78 - titlePadding - title.Length - 4));
            Console.ForegroundColor = NeonBlue;
            Console.WriteLine("║");
            
            Console.Write("╚");
            Console.Write(new string('═', 78));
            Console.WriteLine("╝");
            Console.WriteLine();
            Console.ResetColor();
        }

        public void Menu(string[] options)
        {
            Console.ForegroundColor = NeonPurple;
            Console.Write("┏");
            Console.Write(new string('━', 70));
            Console.WriteLine("┓");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("┃");
            Console.ForegroundColor = Glow;
            Console.Write("  MENU");
            Console.ForegroundColor = NeonPurple;
            Console.Write(new string(' ', 64));
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("┣");
            Console.Write(new string('━', 70));
            Console.WriteLine("┫");
            
            for (int i = 0; i < options.Length; i++)
            {
                Console.ForegroundColor = NeonPurple;
                Console.Write("┃");
                Console.ForegroundColor = Gold;
                Console.Write($"  [{i + 1}] ");
                Console.ForegroundColor = Glow;
                Console.Write(options[i]);
                var padding = 70 - 10 - options[i].Length;
                Console.Write(new string(' ', Math.Max(0, padding)));
                Console.ForegroundColor = NeonPurple;
                Console.WriteLine("┃");
                
                if (i < options.Length - 1)
                {
                    Console.ForegroundColor = NeonPurple;
                    Console.Write("┃");
                    Console.ForegroundColor = Accent;
                    Console.Write(new string('─', 70));
                    Console.ForegroundColor = NeonPurple;
                    Console.WriteLine("┃");
                }
            }
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("┗");
            Console.Write(new string('━', 70));
            Console.WriteLine("┛");
            
            Console.ForegroundColor = Gold;
            Console.Write("\n  ▶ ");
            Console.ForegroundColor = Glow;
            Console.Write("Выберите опцию: ");
            Console.ResetColor();
        }

        public void Write(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"  {text}");
            Console.ResetColor();
        }

        public void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("  ┏");
            Console.Write(new string('━', 60));
            Console.WriteLine("┓");
            Console.Write("  ┃");
            Console.ForegroundColor = Gold;
            Console.Write("  ⚠ ERROR");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(new string(' ', 52));
            Console.WriteLine("┃");
            Console.Write("  ┣");
            Console.Write(new string('━', 60));
            Console.WriteLine("┫");
            Console.Write("  ┃");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  {msg}");
            Console.Write(new string(' ', Math.Max(0, 60 - msg.Length - 2)));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("┃");
            Console.Write("  ┗");
            Console.Write(new string('━', 60));
            Console.WriteLine("┛");
            Console.ResetColor();
        }

        public void Success(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("  ┏");
            Console.Write(new string('━', 60));
            Console.WriteLine("┓");
            Console.Write("  ┃");
            Console.ForegroundColor = Gold;
            Console.Write("  ✓ SUCCESS");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new string(' ', 50));
            Console.WriteLine("┃");
            Console.Write("  ┣");
            Console.Write(new string('━', 60));
            Console.WriteLine("┫");
            Console.Write("  ┃");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  {msg}");
            Console.Write(new string(' ', Math.Max(0, 60 - msg.Length - 2)));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("┃");
            Console.Write("  ┗");
            Console.Write(new string('━', 60));
            Console.WriteLine("┛");
            Console.ResetColor();
        }

        public void Warning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  ┏");
            Console.Write(new string('━', 60));
            Console.WriteLine("┓");
            Console.Write("  ┃");
            Console.ForegroundColor = Gold;
            Console.Write("  ⚠ WARNING");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(new string(' ', 50));
            Console.WriteLine("┃");
            Console.Write("  ┣");
            Console.Write(new string('━', 60));
            Console.WriteLine("┫");
            Console.Write("  ┃");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  {msg}");
            Console.Write(new string(' ', Math.Max(0, 60 - msg.Length - 2)));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("┃");
            Console.Write("  ┗");
            Console.Write(new string('━', 60));
            Console.WriteLine("┛");
            Console.ResetColor();
        }

        public void ShowTransaction(Transaction t, int index = -1)
        {
            var color = t.Type == TransactionType.Income ? ConsoleColor.Green : ConsoleColor.Red;
            var icon = t.Type == TransactionType.Income ? "💰" : "💸";
            var prefix = index > 0 ? $"{index}. " : "";

            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┏");
            Console.Write(new string('━', 68));
            Console.WriteLine("┓");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = Gold;
            Console.Write("  ");
            Console.ForegroundColor = color;
            Console.Write($"{prefix}{icon} {t.Description}");
            var descPad = 68 - 4 - prefix.Length - icon.Length - t.Description.Length;
            Console.Write(new string(' ', Math.Max(0, descPad)));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = Accent;
            Console.Write("  ");
            Console.Write(new string('─', 66));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = Accent;
            Console.Write("  📋 Категория: ");
            Console.ForegroundColor = Glow;
            Console.Write(t.Category);
            var catPad = 68 - 18 - t.Category.Length;
            Console.Write(new string(' ', Math.Max(0, catPad - 15)));
            Console.ForegroundColor = Gold;
            Console.Write("│ ");
            Console.ForegroundColor = color;
            Console.Write($"{t.Amount:N2} {t.Currency}");
            var amountPad = 15 - (t.Amount.ToString("N2").Length + t.Currency.Length + 1);
            Console.Write(new string(' ', Math.Max(0, amountPad)));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = Accent;
            Console.Write("  📅 Дата: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{t.Date:dd.MM.yyyy HH:mm}");
            var datePad = 68 - 12 - t.Date.ToString("dd.MM.yyyy HH:mm").Length;
            Console.Write(new string(' ', Math.Max(0, datePad)));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┗");
            Console.Write(new string('━', 68));
            Console.WriteLine("┛");
            Console.ResetColor();
        }

        public void ShowLevel(LevelSystem level)
        {
            Console.ForegroundColor = NeonBlue;
            Console.Write("╔");
            Console.Write(new string('═', 78));
            Console.WriteLine("╗");
            
            Console.Write("║");
            Console.Write(new string(' ', 20));
            Console.ForegroundColor = Gold;
            Console.Write("◆");
            Console.ForegroundColor = Glow;
            Console.Write("  ⚡ SOLO LEVELING SYSTEM ⚡");
            Console.ForegroundColor = Gold;
            Console.Write("  ◆");
            Console.Write(new string(' ', 20));
            Console.ForegroundColor = NeonBlue;
            Console.WriteLine("║");
            
            Console.Write("╚");
            Console.Write(new string('═', 78));
            Console.WriteLine("╝");
            Console.WriteLine();
            
            // Enhanced Level box
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┏");
            Console.Write(new string('━', 68));
            Console.WriteLine("┓");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = Gold;
            Console.Write("  ");
            Console.ForegroundColor = Glow;
            Console.Write("LEVEL INFORMATION");
            Console.ForegroundColor = Gold;
            Console.Write("  ");
            Console.Write(new string('─', 48));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.Write(new string(' ', 68));
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = Gold;
            Console.Write("  🎯 Уровень: ");
            Console.ForegroundColor = Glow;
            Console.Write($"Level {level.Level}");
            var levelPad = 68 - 16 - level.Level.ToString().Length;
            Console.Write(new string(' ', Math.Max(0, levelPad)));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = Gold;
            Console.Write("  ⭐ Всего XP: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{level.TotalXP:N0}");
            var xpPad = 68 - 16 - level.TotalXP.ToString("N0").Length;
            Console.Write(new string(' ', Math.Max(0, xpPad)));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            var progress = level.XPToNextLevel > 0 ? (double)level.CurrentLevelXP / level.XPToNextLevel * 100 : 0;
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = Gold;
            Console.Write("  📊 Прогресс: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{level.CurrentLevelXP:N0} / {level.XPToNextLevel:N0} XP");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($" ({progress:F1}%)");
            var progPad = 68 - 20 - level.CurrentLevelXP.ToString("N0").Length - level.XPToNextLevel.ToString("N0").Length - progress.ToString("F1").Length;
            Console.Write(new string(' ', Math.Max(0, progPad)));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.Write(new string(' ', 68));
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.Write(new string(' ', 8));
            Console.ForegroundColor = Gold;
            Console.Write("[");
            var filled = (int)(progress / 2);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new string('█', filled));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('░', 50 - filled));
            Console.ForegroundColor = Gold;
            Console.Write("]");
            Console.Write(new string(' ', 8));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┗");
            Console.Write(new string('━', 68));
            Console.WriteLine("┛");
            Console.ResetColor();
        }

        public void ShowAchievement(Achievement a)
        {
            var color = a.IsUnlocked ? ConsoleColor.Green : ConsoleColor.DarkGray;
            var icon = a.IsUnlocked ? "✓" : "○";
            var glow = a.IsUnlocked ? Glow : ConsoleColor.DarkGray;

            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┏");
            Console.Write(new string('━', 68));
            Console.WriteLine("┓");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = color;
            Console.Write($"  {icon} ");
            Console.ForegroundColor = glow;
            Console.Write($"{a.Icon} {a.Title}");
            var titlePad = 68 - 8 - a.Icon.Length - a.Title.Length;
            Console.Write(new string(' ', Math.Max(0, titlePad)));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = Accent;
            Console.Write("  ");
            Console.Write(new string('─', 66));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = Gold;
            Console.Write("  📈 Прогресс: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{a.CurrentValue}/{a.TargetValue}");
            if (a.IsUnlocked)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" ✓ UNLOCKED!");
            }
            var progPad = 68 - 18 - a.CurrentValue.ToString().Length - a.TargetValue.ToString().Length - (a.IsUnlocked ? 12 : 0);
            Console.Write(new string(' ', Math.Max(0, progPad)));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┗");
            Console.Write(new string('━', 68));
            Console.WriteLine("┛");
            Console.ResetColor();
        }

        public void ShowDailyEntry(DailyEntry entry)
        {
            Console.ForegroundColor = NeonBlue;
            Console.Write("╔");
            Console.Write(new string('═', 78));
            Console.WriteLine("╗");
            
            Console.Write("║");
            Console.Write(new string(' ', 25));
            Console.ForegroundColor = Gold;
            Console.Write("◆");
            Console.ForegroundColor = Glow;
            Console.Write($" 📅 {entry.Date:dd MMMM yyyy} ");
            Console.ForegroundColor = Gold;
            Console.Write("◆");
            Console.Write(new string(' ', 25));
            Console.ForegroundColor = NeonBlue;
            Console.WriteLine("║");
            
            Console.Write("╠");
            Console.Write(new string('═', 78));
            Console.WriteLine("╣");
            
            Console.Write("║");
            Console.ForegroundColor = Gold;
            Console.Write("  💭 Настроение: ");
            Console.ForegroundColor = Glow;
            Console.Write($"{entry.Mood}/10");
            var moodPad = 78 - 20 - entry.Mood.ToString().Length;
            Console.Write(new string(' ', Math.Max(0, moodPad)));
            Console.ForegroundColor = NeonBlue;
            Console.WriteLine("║");
            
            if (entry.Habits.Any())
            {
                Console.Write("║");
                Console.ForegroundColor = Gold;
                Console.Write("  🎯 Привычки:");
                Console.ForegroundColor = NeonBlue;
                Console.Write(new string(' ', 65));
                Console.WriteLine("║");
                foreach (var h in entry.Habits)
                {
                    var icon = h.IsCompleted ? "✓" : "○";
                    var color = h.IsCompleted ? ConsoleColor.Green : ConsoleColor.Gray;
                    Console.Write("║");
                    Console.Write(new string(' ', 6));
                    Console.ForegroundColor = color;
                    Console.Write($"{icon} ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(h.HabitName);
                    Console.ForegroundColor = NeonBlue;
                    Console.Write(new string(' ', 68 - 8 - h.HabitName.Length));
                    Console.WriteLine("║");
                }
            }

            if (entry.Tasks.Any())
            {
                Console.Write("║");
                Console.ForegroundColor = Gold;
                Console.Write("  ✅ Задачи:");
                Console.ForegroundColor = NeonBlue;
                Console.Write(new string(' ', 67));
                Console.WriteLine("║");
                foreach (var task in entry.Tasks)
                {
                    var icon = task.IsCompleted ? "✓" : "○";
                    var color = task.IsCompleted ? ConsoleColor.Green : ConsoleColor.Gray;
                    Console.Write("║");
                    Console.Write(new string(' ', 6));
                    Console.ForegroundColor = color;
                    Console.Write($"{icon} ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"{task.Description} ");
                    Console.ForegroundColor = Glow;
                    Console.Write($"(+{task.XP} XP)");
                    Console.ForegroundColor = NeonBlue;
                    var taskPad = 78 - 10 - task.Description.Length - task.XP.ToString().Length;
                    Console.Write(new string(' ', Math.Max(0, taskPad)));
                    Console.WriteLine("║");
                }
            }

            if (!string.IsNullOrEmpty(entry.Notes))
            {
                Console.Write("║");
                Console.ForegroundColor = Gold;
                Console.Write("  📝 Заметки: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(entry.Notes);
                Console.ForegroundColor = NeonBlue;
                var notesPad = 78 - 16 - entry.Notes.Length;
                Console.Write(new string(' ', Math.Max(0, notesPad)));
                Console.WriteLine("║");
            }
            
            Console.Write("╚");
            Console.Write(new string('═', 78));
            Console.WriteLine("╝");
            Console.ResetColor();
        }

        public void ShowStatsBox(string title, string value, ConsoleColor valueColor = ConsoleColor.White)
        {
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┏");
            Console.Write(new string('━', 68));
            Console.WriteLine("┓");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = Gold;
            Console.Write("  ");
            Console.ForegroundColor = Glow;
            Console.Write(title);
            var titlePad = 68 - 4 - title.Length;
            Console.Write(new string(' ', Math.Max(0, titlePad)));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.ForegroundColor = Accent;
            Console.Write("  ");
            Console.Write(new string('─', 66));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┃");
            Console.Write(new string(' ', 8));
            Console.ForegroundColor = valueColor;
            Console.Write(value);
            var valuePad = 68 - 10 - value.Length;
            Console.Write(new string(' ', Math.Max(0, valuePad)));
            Console.ForegroundColor = NeonPurple;
            Console.WriteLine("┃");
            
            Console.ForegroundColor = NeonPurple;
            Console.Write("  ┗");
            Console.Write(new string('━', 68));
            Console.WriteLine("┛");
            Console.ResetColor();
        }

        public string Read(string prompt, ConsoleColor color = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = Gold;
            Console.Write($"  ▶ ");
            Console.ForegroundColor = Glow;
            Console.Write($"{prompt}");
            var input = Console.ReadLine();
            Console.ResetColor();
            return input ?? "";
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

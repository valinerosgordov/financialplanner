using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using FinancialPlanner.Models;
using FinancialPlanner.ViewModels;

namespace FinancialPlanner.Converters
{
    public class TransactionColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TransactionType type)
            {
                return type == TransactionType.Income 
                    ? new SolidColorBrush(Color.FromRgb(26, 61, 46)) // Dark green anime style
                    : new SolidColorBrush(Color.FromRgb(61, 26, 26)); // Dark red anime style
            }
            return new SolidColorBrush(Color.FromRgb(26, 13, 46)); // Anime purple dark
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TransactionBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TransactionType type)
            {
                return type == TransactionType.Income 
                    ? new SolidColorBrush(Color.FromRgb(16, 185, 129)) // Green
                    : new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Red
            }
            return new SolidColorBrush(Color.FromRgb(139, 92, 246)); // Anime purple
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BalanceColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal balance)
            {
                return balance >= 0 
                    ? new SolidColorBrush(Color.FromRgb(167, 139, 250)) // Anime purple bright
                    : new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Red
            }
            return new SolidColorBrush(Color.FromRgb(233, 213, 255)); // Anime text
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StrikeThroughConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted && isCompleted)
            {
                return TextDecorations.Strikethrough;
            }
            return new TextDecorationCollection();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BudgetProgressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is MainViewModel vm && values[1] is string category)
            {
                return vm.GetBudgetProgress(category);
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CategoryTotalConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is ViewModels.MainViewModel vm && values[1] is string category)
            {
                return vm.GetCategoryTotal(category);
            }
            return 0m;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TaskCompletedBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted)
            {
                return isCompleted 
                    ? new SolidColorBrush(Color.FromRgb(16, 185, 129)) // Green
                    : new SolidColorBrush(Color.FromRgb(139, 92, 246)); // Anime purple
            }
            return new SolidColorBrush(Color.FromRgb(139, 92, 246));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TaskCompletedEffectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted && isCompleted)
            {
                return new DropShadowEffect
                {
                    Color = Color.FromRgb(16, 185, 129),
                    Direction = 270,
                    ShadowDepth = 4,
                    BlurRadius = 15,
                    Opacity = 0.6
                };
            }
            return new DropShadowEffect { Opacity = 0 }; // Возвращаем пустой эффект вместо null
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }
            return false;
        }
    }

    public class AchievementBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isUnlocked)
            {
                return isUnlocked 
                    ? new SolidColorBrush(Color.FromRgb(26, 61, 46)) // Green
                    : new SolidColorBrush(Color.FromRgb(26, 13, 46)); // Dark purple
            }
            return new SolidColorBrush(Color.FromRgb(26, 13, 46));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AchievementBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isUnlocked)
            {
                return isUnlocked 
                    ? new SolidColorBrush(Color.FromRgb(16, 185, 129)) // Green
                    : new SolidColorBrush(Color.FromRgb(139, 92, 246)); // Purple
            }
            return new SolidColorBrush(Color.FromRgb(139, 92, 246));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AchievementEffectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isUnlocked && isUnlocked)
            {
                return new DropShadowEffect
                {
                    Color = Color.FromRgb(16, 185, 129),
                    Direction = 270,
                    ShadowDepth = 4,
                    BlurRadius = 15,
                    Opacity = 0.6
                };
            }
            return new DropShadowEffect { Opacity = 0 };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CategoryProgressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is decimal categoryTotal && values[1] is decimal totalExpenses)
            {
                if (totalExpenses == 0) return 0.0;
                // Максимальная ширина 400px, пропорционально расходу
                return Math.Min(400, (double)(categoryTotal / totalExpenses * 400));
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

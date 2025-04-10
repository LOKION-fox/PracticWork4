using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data;

namespace Work_03._04._2025
{
    public partial class MainWindow : Window
    {
        private StringBuilder builder = new StringBuilder();
        private bool resultDisplay = false;
        private bool error = false;
        private int parenthesisCount = 0;
        private readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private bool powerOperation = false;
        private double baseValue = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClearAll()
        {
            builder.Clear();
            ExpressionTextBlock.Text = "";
            ResultTextBlock.Text = "0";
            resultDisplay = false;
            error = false;
            parenthesisCount = 0;
            powerOperation = false;
        }

        private void Clear()
        {
            if (resultDisplay || error)
            {
                ClearAll();
            }
            else
            {
                ResultTextBlock.Text = "0";
                powerOperation = false;
            }
        }

        private void UpdateExpressionDisplay()
        {
            ExpressionTextBlock.Text = builder.ToString();
        }

        private void AddToExpression(string value)
        {
            if (resultDisplay || error)
            {
                builder.Clear();
                resultDisplay = false;
                error = false;
                parenthesisCount = 0;
                powerOperation = false;
            }
            builder.Append(value);
            UpdateExpressionDisplay();
        }

        private void DigitButton_Click(object sender, RoutedEventArgs e)
        {
            if (powerOperation && ResultTextBlock.Text == "0")
            {
                ResultTextBlock.Text = "";
            }

            if (resultDisplay || error)
            {
                builder.Clear();
                ResultTextBlock.Text = "0";
                resultDisplay = false;
                error = false;
                parenthesisCount = 0;
                powerOperation = false;
            }

            if (builder.Length > 0 && builder.ToString().TrimEnd().EndsWith(")"))
            {
                return;
            }

            var button = (Button)sender;
            string digit = button.Content.ToString();

            if (ResultTextBlock.Text == "0")
            {
                ResultTextBlock.Text = digit;
            }
            else
            {
                ResultTextBlock.Text += digit;
            }
        }

        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (resultDisplay)
            {
                builder.Clear();
                ResultTextBlock.Text = "0";
                resultDisplay = false;
                powerOperation = false;
            }

            if (!ResultTextBlock.Text.Contains(",") && !error)
            {
                ResultTextBlock.Text += ResultTextBlock.Text == "" ? "0," : ",";
            }
        }

        private void SignButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBlock.Text != "0")
            {
                if (ResultTextBlock.Text.StartsWith("-"))
                {
                    ResultTextBlock.Text = ResultTextBlock.Text.Substring(1);
                }
                else
                {
                    ResultTextBlock.Text = "-" + ResultTextBlock.Text;
                }
            }
        }

        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            if (powerOperation) return;

            var button = (Button)sender;
            string operation = button.Content.ToString();

            if (builder.Length > 0 && builder.ToString().TrimEnd().EndsWith(")"))
            {
                AddToExpression(" " + operation + " ");
                ResultTextBlock.Text = "0";
            }
            else if (ResultTextBlock.Text != "0")
            {
                AddToExpression(ResultTextBlock.Text.Replace(",", ".") + " " + operation + " ");
                ResultTextBlock.Text = "0";
            }
        }

        private void ParenthesisButton_Click(object sender, RoutedEventArgs e)
        {
            if (powerOperation) return;

            var button = (Button)sender;
            string parenthesis = button.Content.ToString();

            if (parenthesis == "(")
            {
                if (ResultTextBlock.Text != "0" && !resultDisplay)
                {
                    AddToExpression(ResultTextBlock.Text.Replace(",", ".") + " × ( ");
                    ResultTextBlock.Text = "0";
                }
                else
                {
                    AddToExpression("( ");
                }
                parenthesisCount++;
            }
            else if (parenthesis == ")")
            {
                if (parenthesisCount > 0)
                {
                    AddToExpression(ResultTextBlock.Text.Replace(",", ".") + " )");
                    ResultTextBlock.Text = "0";
                    parenthesisCount--;
                }
                else
                {
                    ResultTextBlock.Text = "Ошибка скобок";
                    error = true;
                }
            }
        }

        private void PowerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!powerOperation)
            {
                baseValue = double.Parse(ResultTextBlock.Text.Replace(",", "."), culture);
                ResultTextBlock.Text = "0";
                powerOperation = true;
                builder.Append($"{baseValue.ToString(culture)}^(");
                UpdateExpressionDisplay();
            }
            else
            {
                try
                {
                    double exponent = double.Parse(ResultTextBlock.Text.Replace(",", "."), culture);
                    double result = Math.Pow(baseValue, exponent);
                    ResultTextBlock.Text = result.ToString(culture).Replace(".", ",");
                    builder.Append($"{exponent.ToString(culture)})");
                    UpdateExpressionDisplay();
                    powerOperation = false;
                    resultDisplay = true;
                }
                catch
                {
                    ResultTextBlock.Text = "Ошибка";
                    error = true;
                    powerOperation = false;
                }
            }
        }

        private void ConstantButton_Click(object sender, RoutedEventArgs e)
        {
            if (powerOperation) return;

            var button = (Button)sender;
            string constant = button.Content.ToString();

            switch (constant)
            {
                case "π":
                    ResultTextBlock.Text = Math.PI.ToString().Replace(".", ",");
                    break;
                case "e":
                    ResultTextBlock.Text = Math.E.ToString().Replace(".", ",");
                    break;
            }

            AddToExpression(constant);
        }

        private void FunctionButton_Click(object sender, RoutedEventArgs e)
        {
            if (powerOperation) return;

            var button = (Button)sender;
            string function = button.Content.ToString();

            try
            {
                double value = double.Parse(ResultTextBlock.Text.Replace(",", "."),
                    culture);
                double result = CalculateFunction(function, value);

                ResultTextBlock.Text = result.ToString(culture).Replace(".", ",");
                AddToExpression($"{function}({value.ToString(culture)})");
                resultDisplay = true;
            }
            catch
            {
                ResultTextBlock.Text = "Ошибка";
                error = true;
            }
        }

        private double CalculateFunction(string function, double value)
        {
            switch (function)
            {
                case "sin": return Math.Sin(value * Math.PI / 180);
                case "cos": return Math.Cos(value * Math.PI / 180);
                case "tan": return Math.Tan(value * Math.PI / 180);
                case "x²": return Math.Pow(value, 2);
                case "1/x": return 1 / value;
                case "|x|": return Math.Abs(value);
                case "n!": return Factorial((int)value);
                case "log": return Math.Log10(value);
                case "ln": return Math.Log(value);
                case "10ˣ": return Math.Pow(10, value);
                case "∛x": return Math.Cbrt(value);
                default: throw new ArgumentException("Неизвестная функция");
            }
        }

        private int Factorial(int n)
        {
            if (n < 0) return -1;
            if (n == 0) return 1;
            if (n > 20) throw new OverflowException();
            return n * Factorial(n - 1);
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (powerOperation) return;

            if (ResultTextBlock.Text.Length > 1)
            {
                ResultTextBlock.Text = ResultTextBlock.Text.Substring(0, ResultTextBlock.Text.Length - 1);
            }
            else
            {
                ResultTextBlock.Text = "0";
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            if (powerOperation)
            {
                ResultTextBlock.Text = "Завершите ввод степени";
                return;
            }

            try
            {
                if (!resultDisplay)
                {
                    AddToExpression(ResultTextBlock.Text.Replace(",", "."));
                }

                if (parenthesisCount != 0)
                {
                    ResultTextBlock.Text = "Незакрытые скобки";
                    error = true;
                    return;
                }

                string expression = builder.ToString()
                    .Replace("×", "*")
                    .Replace("÷", "/")
                    .Replace(",", ".")
                    .Replace(" ", "");

                var result = EvaluateExpression(expression);
                ResultTextBlock.Text = result.ToString(culture).Replace(".", ",");
                builder.Clear();
                resultDisplay = true;
            }
            catch
            {
                ResultTextBlock.Text = "Ошибка";
                builder.Clear();
                error = true;
                parenthesisCount = 0;
            }
        }

        private double EvaluateExpression(string expression)
        {
            return Convert.ToDouble(new DataTable().Compute(expression, null));
        }
    }
}
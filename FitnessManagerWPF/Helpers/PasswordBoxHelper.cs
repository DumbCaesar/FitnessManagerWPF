using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FitnessManagerWPF.Helpers
{
    public static class PasswordBoxHelper
    {
        // Attached property to enable password masking
        public static readonly DependencyProperty PasswordCharProperty =
            DependencyProperty.RegisterAttached("PasswordChar", typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnPasswordCharChanged));

        // Attached property to store the actual password
        public static readonly DependencyProperty ActualPasswordProperty =
            DependencyProperty.RegisterAttached("ActualPassword", typeof(string),
                typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnActualPasswordPropertyChanged));

        public static bool GetPasswordChar(DependencyObject obj)
        {
            return (bool)obj.GetValue(PasswordCharProperty);
        }

        public static void SetPasswordChar(DependencyObject obj, bool value)
        {
            obj.SetValue(PasswordCharProperty, value);
        }

        public static string GetActualPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(ActualPasswordProperty);
        }

        public static void SetActualPassword(DependencyObject obj, string value)
        {
            obj.SetValue(ActualPasswordProperty, value);
        }

        // NEW: Handle changes from ViewModel to TextBox
        private static void OnActualPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if (_isUpdating.ContainsKey(textBox) && _isUpdating[textBox])
                    return;

                _isUpdating[textBox] = true;

                string newPassword = e.NewValue as string ?? string.Empty;

                // Update the masked display
                textBox.Text = new string('●', newPassword.Length);
                textBox.CaretIndex = textBox.Text.Length;

                _isUpdating[textBox] = false;
            }
        }

        private static void OnPasswordCharChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox && (bool)e.NewValue)
            {
                textBox.FontFamily = new FontFamily("Segoe UI Symbol");
                textBox.TextChanged += OnTextBoxTextChanged;
            }
        }

        private static readonly Dictionary<TextBox, bool> _isUpdating = new Dictionary<TextBox, bool>();

        private static void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (_isUpdating.ContainsKey(textBox) && _isUpdating[textBox])
                    return;

                _isUpdating[textBox] = true;

                string currentPassword = GetActualPassword(textBox) ?? "";
                string currentText = textBox.Text;
                int caretIndex = textBox.CaretIndex;

                // User deleted characters
                if (currentText.Length < currentPassword.Length)
                {
                    int diff = currentPassword.Length - currentText.Length;
                    int removeIndex = Math.Min(caretIndex, currentPassword.Length - diff);
                    if (removeIndex >= 0 && diff > 0 && removeIndex + diff <= currentPassword.Length)
                    {
                        currentPassword = currentPassword.Remove(removeIndex, diff);
                    }
                }
                // User typed new characters
                else if (currentText.Length > currentPassword.Length)
                {
                    string newChars = currentText.Replace("●", "");
                    int insertIndex = Math.Max(0, caretIndex - newChars.Length);
                    if (insertIndex >= 0 && insertIndex <= currentPassword.Length && newChars.Length > 0)
                    {
                        currentPassword = currentPassword.Insert(insertIndex, newChars);
                    }
                }

                // Update the ActualPassword property (this will update your ViewModel binding)
                SetActualPassword(textBox, currentPassword);

                // Display masked version
                textBox.Text = new string('●', currentPassword.Length);
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length);

                _isUpdating[textBox] = false;
            }
        }
    }
}
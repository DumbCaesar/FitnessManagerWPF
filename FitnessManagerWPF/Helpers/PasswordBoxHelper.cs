using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FitnessManagerWPF.Helpers
{
    /// <summary>
    /// Attached property helper that enables password masking on regular TextBox controls
    /// </summary>
    public static class PasswordBoxHelper
    {
        // Attached property to enable password character masking on TextBox
        public static readonly DependencyProperty PasswordCharProperty =
            DependencyProperty.RegisterAttached("PasswordChar", typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnPasswordCharChanged));

        public static bool GetPasswordChar(DependencyObject obj)
        {
            return (bool)obj.GetValue(PasswordCharProperty);
        }

        public static void SetPasswordChar(DependencyObject obj, bool value)
        {
            obj.SetValue(PasswordCharProperty, value);
        }

        // Called when PasswordChar property is set on a TextBox
        private static void OnPasswordCharChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox && (bool)e.NewValue)
            {
                // Use font that displays bullet characters properly
                textBox.FontFamily = new FontFamily("Segoe UI Symbol");
                // Hook into TextChanged event to mask password
                textBox.TextChanged += (s, args) => MaskPassword(textBox);
            }
        }

        // Store actual passwords for each TextBox
        private static readonly Dictionary<TextBox, string> _actualPasswords = new Dictionary<TextBox, string>();

        // Track which TextBox is currently being updated to prevent recursion
        private static readonly Dictionary<TextBox, bool> _isUpdating = new Dictionary<TextBox, bool>();

        private static void MaskPassword(TextBox textBox)
        {
            // Prevent recursive calls when we programmatically update the TextBox
            if (_isUpdating.ContainsKey(textBox) && _isUpdating[textBox])
                return;

            _isUpdating[textBox] = true;

            // Initialize actual password storage for this TextBox
            if (!_actualPasswords.ContainsKey(textBox))
                _actualPasswords[textBox] = "";

            string currentText = textBox.Text;
            int caretIndex = textBox.CaretIndex;

            // User deleted characters (backspace/delete)
            if (currentText.Length < _actualPasswords[textBox].Length)
            {
                int diff = _actualPasswords[textBox].Length - currentText.Length;
                int removeIndex = Math.Min(caretIndex, _actualPasswords[textBox].Length - diff);

                // Remove characters from actual password at correct position
                if (removeIndex >= 0 && diff > 0 && removeIndex + diff <= _actualPasswords[textBox].Length)
                {
                    _actualPasswords[textBox] = _actualPasswords[textBox].Remove(removeIndex, diff);
                }
            }
            // User typed new characters
            else if (currentText.Length > _actualPasswords[textBox].Length)
            {
                // Extract only new characters (not bullets)
                string newChars = currentText.Replace("●", "");
                int insertIndex = Math.Max(0, caretIndex - newChars.Length);

                // Insert new characters into actual password at correct position
                if (insertIndex >= 0 && insertIndex <= _actualPasswords[textBox].Length)
                {
                    _actualPasswords[textBox] = _actualPasswords[textBox].Insert(insertIndex, newChars);
                }
            }

            // Update the ViewModel binding with the actual password value
            var binding = textBox.GetBindingExpression(TextBox.TextProperty);
            if (binding != null)
            {
                var source = binding.ResolvedSource;
                var path = binding.ParentBinding.Path.Path;
                source.GetType().GetProperty(path)?.SetValue(source, _actualPasswords[textBox]);
            }

            // Replace TextBox display with bullet characters
            textBox.Text = new string('●', _actualPasswords[textBox].Length);
            // Restore caret position (clamped to valid range)
            textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length);

            _isUpdating[textBox] = false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// Defines a static class for WPF Attached Properties to extend the functionality of a TextBox
namespace FitnessManagerWPF.Helpers
{
    // =====================================
    //          PasswordBoxHelper
    //     Author: StackoverFlow/Claude
    // =====================================
    public static class PasswordBoxHelper
    {
        // --- Attached Properties ---

        // Attached property to enable the custom password masking behavior on a TextBox.
        // When set to 'true', it triggers the OnPasswordCharChanged method.
        public static readonly DependencyProperty PasswordCharProperty =
            DependencyProperty.RegisterAttached("PasswordChar", typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnPasswordCharChanged));

        // Attached property to store the actual, unmasked password string.
        // This is what a ViewModel will bind to.
        public static readonly DependencyProperty ActualPasswordProperty =
            DependencyProperty.RegisterAttached("ActualPassword", typeof(string),
                typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty,
                    // Enables two-way binding with a ViewModel
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    // Called when the ViewModel changes the password value
                    OnActualPasswordPropertyChanged));

        // --- Standard Get/Set Wrappers for Attached Properties ---

        // Getter for the PasswordChar attached property.
        public static bool GetPasswordChar(DependencyObject obj)
        {
            return (bool)obj.GetValue(PasswordCharProperty);
        }

        // Setter for the PasswordChar attached property.
        public static void SetPasswordChar(DependencyObject obj, bool value)
        {
            obj.SetValue(PasswordCharProperty, value);
        }

        // Getter for the ActualPassword attached property.
        public static string GetActualPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(ActualPasswordProperty);
        }

        // Setter for the ActualPassword attached property.
        public static void SetActualPassword(DependencyObject obj, string value)
        {
            obj.SetValue(ActualPasswordProperty, value);
        }

        // --- Property Changed Handlers ---

        // Handles changes to the ActualPassword property, typically from the ViewModel.
        private static void OnActualPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Only proceed if the attached property is on a TextBox
            if (d is TextBox textBox)
            {
                // Prevents re-entrancy (i.e., avoids triggering another update loop)
                if (_isUpdating.ContainsKey(textBox) && _isUpdating[textBox])
                    return;

                _isUpdating[textBox] = true;

                string newPassword = e.NewValue as string ?? string.Empty;

                // Update the TextBox display with the masked character ('●') based on password length
                textBox.Text = new string('●', newPassword.Length);
                // Set the cursor (caret) to the end of the masked text
                textBox.CaretIndex = textBox.Text.Length;

                _isUpdating[textBox] = false;
            }
        }

        // Handles initial setting of the PasswordChar property.
        private static void OnPasswordCharChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // If the property is set to true on a TextBox:
            if (d is TextBox textBox && (bool)e.NewValue)
            {
                // Sets a font that reliably displays the '●' character.
                textBox.FontFamily = new FontFamily("Segoe UI Symbol");
                // Subscribes to the TextBox's TextChanged event to handle user input.
                textBox.TextChanged += OnTextBoxTextChanged;
            }
        }

        // Dictionary to track which TextBox is currently being updated programmatically,
        // preventing infinite loops between property change and text changed events.
        private static readonly Dictionary<TextBox, bool> _isUpdating = new Dictionary<TextBox, bool>();

        // --- Text Changed Handler (User Input Logic) ---

        // Handles changes in the TextBox text, which are caused by the user typing or deleting.
        private static void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Re-entrancy check: ensures we're not inside a programmatic update
                if (_isUpdating.ContainsKey(textBox) && _isUpdating[textBox])
                    return;

                _isUpdating[textBox] = true;

                string currentPassword = GetActualPassword(textBox) ?? "";
                string currentText = textBox.Text;
                int caretIndex = textBox.CaretIndex;

                // --- User Deleted Characters ---
                if (currentText.Length < currentPassword.Length)
                {
                    // Calculates the number of characters deleted.
                    int diff = currentPassword.Length - currentText.Length;
                    // Determines the starting index for removal in the actual password string.
                    int removeIndex = Math.Min(caretIndex, currentPassword.Length - diff);
                    if (removeIndex >= 0 && diff > 0 && removeIndex + diff <= currentPassword.Length)
                    {
                        // Removes the characters from the ActualPassword string.
                        currentPassword = currentPassword.Remove(removeIndex, diff);
                    }
                }
                // --- User Typed New Characters ---
                else if (currentText.Length > currentPassword.Length)
                {
                    // Finds the *unmasked* new characters typed by removing all '●' symbols.
                    string newChars = currentText.Replace("●", "");
                    // Determines the insertion point in the actual password string.
                    int insertIndex = Math.Max(0, caretIndex - newChars.Length);
                    if (insertIndex >= 0 && insertIndex <= currentPassword.Length && newChars.Length > 0)
                    {
                        // Inserts the new characters into the ActualPassword string.
                        currentPassword = currentPassword.Insert(insertIndex, newChars);
                    }
                }

                // Update the bound ActualPassword property with the new unmasked value.
                SetActualPassword(textBox, currentPassword);

                // Update the TextBox display to show the correct number of masked symbols.
                textBox.Text = new string('●', currentPassword.Length);
                // Corrects the cursor position after updating the text.
                textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length);

                _isUpdating[textBox] = false;
            }
        }
    }
}
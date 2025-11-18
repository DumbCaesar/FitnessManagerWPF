using System;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace FitnessManagerWPF.Helpers
{
    public static class PasswordBoxHelper
    {
        // This attached property will hold the bound SecureString from the ViewModel
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",                     // Name of the attached property
                typeof(SecureString),                // Type of the property (SecureString for passwords)
                typeof(PasswordBoxHelper),           // Owner type of the property
                new PropertyMetadata(null, null));   // Default metadata (null, no change callback needed)

        // Getter for the attached property
        public static SecureString GetBoundPassword(DependencyObject obj)
        {
            return (SecureString)obj.GetValue(BoundPasswordProperty);
        }

        // Setter for the attached property
        public static void SetBoundPassword(DependencyObject obj, SecureString value)
        {
            obj.SetValue(BoundPasswordProperty, value);
        }

        // This attached property determines whether the PasswordBox should "attach" the helper
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached(
                "Attach",                            // Name of the attached property
                typeof(bool),                        // Boolean: true = attach, false = detach
                typeof(PasswordBoxHelper),           // Owner type
                new PropertyMetadata(false, OnAttachChanged)); // Callback when value changes

        // Getter for Attach property
        public static bool GetAttach(DependencyObject obj) => (bool)obj.GetValue(AttachProperty);

        // Setter for Attach property
        public static void SetAttach(DependencyObject obj, bool value) => obj.SetValue(AttachProperty, value);

        // This method is called whenever the Attach property changes
        private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // extra validation, make sure we are dealing with a PasswordBox
            if (d is PasswordBox passwordBox)
            {
                // If Attach is true, subscribe to PasswordChanged event
                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
                // If Attach is false, unsubscribe to prevent memory leaks
                else
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }
            }
        }

        // Event handler for PasswordBox.PasswordChanged
        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Cast the sender to PasswordBox
            var passwordBox = sender as PasswordBox;

            // Update the BoundPassword attached property with the current SecurePassword
            // This triggers the binding to the LoginViewModel SecureString property
            SetBoundPassword(passwordBox, passwordBox.SecurePassword);
        }
    }
}

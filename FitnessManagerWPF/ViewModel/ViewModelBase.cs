using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FitnessManagerWPF.ViewModel
{
    // Abstract class, as we don't want object creation to be possible.
    public abstract class ViewModelBase : INotifyPropertyChanged // Implements INotifyPropertyChanged by default
    {
        public event PropertyChangedEventHandler PropertyChanged; // PropertyChangedEventHandler
                                                                  // is the event responsible for updating the UI

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) // get's the object/sender
                                                                                        // that called the OnPropertyChanged
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // if PropertyChanged invoke event
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            // Check if the current value (field) is equal to the new value.
            // If they are the same, no property needs to be set
            if (Equals(field, value))
                return false; // Nothing changed, so return false.

            // Update the backing field with the new value
            field = value;

            // Notify any data bindings that this property has changed.
            // This triggers INotifyPropertyChanged, so the UI updates automatically.
            OnPropertyChanged(propertyName);

            // Return true to indicate the value was changed
            return true;
        }

    }
}

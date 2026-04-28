using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace clavierdor.ViewModels;

// Classe de base qui notifie l'interface quand une propriete change
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // Met a jour une propriete et previent l'interface si la valeur change
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    // Force la notification d'une propriete calculee
    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

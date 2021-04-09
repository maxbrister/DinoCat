using System.ComponentModel;

namespace DinoCat.State
{
    public interface IImplicitStateScope
    {
        void OnPropertyChanged(object? sender, PropertyChangedEventArgs args);
    }
}

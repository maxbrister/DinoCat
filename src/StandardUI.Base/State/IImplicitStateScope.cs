using System.ComponentModel;

namespace Microsoft.StandardUI.State
{
    public interface IImplicitStateScope
    {
        void OnPropertyChanged(object? sender, PropertyChangedEventArgs args);
    }
}

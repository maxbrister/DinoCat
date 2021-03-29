using DinoCat.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace DinoCat.Wpf
{
    [ContentProperty("Children")]
    public class DinoRow : Host
    {
        public DinoRow()
        {
            Children.CollectionChanged += Children_CollectionChanged;
            RootElement = () => new Row(Children.Select(child => child.Expand).ToArray());
        }

        public DinoRowChildrenCollection Children { get; } = new();

        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (var old in e.OldItems)
                    ((DinoExpand)old).PropertyChanged -= DinoRow_PropertyChanged;

            if (e.NewItems != null)
                foreach (var newItem in e.NewItems)
                    ((DinoExpand)newItem).PropertyChanged += DinoRow_PropertyChanged;
        }

        private void DinoRow_PropertyChanged(object? sender, global::System.ComponentModel.PropertyChangedEventArgs e) =>
            Refresh();
    }

    public class DinoRowChildrenCollection : ObservableCollection<DinoExpand>
    {
    }
}

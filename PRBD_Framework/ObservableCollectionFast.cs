using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRBD_Framework {

    // see: https://forums.xamarin.com/discussion/29925/observablecollection-addrange
    public class ObservableCollectionFast<T> : ObservableCollection<T> {
        public ObservableCollectionFast() : base() { }

        public ObservableCollectionFast(IEnumerable<T> collection) : base(collection) { }

        public ObservableCollectionFast(List<T> list) : base(list) { }

        public void Reset(IEnumerable<T> range) {
            this.Items.Clear();

            foreach (var item in range) {
                Items.Add(item);
            }

            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}

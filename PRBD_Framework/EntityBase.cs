using System.ComponentModel;

namespace PRBD_Framework {
    public class EntityBase<C> : ErrorManager, INotifyPropertyChanged where C : DbContextBase, new() {
        public event PropertyChangedEventHandler PropertyChanged;

        public EntityBase() {
        }

        public void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static C Context { get => ApplicationBase.Context<C>(); }
    }
}
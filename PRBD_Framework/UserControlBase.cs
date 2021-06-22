using System;
using System.Windows.Controls;

namespace PRBD_Framework {
    public class UserControlBase : UserControl, IDisposable {
        private bool disposed = false;

        public virtual void Dispose() {
            if (!disposed) {
                disposed = true;
                (DataContext as IDisposable)?.Dispose();
            }
        }
    }
}

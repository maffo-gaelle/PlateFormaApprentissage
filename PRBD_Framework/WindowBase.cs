using System;
using System.ComponentModel;
using System.Windows;

namespace PRBD_Framework {
    public class WindowBase : Window, IDisposable {
        private bool disposed = false;

        public virtual void Dispose() {
            if (!disposed) {
                disposed = true;
                (DataContext as IDisposable)?.Dispose();
            }
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            Dispose();
        }
    }
}

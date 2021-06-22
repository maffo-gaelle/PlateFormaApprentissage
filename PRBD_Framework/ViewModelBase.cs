using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PRBD_Framework {

    public abstract class ViewModelBase<C> : INotifyPropertyChanged, INotifyDataErrorInfo, IErrorManager, IDisposable where C : DbContextBase, new() {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected ErrorManager errors;
        protected bool disposed = false;

        protected C Context { get => ApplicationBase.Context<C>(); }

        public ViewModelBase() {
            errors = new ErrorManager(ErrorsChanged);
            ApplicationBase.Register(this, ApplicationBaseMessages.MSG_REFRESH_DATA, OnRefreshData);
        }

        protected abstract void OnRefreshData();

        public void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertyChanged(INotifyPropertyChanged source, string propertyName) {
            PropertyChanged?.Invoke(source, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
		/// Checks if a property already matches a desired value. Sets the property and
		/// notifies listeners only when necessary. (origin: Prism)
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="storage">Reference to a property with both getter and setter.</param>
		/// <param name="value">Desired value for the property.</param>
		/// <param name="propertyName">Name of the property used to notify listeners. This
		/// value is optional and can be provided automatically when invoked from compilers that
		/// support CallerMemberName.</param>
		/// <returns>True if the value was changed, false if the existing value matched the
		/// desired value.</returns>
		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary. (origin: Prism)
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <param name="onChanged">Action that is called after the property value has been changed.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);

            return true;
        }

        public void RaisePropertyChanged(params string[] propertyNames) {
            foreach (var n in propertyNames)
                RaisePropertyChanged(n);
        }

        public void RaisePropertyChanged(INotifyPropertyChanged source, params string[] propertyNames) {
            foreach (var n in propertyNames)
                RaisePropertyChanged(source, n);
        }

        /// <summary>
        /// Déclenche le PropertyChanged sur toutes les propriétés publiques.
        /// </summary>
        public void RaisePropertyChanged() {
            var type = GetType();
            foreach (var n in type.GetProperties())
                RaisePropertyChanged(n.Name);
        }

        public void RaisePropertyChanged(INotifyPropertyChanged source) {
            var type = GetType();
            foreach (var n in type.GetProperties())
                RaisePropertyChanged(source, n.Name);
        }

        public void AddError(string propertyName, string error) {
            errors.AddError(propertyName, error);
        }

        public void SetError(string propertyName, string error) {
            errors.SetError(propertyName, error);
        }

        public void ClearErrors(string propertyName) {
            errors.ClearErrors(propertyName);
        }

        public void ClearErrors() {
            errors.ClearErrors();
        }

        public void RaiseErrors() {
            errors.RaiseErrors();
            foreach (var key in errors.Errors.Keys) {
                RaisePropertyChanged(key);
            }
        }

        public void RaiseErrorsChanged(string propertyName) {
            errors.RaiseErrorsChanged(propertyName);
        }

        public IEnumerable GetErrors(string propertyName) {
            return errors.GetErrors(propertyName);
        }

        public Dictionary<string, ICollection<string>> Errors {
            get => errors.Errors;
        }

        public void AddErrors(Dictionary<string, ICollection<string>> errors) {
            this.errors.AddErrors(errors);
        }

        public void SetErrors(Dictionary<string, ICollection<string>> errors) {
            this.errors.SetErrors(errors);
        }

        public void SetErrors(string propertyName, IEnumerable errors) {
            this.errors.SetErrors(propertyName, errors);
        }

        public virtual bool Validate() {
            return errors.Validate();
        }

        public bool HasErrors {
            get => errors.HasErrors;
        }

        public virtual void Dispose() {
            if (!disposed) {
                //Console.WriteLine("Disposing " + this);
                ApplicationBase.UnRegister(this);

                // Supprime les bindings dont ce modèle de vue est la source ou la destination
                this.Unbind();

                disposed = true;
            }
        }

        public static void Register(object owner, Enum message, Action callback) {
            ApplicationBase.Register(owner, message, callback);
        }

        public static void Register<T>(object owner, Enum message, Action<T> callback) {
            ApplicationBase.Register(owner, message, callback);
        }

        public static void NotifyColleagues(Enum message, object parameter) {
            ApplicationBase.NotifyColleagues(message, parameter);
        }

        public static void NotifyColleagues(Enum message) {
            ApplicationBase.NotifyColleagues(message);
        }

        public static void UnRegister(object owner) {
            ApplicationBase.UnRegister(owner);
        }

    }
}

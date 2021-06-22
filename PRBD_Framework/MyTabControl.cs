using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PRBD_Framework {
    public class MyTabControl : TabControl, IDisposable {
        public TabItem Add(ContentControl content, string header, object tag = null) {

            // crée le tab
            var tab = new TabItem() {
                Content = content,
                Header = header,
                Tag = tag
            };

            if (HasCloseButton) {
                // crée le header du tab avec le bouton de fermeture
                var headerPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                headerPanel.Children.Add(new TextBlock() { Text = header, VerticalAlignment = VerticalAlignment.Center });
                var closeButton = new Button() {
                    Content = "X",
                    FontWeight = FontWeights.Bold,
                    Background = Brushes.Transparent,
                    Foreground = Brushes.Red,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(10, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    ToolTip = "Close"
                };
                headerPanel.Children.Add(closeButton);
                tab.Header = headerPanel;

                closeButton.Click += (o, e) => {
                    Items.Remove(tab);
                };
            }

            // ajoute cet onglet à la liste des onglets existant du TabControl
            Items.Add(tab);
            // exécute la méthode Focus() de l'onglet pour lui donner le focus (càd l'activer)
            SetFocus(tab);
            return tab;
        }

        public void RenameTab(TabItem tab, string newName) {
            if (tab.Header is StackPanel stackPanel) {
                (stackPanel.Children[0] as TextBlock).Text = newName;
            } else
                tab.Header = newName;
        }

        private void Dispose(IEnumerable<TabItem> tabs) {
            var tabsToDispose = new List<TabItem>(tabs);
            foreach (TabItem tab in tabsToDispose) {
                if (tab.Content != null) {
                    var content = (tab.Content as ContentControl).Content as FrameworkElement;
                    if (content.DataContext != null && content.DataContext is IDisposable) {
                        (content.DataContext as IDisposable).Dispose();
                        content.DataContext = null;
                    }
                }
            }
        }

        public void Dispose() {
            Dispose(Items.OfType<TabItem>());
        }

        public TabItem FindByTag(String tag) {
            return (from TabItem t in Items where tag == t.Tag?.ToString() select t).FirstOrDefault();
        }

        public TabItem FindByHeader(String header) {
            return (from TabItem t in Items where header == t.Header?.ToString() select t).FirstOrDefault();
        }

        public void SetFocus(TabItem tab) {
            Dispatcher.InvokeAsync(() => {
                tab.Focus();
                SelectedItem = tab;
            });
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
            // vérifie si au moins un des tabs sélectionnés est visible
            bool visible = false;
            foreach (TabItem tab in e.AddedItems) {
                if (tab.Visibility == Visibility.Visible) {
                    visible = true;
                    break;
                }
            }
            if (!visible) {
                // cherche un tab visible
                foreach (TabItem tab in Items)
                    if (tab.Visibility == Visibility.Visible) {
                        SelectedItem = tab;
                        e.Handled = false;
                        break;
                    }
            }
            base.OnSelectionChanged(e);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
            base.OnItemsChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Remove) {
                Dispose(e.OldItems.OfType<TabItem>());
            } else if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (TabItem tab in e.NewItems) {
                    tab.MouseDown += (o, e) => {
                        if (e.ChangedButton == MouseButton.Middle &&
                            e.ButtonState == MouseButtonState.Pressed) {
                            Items.Remove(o);
                        }
                    };
                    tab.PreviewKeyDown += (o, e) => {
                        if (e.Key == Key.W && Keyboard.IsKeyDown(Key.LeftCtrl)) {
                            Items.Remove(o);
                        }
                    };
                }
            }
        }

        public bool HasCloseButton {
            get { return (bool)GetValue(HasCloseButtonProperty); }
            set { SetValue(HasCloseButtonProperty, value); }
        }

        public static readonly DependencyProperty HasCloseButtonProperty =
            DependencyProperty.Register("HasCloseButton", typeof(bool), typeof(MyTabControl), new PropertyMetadata(false));
    }
}

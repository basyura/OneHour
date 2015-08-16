using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace OneHour
{
    public class Notifier
    {
        /// <summary></summary>
        private NotifyIcon _icon;
        /// <summary></summary>
        private MainWindow _window;
        /// <summary>
        /// 
        /// </summary>
        public void Run()
        {
            _icon = new NotifyIcon()
            {
                Icon        = Properties.Resources.WatchIcon,
                Visible     = true,
                ContextMenu = CreateMenus(),
            };

            _icon.Click += (sender, ev) => {
                MessageBox.Show("hi");
            };

            WatchStart();
        }
        /// <summary>
        /// 
        /// </summary>
        private void WatchStart()
        {
            new TaskFactory().StartNew(() =>
            {
                Thread.Sleep(1000 * 10);
                BeginInvoke(() => 
                {
                    if (_window == null)
                    {
                        _window = new MainWindow();
                        _window.Show();
                    }
                    else
                    {
                        _window.Visibility = System.Windows.Visibility.Visible;
                        _window.Activate();
                    }

                    WindowInteropHelper wih = new WindowInteropHelper(_window);
                    Rectangle bounds = Screen.FromHandle(wih.Handle).Bounds;
                    _window.Left = bounds.Width  / 2 - (_window.Width  / 2);
                    _window.Top  = bounds.Height / 2 - (_window.Height / 2);
                });
            })
            .ContinueWith((task) => WatchStart());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ContextMenu CreateMenus()
        {
            List<MenuItem> items = new List<MenuItem>();

            items.Add(new MenuItem("Exit", (sender, ev) => {
                _icon.Dispose();
                System.Windows.Application.Current.Shutdown();
            }));


            return new ContextMenu(items.ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        private void BeginInvoke(Action action)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(action);
        }
    }
}

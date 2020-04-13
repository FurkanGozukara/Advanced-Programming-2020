using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace week_11_lock_example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private static Dictionary<int, int> dicNoLock = new Dictionary<int, int>();

        private void btnNoLockSync_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => { noLockAdd(); });
        }

        private void noLockAdd()
        {

            while (true)
            {
                Task.Factory.StartNew(() => { RandomAdd(); });
                // System.Threading.Thread.Sleep(1);
            }

        }

        private void RandomAdd()
        {
            int? id1 = Task.CurrentId;
            int irId = Convert.ToInt32(id1);
            Random rand = new Random();
            int irRandNumber = rand.Next(1, 100);
            if (dicNoLock.ContainsKey(irRandNumber))
            {

            }
            else
                dicNoLock.Add(irRandNumber, irId);
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {

            Task.Factory.StartNew(() => { LockAdd(); });
            // System.Threading.Thread.Sleep(1);

        }

        private void LockAdd()
        {
            Int64 irCounter = 0;
            DateTime dtStart = DateTime.Now;
            while (true)
            {
                Task.Factory.StartNew(() => { RandomAddSync(); });
                // System.Threading.Thread.Sleep(1);
                irCounter++;

                if (irCounter % 1000000 ==0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        lblSpeed.Content = irCounter.ToString("N0") +"\t"+ (DateTime.Now-dtStart).TotalSeconds.ToString("N0");
                    }));
                }
            }

        }

        ConcurrentDictionary<int, int> concDic = new ConcurrentDictionary<int, int>();

        private static object lock_dic = new object();

        private void RandomAddSync()
        {
            int? id1 = Task.CurrentId;
            int irId = Convert.ToInt32(id1);
            Random rand = new Random();
            int irRandNumber = rand.Next(1, 100);

            //concDic.TryAdd(irRandNumber, irId); this looks like slower
            //more depeer tests are required to verify


            lock (lock_dic)
            {
                if (dicNoLock.ContainsKey(irRandNumber))
                {

                }
                else
                    dicNoLock.Add(irRandNumber, irId);
            }

        }
    }
}

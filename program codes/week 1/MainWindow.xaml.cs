using System;
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
using public_variables;
using System.IO;

namespace week_1
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

        //compose a class that has record id and random number (int 64)
        //compose 100 object from this class and add them to a list
        //add elements of this list to the list box as record id + tab tab + random number

        private void BtnRandomFill_Click(object sender, RoutedEventArgs e)
        {
            fillListBox();
        }

        private void fillListBox()
        {
            Random randomGenerator = new Random();


            List<randomNumbers> lstRandVariables = new List<randomNumbers>();
            for (int i = 0; i < 100; i++)
            {
                randomNumbers myRand = new randomNumbers();
                myRand.irId = staticVariables.irGlobalId;
                staticVariables.irGlobalId++;
                myRand.irRandNumber = Convert.ToUInt64(randomGenerator.Next());
                lstRandVariables.Add(myRand);
            }

            foreach (var vrElement in lstRandVariables)
            {
                listBoxRands.Items.Add($"{vrElement.irId}\t\t{vrElement.irRandNumber.ToString("N0")}");
            }
        }

        private void BtnReadToDic_Click(object sender, RoutedEventArgs e)
        {
            readFromDic();
        }

        private void readFromDic()
        {
            staticVariables.readFromListBox(ref listBoxRands);
        }

        private void BtnWriteToFile_Click(object sender, RoutedEventArgs e)
        {
            //using dispose or close used resources after exited from scope {}
            using (StreamWriter swWrite = new StreamWriter("dosya1.txt"))
            {
                swWrite.AutoFlush = true;
                foreach (var vrPeritem in staticVariables.dicGeneratedValues)
                {
                    swWrite.WriteLine($"{vrPeritem.Key}\t\t{vrPeritem.Value}");
                }
            }

            //
            File.WriteAllLines("dosya2.txt", staticVariables.dicGeneratedValues.Select(pr => pr.Key + "\t\t" + pr.Value));
        }
    }
}

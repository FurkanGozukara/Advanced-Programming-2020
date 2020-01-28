using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace week_1
{
    public static class staticVariables
    {
        public static int irGlobalId = 1;

        public static Dictionary<string, string> dicGeneratedValues = new Dictionary<string, string>();

        public static void readFromListBox(ref ListBox myListBox)
        {
            foreach (var vrListBoxItem in myListBox.Items)
            {
                string srId = vrListBoxItem.ToString().Split('\t').First();
                string srNumber = vrListBoxItem.ToString().Split('\t').Last();
            }
        }
    }
   
}

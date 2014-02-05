using SmartWard.PDA.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SmartWard.PDA.Helpers
{
    public class NavigationHelper
    {
        public static void NavigateBack(PDAWindow w)
        {
            bool popStack = false;
            JournalEntry j = null;
            foreach (JournalEntry journal in w.ContentFrame.BackStack)
            {
                if (journal.Name.Equals(typeof(AddResourceView).Name))
                {
                    popStack = true;
                }
                else
                {
                    j = journal;
                }
                break;
            }

            if (popStack)
            {
                w.ContentFrame.RemoveBackEntry();
                w.ContentFrame.NavigationService.GoBack();
            }
            else
            {
                w.ContentFrame.NavigationService.GoBack();
                //NavigationCommands.NavigateJournal.Execute(j, w.ContentFrame);
            }
        }
    }
}

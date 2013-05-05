using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wygwam.Windows.ViewModels
{
    public class SelectableItemChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public SelectableListItem<T> ChangedItem
        {
            get;
            set;
        } 
    }
}

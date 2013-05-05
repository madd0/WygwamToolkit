//-----------------------------------------------------------------------
// <copyright file="SelectableListView.cs" company="">
//     Copyright (c) mauri_000, . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Wygwam.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Data;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls.Primitives;


    /// <summary>
    /// TODO: Provide summary section in the documentation header.
    /// </summary>
    public class SelectableListView : ListView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            Binding binding = new Binding()
            {
                Source = item,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("IsSelected")
            };

            ((SelectorItem)element).SetBinding(SelectorItem.IsSelectedProperty, binding);
        }
    }
}

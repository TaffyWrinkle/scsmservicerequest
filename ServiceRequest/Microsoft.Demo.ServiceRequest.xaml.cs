﻿/*
Microsoft.Demo.SeviceRequest.xaml.cs
 
Description:
Microsoft.Demo.SeviceRequest.xaml.cs contains the code behind 
business logic for the Service Request form.
 
Copyright(c) Microsoft.  All rights reserved.
This code is licensed under the Microsoft Public License.
http://www.microsoft.com/opensource/licenses.mspx
 
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND,
EITHER EXPRESSED OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES
OF FITNESS FOR A PARITCULAR PURPOSE, MERCHANTABILITY, OR
NON-INFRINGEMENT.
 
Original Author: Travis Wright (twright@microsoft.com)
Original Creation Date: Jan 7, 2010
Original Version: 1.0
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//Requires Microsoft.EnterpriseManagement.UI.SMControls
using Microsoft.EnterpriseManagement.UI.WpfControls;    //Contains InstancePickerDialog, UserPicker, and ListPicker

// Requires Microsoft.EnterpriseManagement.UI.Foundation
using Microsoft.EnterpriseManagement.UI.DataModel;      //Contains IDataItem

//Requires Microsoft.EnterpriseManagement.UI.WpfViews
using Microsoft.EnterpriseManagement.UI.WpfViews;       //Contains FormEvents

//Requires Microsoft.EnterpriseManagement.FormsInfra
using Microsoft.EnterpriseManagement.UI.FormsInfra;     //Contains PreviewFormCommandEventArgs

//Requires Microsoft.EnterpriseManagement.ServiceManager.Application.Common
using Microsoft.EnterpriseManagement.ServiceManager.Application.Common; //Contains ConsoleContextHelper


namespace Microsoft.Demo.ServiceRequest
{
    /// <summary>
    /// Interaction logic for ServiceRequest.xaml
    /// </summary>
    public partial class ServiceRequest : UserControl
    {
        public static Guid guidClassificationEnumRoot = new Guid("AB422C80-3349-3792-F808-5BFD454EBD2B"); //Sealed with testkeys.snk
        //public static Guid guidClassificationEnumRoot = new Guid("4A7368B0-10F9-E93E-F399-0E250E05F108"); //Unsealed

        public ServiceRequest()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddItemToListView(this.lvAffectedCIs, new Guid("62F0BE9F-ECEA-E73C-F00D-3DD78A7422FC"));
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveItemFromWorkItemListView(this.lvAffectedCIs);
        }

        internal static void AddItemToListView(ListView listView, Guid classId)
        {
            if (listView != null && listView.Items != null)
            {
                /* NOTE: The use of the IDataItem and InstancePickerDialog interfaces here is not supported/documented.
                 * This interface may change in the future and no migration path is guaranteed by Microsoft.
                */ 
                InstancePickerDialog ciPicker = new InstancePickerDialog();
                ciPicker.ClassId = classId;
                ciPicker.SelectionMode = SelectionMode.Multiple;

                if (listView.Items.Count > 0)
                {
                    ciPicker.SetPickedInstances((Collection<IDataItem>)listView.ItemsSource);
                }

                bool? result = ciPicker.ShowDialog();
                if (result != null && result == true)
                {
                    Collection<IDataItem> items = listView.ItemsSource as Collection<IDataItem>;
                    foreach (IDataItem item in ciPicker.RemovedInstances)
                        items.Remove(item);
                    foreach (IDataItem item in ciPicker.PickedInstances)
                        if (!items.Contains(item))
                            items.Add(item);
                }
            }
        }

        internal static void RemoveItemFromWorkItemListView(ListView listView)
        {
            if (listView.ItemsSource == null ||
                listView.SelectedItems == null ||
                listView.SelectedItems.Count == 0)
            {
                return;
            }

            ItemCollection items = listView.ItemsSource as ItemCollection;
            if (items != null)
            {
                foreach (object obj in new ArrayList(listView.SelectedItems))
                {
                    /* NOTE: The use of the IDataItem interface here is not supported/documented.
                     * This interface may change in the future and no migration path is guaranteed by Microsoft.
                    */ 
                    items.Remove(obj as IDataItem);
                }
            }
        }

        internal static void OpenSelectedItem(ListViewItem listviewItem)
        {

        }

        private void OnPreviewSubmit(object sender, PreviewFormCommandEventArgs e)
        {
            /* NOTE: The use of the IDataItem interface here is not supported/documented.
            * This interface may change in the future and no migration path is guaranteed by Microsoft.
            */
            IDataItem itemServiceRequest = this.DataContext as IDataItem;

            //Return here because all validation will be done in pure XAML. Displayname will be done with a ModifyPropertyAction like the Title of the FormWindow.
            return;

            if (String.IsNullOrEmpty(this.txtTitle.Text.Trim()))
            {
                e.Cancel = true;
                MessageBox.Show("Please provide a title"); 
            }
            else
            {
                itemServiceRequest["DisplayName"] = itemServiceRequest["Id"] + " - " + itemServiceRequest["Title"];
            }
            
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.AddHandler(FormEvents.PreviewSubmitEvent, new EventHandler<PreviewFormCommandEventArgs>(this.OnPreviewSubmit));
        }

        private void lvAffectedCIs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IDataItem emoProjectionObject = (IDataItem)lvAffectedCIs.SelectedItem;
            ConsoleContextHelper.Instance.PopoutForm(emoProjectionObject);
        }
       
    }
}
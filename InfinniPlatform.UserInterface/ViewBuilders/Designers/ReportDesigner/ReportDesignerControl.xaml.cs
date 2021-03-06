﻿using System;
using System.Windows.Controls;

namespace InfinniPlatform.UserInterface.ViewBuilders.Designers.ReportDesigner
{
    /// <summary>
    ///     Элемент управления для редактирования отчетов.
    /// </summary>
    sealed partial class ReportDesignerControl : UserControl
    {
        public ReportDesignerControl()
        {
            InitializeComponent();

            Designer.EditValueChanged += OnEditValueChanged;
        }

        public object EditValue
        {
            get { return Designer.EditValue; }
            set { Designer.EditValue = value; }
        }

        private void OnEditValueChanged(object sender, EventArgs e)
        {
            var handler = EditValueChanged;

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public event EventHandler EditValueChanged;
    }
}
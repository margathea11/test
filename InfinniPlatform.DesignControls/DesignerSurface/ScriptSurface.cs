﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using InfinniPlatform.DesignControls.Controls.Scripts;
using InfinniPlatform.DesignControls.Layout;
using InfinniPlatform.DesignControls.ObjectInspector;
using InfinniPlatform.DesignControls.PropertyDesigner;

namespace InfinniPlatform.DesignControls.DesignerSurface
{
    public partial class ScriptSurface : UserControl, ILayoutProvider, IInspectedItem
    {
        private readonly List<ScriptSourceObject> _scripts = new List<ScriptSourceObject>();

        public ScriptSurface()
        {
            InitializeComponent();

            gridBinding.DataSource = Scripts;
        }

        public List<ScriptSourceObject> Scripts
        {
            get { return _scripts; }
        }

        public ObjectInspectorTree ObjectInspector { get; set; }

        public dynamic GetLayout()
        {
            dynamic instanceLayout = new List<dynamic>();

            foreach (ILayoutProvider scriptSourceObject in Scripts)
            {
                dynamic instance = scriptSourceObject.GetLayout();
                instanceLayout.Add(instance);
            }
            return instanceLayout;
        }

        public void SetLayout(dynamic value)
        {
            throw new NotImplementedException();
        }

        public string GetPropertyName()
        {
            return "Scripts";
        }

        private void repositoryItemButtonEditSource_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            var scriptSource = Scripts.ElementAt(GridViewScripts.FocusedRowHandle).ScriptSource as IPropertiesProvider;
            if (scriptSource != null)
            {
                var form = new PropertiesForm();

                var validationRules = scriptSource.GetValidationRules();
                form.SetValidationRules(validationRules);
                var propertyEditors = scriptSource.GetPropertyEditors();
                form.SetPropertyEditors(propertyEditors);
                var simpleProperties = scriptSource.GetSimpleProperties();
                form.SetSimpleProperties(simpleProperties);
                var collectionProperties = scriptSource.GetCollections();
                form.SetCollectionProperties(collectionProperties);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    scriptSource.ApplySimpleProperties();
                    scriptSource.ApplyCollections();
                    GridViewScripts.HideEditor();
                    GridViewScripts.RefreshData();
                }
                else
                {
                    form.RevertChanges();
                }
            }
        }

        private void AddScriptButton_Click(object sender, EventArgs e)
        {
            var dataSourceObject = new ScriptSourceObject();
            Scripts.Add(dataSourceObject);
            dataSourceObject.ScriptSource = new Script();
            GridViewScripts.RefreshData();
        }

        private void DeleteScriptButton_Click(object sender, EventArgs e)
        {
            if (GridViewScripts.FocusedRowHandle >= 0)
            {
                Scripts.RemoveAt(GridViewScripts.FocusedRowHandle);
                GridViewScripts.RefreshData();
            }
        }

        private void GridViewScripts_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            var value = (ScriptSourceObject) GridViewScripts.GetRow(e.RowHandle);
            if (value == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(value.ScriptSourceName))
            {
                e.DisplayText = "<Script name not specified>";
            }
            else
            {
                e.DisplayText = value.ScriptSourceName;
            }
        }

        private void GEtLayoutButton_Click(object sender, EventArgs e)
        {
            dynamic value = GetLayout();

            var valueEdit = new ValueEdit();
            valueEdit.Value = value.ToString();
            valueEdit.ShowDialog();
        }

        public void ProcessJson(dynamic scripts)
        {
            Scripts.Clear();
            foreach (var source in scripts)
            {
                var scriptSourceObject = new ScriptSourceObject();

                var script = new Script();
                script.LoadProperties(source);
                scriptSourceObject.ScriptSource = script;

                Scripts.Add(scriptSourceObject);
            }
            GridViewScripts.RefreshData();
        }

        public void Clear()
        {
            _scripts.Clear();
            GridControlScripts.RefreshDataSource();
        }
    }
}
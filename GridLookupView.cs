using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;

namespace Ecis.Infrastructure.UI.Controls
{
    public partial class GridLookupView : GridLookUpEdit
    {
        public delegate void CreateDate(string value);
        public event CreateDate newDate;
        public GridLookupView()
        {
            InitializeComponent();
            InitDefaultValue();
        }

        private void InitDefaultValue()
        {
            this.Properties.TextEditStyle = TextEditStyles.Standard;
            this.Properties.ImmediatePopup = true;
            this.ProcessNewValue += gridLookUpEdit_ProcessNewValue;
            //SetGridLookUpEditMoreColumnFilter(this.Properties);
        }
        private void gridLookUpEdit_ProcessNewValue(object sender, ProcessNewValueEventArgs e)
        {
            var sender1 = ((GridLookUpEdit)sender);
            if (sender1 != null)
            {
                var Edit = sender1.Properties;
                if (e.DisplayValue == null || Edit.NullText.Equals(e.DisplayValue) || string.Empty.Equals(e.DisplayValue))
                    return;
                if (newDate != null)
                {
                    newDate(e.DisplayValue.ToString());
                }
                e.Handled = true;
            }

        }
         //<summary>
         //设置GridLookUpEdit多列过滤
         //</summary>
         //<param name="repGLUEdit">GridLookUpEdit的知识库，eg:gridlookUpEdit.Properties</param>
        public void SetGridLookUpEditMoreColumnFilter(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repGLUEdit)
        {
            repGLUEdit.EditValueChanging += (sender, e) =>
            {
                this.BeginInvoke(new System.Windows.Forms.MethodInvoker(() =>
                {
                    GridLookUpEdit edit = sender as GridLookUpEdit;
                    DevExpress.XtraGrid.Views.Grid.GridView view = edit.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
                    //获取GriView私有变量
                    System.Reflection.FieldInfo extraFilter = view.GetType().GetField("extraFilter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    List<DevExpress.Data.Filtering.CriteriaOperator> columnsOperators = new List<DevExpress.Data.Filtering.CriteriaOperator>();
                    foreach (GridColumn col in view.VisibleColumns)
                    {
                        //这里判断哪一种条件的列头是否可以进行搜索，你也可以自己修改自己需要的情况！
                        if (col.Visible && col.ColumnType == typeof(string))
                            columnsOperators.Add(new DevExpress.Data.Filtering.FunctionOperator(DevExpress.Data.Filtering.FunctionOperatorType.Contains,
                                new DevExpress.Data.Filtering.OperandProperty(col.FieldName),
                                new DevExpress.Data.Filtering.OperandValue(edit.Text)));
                    }

                    string filterCondition = new DevExpress.Data.Filtering.GroupOperator(DevExpress.Data.Filtering.GroupOperatorType.Or, columnsOperators).ToString();

                    extraFilter.SetValue(view, filterCondition);
                    //获取GriView中处理列过滤的私有方法
                    System.Reflection.MethodInfo ApplyColumnsFilterEx = view.GetType().GetMethod("ApplyColumnsFilterEx", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    ApplyColumnsFilterEx.Invoke(view, null);
                }));
            };
        }

       

       
    }
}

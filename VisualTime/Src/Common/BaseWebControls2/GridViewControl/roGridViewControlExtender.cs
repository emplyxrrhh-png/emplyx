using AjaxControlToolkit;
using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: System.Web.UI.WebResource("Robotics.WebControls2.GridViewControl.GridViewControlBehavior.js", "text/javascript")]

namespace Robotics.WebControls2.GridViewControl
{
    /// <summary>
    ///
    /// </summary>
    [Designer(typeof(roGridViewControlDesigner))]
    [ClientScriptResource("Robotics.WebControls2.GridViewControl.GridViewControlBehavior", "Robotics.WebControls2.GridViewControl.GridViewControlBehavior.js")]
    [TargetControlType(typeof(GridView))]
    public class roGridViewControlExtender : ExtenderControlBase
    {
        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        public string HeaderCellHoverCssClass
        {
            get
            {
                return this.GetPropertyValue("HeaderCellHoverCssClass", "");
            }
            set
            {
                this.SetPropertyValue("HeaderCellHoverCssClass", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        public string HeaderCellSelectCssClass
        {
            get
            {
                return this.GetPropertyValue("HeaderCellSelectCssClass", "");
            }
            set
            {
                this.SetPropertyValue("HeaderCellSelectCssClass", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        public string CellHoverCssClass
        {
            get
            {
                return this.GetPropertyValue("CellHoverCssClass", "");
            }
            set
            {
                this.SetPropertyValue("CellHoverCssClass", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        public string CellSelectCssClass
        {
            get
            {
                return this.GetPropertyValue("CellSelectCssClass", "");
            }
            set
            {
                this.SetPropertyValue("CellSelectCssClass", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        public string ColumnHoverCssClass
        {
            get
            {
                return this.GetPropertyValue("ColumnHoverCssClass", "");
            }
            set
            {
                this.SetPropertyValue("ColumnHoverCssClass", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        public string ColumnSelectCssClass
        {
            get
            {
                return this.GetPropertyValue("ColumnSelectCssClass", "");
            }
            set
            {
                this.SetPropertyValue("ColumnSelectCssClass", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        public string RowHoverCssClass
        {
            get
            {
                return this.GetPropertyValue("RowHoverCssClass", "");
            }
            set
            {
                this.SetPropertyValue("RowHoverCssClass", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        public string RowSelectCssClass
        {
            get
            {
                return this.GetPropertyValue("RowSelectCssClass", "");
            }
            set
            {
                this.SetPropertyValue("RowSelectCssClass", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue(true)]
        public Boolean KeysNavigation
        {
            get
            {
                return this.GetPropertyValue("KeysNavigation", true);
            }
            set
            {
                this.SetPropertyValue("KeysNavigation", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        public string SelectedRowIndexCookie
        {
            get
            {
                return this.GetPropertyValue("SelectedRowIndexCookie", "");
            }
            set
            {
                this.SetPropertyValue("SelectedRowIndexCookie", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        public string SelectedColIndexCookie
        {
            get
            {
                return this.GetPropertyValue("SelectedColIndexCookie", "");
            }
            set
            {
                this.SetPropertyValue("SelectedColIndexCookie", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        [Browsable(false)]
        public string DataRowCssClass
        {
            get
            {
                return this.GetPropertyValue("DataRowCssClass", "");
            }
            set
            {
                this.SetPropertyValue("DataRowCssClass", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        [Browsable(false)]
        public string AlternateDataRowCssClass
        {
            get
            {
                return this.GetPropertyValue("AlternateDataRowCssClass", "");
            }
            set
            {
                this.SetPropertyValue("AlternateDataRowCssClass", value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var target = this.TargetControl as GridView;
            this.DataRowCssClass = target.RowStyle.CssClass;
            this.AlternateDataRowCssClass = target.AlternatingRowStyle.CssClass;
        }
    }
}
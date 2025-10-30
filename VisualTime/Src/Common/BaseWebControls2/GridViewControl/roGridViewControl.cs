using System;
using System.ComponentModel;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Robotics.WebControls2.GridViewControl
{
    public class roGridViewControl : GridView
    {
        private const string FrozenTopCssClass = "frozenTop";
        private const string FrozenLeftCssClass = "frozenLeft";
        private const string FrozenLeftHeaderCssClass = "frozenLeftHeader";

        private const string ASCENDING = " ASC";
        private const string DESCENDING = " DESC";

        private HiddenField hdnRow;
        private HiddenField hdnCol;

        public roGridViewControl()
        {
            this.hdnRow = new HiddenField();
            this.hdnRow.ID = String.Format(CultureInfo.InvariantCulture, "__gv{0}__hdnRow", this.ID);
            this.hdnRow.Value = "-1";
            this.hdnCol = new HiddenField();
            this.hdnCol.ID = String.Format(CultureInfo.InvariantCulture, "__gv{0}__hdnCol", this.ID);
            this.hdnCol.Value = "-1";
        }

        #region Properties

        [Browsable(true), DefaultValue(true), Category("Behavior"), DisplayName("Freeze Header")]
        public bool FreezeHeader
        {
            get
            {
                object val = this.ViewState["FreezeHeader"];
                if (null == val)
                {
                    return true;
                }

                return (bool)val;
            }
            set
            {
                this.ViewState["FreezeHeader"] = value;
            }
        }

        [Browsable(true), DefaultValue(-1), Category("Behavior"), DisplayName("Freeze Column Index")]
        public int FreezeColumn
        {
            get
            {
                object val = this.ViewState["FreezeColumn"];
                if (null == val)
                {
                    return -1;
                }

                return (int)val;
            }
            set
            {
                this.ViewState["FreezeColumn"] = value;
            }
        }

        [Browsable(true), DefaultValue(ScrollBars.None), Category("Behavior")]
        public ScrollBars Scrolling
        {
            get
            {
                object val = this.ViewState["Scrolling"];
                if (null == val)
                {
                    return ScrollBars.None;
                }

                return (ScrollBars)val;
            }
            set
            {
                this.ViewState["Scrolling"] = value;
            }
        }

        public Unit ScrollWidth
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
            }
        }

        public override Unit Width
        {
            get
            {
                object val = this.ViewState["DivWidth"];
                if (null == val)
                {
                    return Unit.Empty;
                }

                return (Unit)val;
            }
            set
            {
                this.ViewState["DivWidth"] = value;
            }
        }

        public Unit ScrollHeight
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
            }
        }

        public override Unit Height
        {
            get
            {
                object val = this.ViewState["DivHeight"];
                if (null == val)
                {
                    return Unit.Empty;
                }

                return (Unit)val;
            }
            set
            {
                this.ViewState["DivHeight"] = value;
            }
        }

        private string OverflowX
        {
            get
            {
                if (this.Scrolling == ScrollBars.Horizontal || this.Scrolling == ScrollBars.Both)
                {
                    return "scroll";
                }
                else if (this.Scrolling == ScrollBars.Auto)
                {
                    return "auto";
                }
                else
                {
                    return "visible";
                }
            }
        }

        private string OverflowY
        {
            get
            {
                if (this.Scrolling == ScrollBars.Vertical || this.Scrolling == ScrollBars.Both)
                {
                    return "scroll";
                }
                else if (this.Scrolling == ScrollBars.Auto)
                {
                    return "auto";
                }
                else
                {
                    return "visible";
                }
            }
        }

        [Browsable(true), DefaultValue("")]
        public string CellHoverCssClass
        {
            get
            {
                object val = this.ViewState["CellHoverCssClass"];
                if (null == val)
                {
                    return "";
                }
                return (string)val;
            }
            set
            {
                this.ViewState["CellHoverCssClass"] = value;
            }
        }

        [Browsable(true), DefaultValue("")]
        public string CellSelectCssClass
        {
            get
            {
                object val = this.ViewState["CellSelectCssClass"];
                if (null == val)
                {
                    return "";
                }
                return (string)val;
            }
            set
            {
                this.ViewState["CellSelectCssClass"] = value;
            }
        }

        [Browsable(true), DefaultValue("")]
        public string RowSelectedControlID
        {
            get
            {
                object val = this.ViewState["RowSelectedControlID"];
                if (null == val)
                {
                    return "";
                }
                return (string)val;
            }
            set
            {
                this.ViewState["RowSelectedControlID"] = value;
            }
        }
        [Browsable(true), DefaultValue("")]
        public string ColSelectedControlID
        {
            get
            {
                object val = this.ViewState["ColSelectedControlID"];
                if (null == val)
                {
                    return "";
                }
                return (string)val;
            }
            set
            {
                this.ViewState["ColSelectedControlID"] = value;
            }
        }

        [Browsable(true), DefaultValue(SortDirection.Ascending)]
        public SortDirection _SortDirection
        {
            get
            {
                if (this.ViewState["sortDirection"] == null)
                    this.ViewState["sortDirection"] = SortDirection.Ascending;
                return (SortDirection)this.ViewState["sortDirection"];
            }
            set
            {
                this.ViewState["sortDirection"] = value;
            }
        }
        [Browsable(true), DefaultValue("")]
        public string _SortDirectionStr
        {
            get
            {
                if (this._SortDirection == SortDirection.Ascending)
                    return "ASC";
                else
                    return "DESC";
            }
        }
        [Browsable(true), DefaultValue("")]
        public string _SortExpression
        {
            get
            {
                if (this.ViewState["sortExpression"] == null)
                    this.ViewState["sortExpression"] = "";
                return (string)this.ViewState["sortExpression"];
            }
            set
            {
                this.ViewState["sortExpression"] = value;
            }
        }

        #endregion Properties

        #region Overrides

        private bool bolDataSourceChanging;

        protected override void OnDataPropertyChanged()
        {
            base.OnDataPropertyChanged();

            if (this.bolDataSourceChanging == false)
            {
                this.bolDataSourceChanging = true;

                if (this.DataSource != null && this.AllowSorting && this._SortExpression != "")
                {
                    System.Data.DataView dv = null;

                    if (this.DataSource is System.Data.DataView)
                    {
                        dv = (System.Data.DataView)this.DataSource;
                    }
                    else
                    {
                        if (this.DataSource is System.Data.DataTable)
                        {
                            dv = new System.Data.DataView((System.Data.DataTable)this.DataSource);
                        }
                    }

                    if (dv != null)
                    {
                        dv.Sort = this._SortExpression + " " + this._SortDirectionStr;
                        this.DataSource = dv;
                    }
                }

                this.bolDataSourceChanging = false;
            }
        }

        protected override void OnDataBound(EventArgs e)
        {
            base.OnDataBound(e);

            this.FreezeCells();
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.RegisterScriptFunctions();

            base.OnPreRender(e);

            if (this.FreezeHeader && !this.Page.Items.Contains(roGridViewControl.FrozenTopCssClass))
            {
                this.Page.Items[roGridViewControl.FrozenTopCssClass] = "1";
                var frozenTopStyle = new FrozenTopStyle();
                this.Page.Header.StyleSheet.CreateStyleRule(frozenTopStyle, null, "." + roGridViewControl.FrozenTopCssClass);
            }
            if (this.FreezeColumn >= 0 && !this.Page.Items.Contains(roGridViewControl.FrozenLeftCssClass))
            {
                this.Page.Items[roGridViewControl.FrozenLeftCssClass] = "1";
                var frozenLeftStyle = new FrozenLeftStyle();
                this.Page.Header.StyleSheet.CreateStyleRule(frozenLeftStyle, null, "." + roGridViewControl.FrozenLeftCssClass);
            }
            if (this.FreezeHeader && this.FreezeColumn >= 0 && !this.Page.Items.Contains(roGridViewControl.FrozenLeftHeaderCssClass))
            {
                this.Page.Items[roGridViewControl.FrozenLeftHeaderCssClass] = "1";
                var frozenLeftHeaderStyle = new FrozenLeftHeaderStyle();
                this.Page.Header.StyleSheet.CreateStyleRule(frozenLeftHeaderStyle, null, "." + roGridViewControl.FrozenLeftHeaderCssClass);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.Page != null)
            {
                this.Page.VerifyRenderingInServerForm(this);
            }
            this.PrepareControlHierarchy();

            if (!this.DesignMode)
            {
                string clientID = this.ClientID;
                if (clientID == null)
                {
                    throw new HttpException("FrozenGridView Must Be Parented");
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Id, String.Format(CultureInfo.InvariantCulture, "__gv{0}__div", clientID), true);
                writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowX, this.OverflowX);
                writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, this.OverflowY);
                if (!this.Width.IsEmpty)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString(CultureInfo.InvariantCulture));
                }
                if (!this.Height.IsEmpty)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.Height.ToString(CultureInfo.InvariantCulture));
                }
                writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, this.BorderColor.ToString());
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, this.BorderWidth.ToString(CultureInfo.InvariantCulture));
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, this.BorderStyle.ToString());
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (this.hdnRow != null) this.hdnRow.RenderControl(writer);
                if (this.hdnCol != null) this.hdnCol.RenderControl(writer);

                //this.AddEvents();
            }

            this.RenderContents(writer);

            if (!this.DesignMode)
            {
                writer.RenderEndTag();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            //this.RegisterScriptFunctions();
            base.OnInit(e);
        }

        protected override void OnRowDataBound(GridViewRowEventArgs e)
        {
            base.OnRowDataBound(e);

            if (/*e.Row.RowType == DataControlRowType.Header || */e.Row.RowType == DataControlRowType.DataRow)
            {
                if (this.CellHoverCssClass != "" || this.CellSelectCssClass != "")
                {
                    string strScript;

                    string strRowSelectedID = this.RowSelectedControlID;
                    if (strRowSelectedID == "") strRowSelectedID = this.hdnRow.ClientID;
                    string strColSelectedID = this.ColSelectedControlID;
                    if (strColSelectedID == "") strColSelectedID = this.hdnCol.ClientID;

                    for (int cell = 0; cell < this.Columns.Count; cell++)
                    {
                        if (this.CellHoverCssClass != "")
                        {
                            strScript = "javascript: roGridViewControl_MouseOver(this, '" + this.CellHoverCssClass + "');";
                            this.AddAttribute(e.Row.Cells[cell].Attributes, "onmouseover", strScript);
                            /*if (this.Rows[row].Cells[cell].Attributes["onmouseover"] != null)
                                this.Rows[row].Cells[cell].Attributes["onmouseover"] += ";" + strScript;
                            else
                                this.Rows[row].Cells[cell].Attributes.Add("onmouseover", strScript);*/
                            strScript = "javascript: roGridViewControl_MouseOut(this, '" + this.CellHoverCssClass + "');";
                            this.AddAttribute(e.Row.Cells[cell].Attributes, "onmouseout", strScript);
                        }
                        if (this.CellSelectCssClass != "")
                        {
                            strScript = "javascript: roGridViewControl_MouseClick(this, '" + strRowSelectedID + "', '" + strColSelectedID + "', '" + this.CellSelectCssClass + "');";
                            this.AddAttribute(e.Row.Cells[cell].Attributes, "onclick", strScript);
                            this.AddAttribute(e.Row.Cells[cell].Attributes, "oncontextmenu", strScript);
                        }
                    }
                }
            }

            /* Add sort image control (if needed) */
            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (this._SortExpression != "")
                {
                    int cell = -1;
                    foreach (DataControlField oColumn in this.Columns)
                    {
                        if (oColumn.SortExpression == this._SortExpression)
                        {
                            cell = this.Columns.IndexOf(oColumn);
                            break;
                        }
                    }

                    if (cell > -1)
                    {
                        var oImage = new System.Web.UI.WebControls.Image();
                        oImage.ImageUrl = this._SortDirection == SortDirection.Ascending ? "~/Base/Images/Grid/SortASC.gif" : "~/Base/Images/Grid/SortDESC.gif";
                        e.Row.Cells[cell].Controls.AddAt(0, oImage);
                    }
                }
            }
        }

        protected override void OnSorting(GridViewSortEventArgs e)
        {
            string sortExpression = e.SortExpression;

            this._SortExpression = sortExpression;

            if (this._SortDirection == SortDirection.Ascending)
            {
                this._SortDirection = SortDirection.Descending;
            }
            else
            {
                this._SortDirection = SortDirection.Ascending;
            }

            base.OnSorting(e);
        }

        #endregion Overrides

        private void FreezeCells()
        {
            if (this.FreezeHeader)
            {
                /*foreach (DataControlFieldHeaderCell th in this.HeaderRow.Cells)
                {
                    th.CssClass = roGridViewControl.FrozenTopCssClass + " " + th.CssClass;
                }*/
                for (int n = 0; n < this.HeaderRow.Cells.Count; n++)
                {
                    if (n != this.FreezeColumn)
                        this.HeaderRow.Cells[n].CssClass = roGridViewControl.FrozenTopCssClass + " " + this.HeaderRow.Cells[n].CssClass;
                }
            }
            if (this.FreezeColumn >= 0)
            {
                this.HeaderRow.Cells[this.FreezeColumn].CssClass = roGridViewControl.FrozenLeftHeaderCssClass + " " + this.HeaderRow.Cells[this.FreezeColumn].CssClass;
                foreach (GridViewRow tr in this.Rows)
                {
                    //tr.Cells[this.FreezeColumn].CssClass = roGridViewControl.FrozenLeftCssClass + " " + tr.CssClass;
                    tr.Cells[this.FreezeColumn].CssClass = roGridViewControl.FrozenLeftCssClass + " " + this.Columns[this.FreezeColumn].ItemStyle.CssClass;
                }
            }
        }

        private class FrozenTopStyle : Style
        {
            internal FrozenTopStyle()
            {
            }

            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                base.FillStyleAttributes(attributes, urlResolver);

                attributes[HtmlTextWriterStyle.Top] = "expression(this.offsetParent.scrollTop)";
                attributes[HtmlTextWriterStyle.Position] = "relative";
                attributes[HtmlTextWriterStyle.ZIndex] = "3";
            }
        }

        private class FrozenLeftStyle : Style
        {
            internal FrozenLeftStyle()
            {
            }

            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                base.FillStyleAttributes(attributes, urlResolver);

                attributes[HtmlTextWriterStyle.Left] = "expression(this.offsetParent.scrollLeft)";
                attributes[HtmlTextWriterStyle.Position] = "relative";
                attributes[HtmlTextWriterStyle.ZIndex] = "2";
            }
        }

        private class FrozenLeftHeaderStyle : Style
        {
            internal FrozenLeftHeaderStyle()
            {
            }

            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                base.FillStyleAttributes(attributes, urlResolver);

                attributes[HtmlTextWriterStyle.Top] = "expression(this.offsetParent.scrollTop)";
                attributes[HtmlTextWriterStyle.Left] = "expression(this.offsetParent.scrollLeft)";
                attributes[HtmlTextWriterStyle.Position] = "relative";
                attributes[HtmlTextWriterStyle.ZIndex] = "4";
            }
        }

        private void RegisterScriptFunctions()
        {
            if (this.CellHoverCssClass != "" || this.CellSelectCssClass != "")
            {
                if (!this.Page.ClientScript.IsClientScriptBlockRegistered("roGridViewControlScript"))
                {
                    string strScript = "<script language=JavaScript> ";
                    if (this.CellHoverCssClass != "")
                    {
                        strScript = "function roGridViewControl_MouseOver(cell, css) { " +
                                        "Sys.UI.DomElement.addCssClass(cell, css); " +
                                     "} " +
                                     "function roGridViewControl_MouseOut(cell, css) { " +
                                        "Sys.UI.DomElement.removeCssClass(cell, css); " +
                                     "} ";
                        Page.ClientScript.RegisterClientScriptBlock(typeof(string), "MouseEventFunctions" + this.ClientID, strScript, true);
                    }
                    if (this.CellSelectCssClass != "")
                    {
                        strScript = "function roGridViewControl_MouseClick(cell, RowSelectedControlID, ColSelectedControlID, css) { " +
                                        "var hdnRow = document.getElementById(RowSelectedControlID); " +
                                        "var hdnCol = document.getElementById(ColSelectedControlID); " +
                                        "var row = hdnRow.value; var col = hdnCol.value; " +
                                        "var rows = cell.parentNode.parentNode.getElementsByTagName('tr'); " +
                                        "if (row > -1 && row < rows.length) { " +
                                            "if (col > -1 && col < rows[row].cells.length) {" +
                                                //"alert(row + ';' + col + ';' + css); " +
                                                "Sys.UI.DomElement.removeCssClass(rows[row].cells[col], css); " +
                                            "} " +
                                        "} " +
                                        "Sys.UI.DomElement.addCssClass(cell, css); " +
                                        "hdnRow.value = cell.parentNode.rowIndex; " +
                                        "hdnCol.value = cell.cellIndex;" +
                                     "} ";
                        strScript += "function roGridViewControl_Load(table, row, col, css) { " +
                                        "var rows = table.getElementsByTagName('tr'); " +
                                        "if (row > -1 && row < rows.length) { " +
                                            "if (col > -1 && col < rows[row].cells.length) {" +
                                                "Sys.UI.DomElement.addCssClass(rows[row].cells[col], css); " +
                                            "} " +
                                        "} " +
                                     "} ";
                        strScript += "function roGridViewControl_Select(table, row, col, css) { " +
                                        "var rows = table.getElementsByTagName('tr'); " +
                                        "if (row > -1 && row < rows.length) { " +
                                            "if (col > -1 && col < rows[row].cells.length) {" +
                                                "Sys.UI.DomElement.addCssClass(rows[row].cells[col], css); " +
                                            "} " +
                                        "} " +
                                     "} ";
                        strScript += "function roGridViewControl_Unselect(table, row, col, css) { " +
                                        "var rows = table.getElementsByTagName('tr'); " +
                                        "if (row > -1 && row < rows.length) { " +
                                            "if (col > -1 && col < rows[row].cells.length) {" +
                                                "Sys.UI.DomElement.removeCssClass(rows[row].cells[col], css); " +
                                            "} " +
                                        "} " +
                                     "} ";
                        Page.ClientScript.RegisterClientScriptBlock(typeof(string), "SelectFunctions" + this.ClientID, strScript, true);
                    }
                    //strScript += "</script>";

                    //this.Page.ClientScript.RegisterClientScriptBlock(typeof(string) , "roGridViewControlScript", strScript);
                }
            }
        }

        private void AddEvents()
        {
            if (this.CellHoverCssClass != "" || this.CellSelectCssClass != "")
            {
                string strScript;

                string strRowSelectedID = this.RowSelectedControlID;
                if (strRowSelectedID == "") strRowSelectedID = this.hdnRow.ClientID;
                string strColSelectedID = this.ColSelectedControlID;
                if (strColSelectedID == "") strColSelectedID = this.hdnCol.ClientID;

                for (int row = 0; row < this.Rows.Count; row++)
                {
                    for (int cell = 0; cell < this.Columns.Count; cell++)
                    {
                        if (this.CellHoverCssClass != "")
                        {
                            strScript = "javascript: roGridViewControl_MouseOver(this, '" + this.CellHoverCssClass + "');";
                            this.AddAttribute(this.Rows[row].Cells[cell].Attributes, "onmouseover", strScript);
                            /*if (this.Rows[row].Cells[cell].Attributes["onmouseover"] != null)
                                this.Rows[row].Cells[cell].Attributes["onmouseover"] += ";" + strScript;
                            else
                                this.Rows[row].Cells[cell].Attributes.Add("onmouseover", strScript);*/
                            strScript = "javascript: roGridViewControl_MouseOut(this, '" + this.CellHoverCssClass + "');";
                            this.AddAttribute(this.Rows[row].Cells[cell].Attributes, "onmouseout", strScript);
                        }
                        if (this.CellSelectCssClass != "")
                        {
                            strScript = "javascript: roGridViewControl_MouseClick(this, '" + strRowSelectedID + "', '" + strColSelectedID + "', '" + this.CellSelectCssClass + "');";
                            this.AddAttribute(this.Rows[row].Cells[cell].Attributes, "onclick", strScript);
                            this.AddAttribute(this.Rows[row].Cells[cell].Attributes, "oncontextmenu", strScript);
                        }
                    }
                }
                /*
                if (this.CellSelectCssClass != "")
                {
                    int row = -1;
                    HiddenField oHdnRow = (HiddenField) this.GetControl(this.Page.Controls, strRowSelectedID);
                    if (oHdnRow != null) row = Convert.ToInt32(oHdnRow.Value);
                    int col = -1;
                    HiddenField oHdnCol = (HiddenField) this.GetControl(this.Page.Controls, strColSelectedID);
                    if (oHdnCol != null) col = Convert.ToInt32(oHdnCol.Value);

                    if (row > -1 && col > -1 && row < this.Rows.Count && col < this.Columns.Count)
                    {
                        this.Rows[row].Cells[col].CssClass = this.CellSelectCssClass;
                    }
                }
                */
            }
        }

        //private Control GetControl(ControlCollection Controls, string strClientID )
        //{
        //    // Obtiene el objeto 'Control' con el identificador 'strControlID', de forma recursiva.
        //    Control oRet = null;

        //    foreach (Control oControl in Controls){
        //        if (oControl.ClientID != null && oControl.ClientID.ToLower() == strClientID.ToLower())
        //        {
        //            oRet = oControl;
        //            break;
        //        }
        //        else {
        //            if (!(oControl.GetType is GridView)) {
        //                oRet = GetControl(oControl.Controls, strClientID);
        //                if (oRet != null) break;
        //            }
        //        }
        //    }
        //    return oRet;

        //}

        private void AddAttribute(System.Web.UI.AttributeCollection _Attributes, string strAttribute, string strScipt)
        {
            if (_Attributes[strAttribute] != null)
            {
                if (_Attributes[strAttribute].Trim().ToLower().StartsWith("javascript:", StringComparison.CurrentCulture))
                    _Attributes[strAttribute] = _Attributes[strAttribute].Trim().Substring(11);
                if (!_Attributes[strAttribute].Trim().EndsWith(";", StringComparison.CurrentCulture))
                    _Attributes[strAttribute] += ";";
                _Attributes[strAttribute] = strScipt + " " + _Attributes[strAttribute];
            }
            else
                _Attributes.Add(strAttribute, strScipt);
        }

        /*
        private void SortGridView(string sortExpression, string direction) {
            System.Data.DataView dv = null;

                dv = (System.Data.DataView) this.DataSource;

            if (dv != null) {
                dv.Sort = sortExpression + direction;

                this.DataSource = dv;
                this.DataBind();
            }
        }
        */
    }
}
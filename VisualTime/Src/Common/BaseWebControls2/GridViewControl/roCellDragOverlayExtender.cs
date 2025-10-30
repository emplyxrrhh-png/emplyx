using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

#region Assembly Resource Attribute

[assembly: System.Web.UI.WebResource("Robotics.WebControls2.GridViewControl.CellDragOverlayBehavior.js", "text/javascript")]

#endregion Assembly Resource Attribute

namespace Robotics.WebControls2.GridViewControl
{
    [TargetControlType(typeof(Control))]
    public class roCellDragOverlayExtender : ExtenderControl, IPostBackEventHandler
    {
        internal const string Invalid_Container = "The roCellDragOverlayExtender can only be placed inside a GridViewRow";

        public delegate void CellDropEventHandler(object sender, roCellDropEventArgs e);

        public event CellDropEventHandler CellDrop;

        private GridViewRow _parentGridViewRow;
        private int _colIndex;

        public string GridViewUniqueID
        {
            get
            {
                return ParentGridViewRow.NamingContainer.UniqueID;
            }
        }

        public int RowIndex
        {
            get
            {
                return ParentGridViewRow.RowIndex;
            }
        }

        public int ColIndex
        {
            get
            {
                return _colIndex;
            }
            set
            {
                _colIndex = value;
            }
        }

        private GridViewRow ParentGridViewRow
        {
            get
            {
                if (_parentGridViewRow == null)
                {
                    _parentGridViewRow = this.NamingContainer as GridViewRow;
                    if (_parentGridViewRow == null)
                    {
                        throw new InvalidOperationException(Invalid_Container);
                    }
                }
                return _parentGridViewRow;
            }
        }

        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl)
        {
            var descriptor =
                new ScriptBehaviorDescriptor("Robotics.CellDragOverlayBehavior", targetControl.ClientID);

            descriptor.AddProperty("rowIndex", this.RowIndex);
            descriptor.AddProperty("colIndex", this.ColIndex);
            descriptor.AddProperty("UniqueID", this.UniqueID);
            descriptor.AddProperty("gridViewUniqueID", this.GridViewUniqueID.Replace("$", "_"));
            return new ScriptDescriptor[] { descriptor };
        }

        protected override IEnumerable<ScriptReference> GetScriptReferences()
        {
            var reference = new ScriptReference(
               "PreviewScript.js", "Microsoft.Web.Preview");
            var reference2 = new ScriptReference(
               "PreviewDragDrop.js", "Microsoft.Web.Preview");
            var reference3 = new ScriptReference(
                "Robotics.WebControls2.GridViewControl.CellDragOverlayBehavior.js", "Robotics.WebControls2");

            return new ScriptReference[] { reference, reference2, reference3 };
        }

        private void OnCellDrop(string eventArgument)
        {
            if (this.CellDrop != null)
            {
                CellDrop(this, new roCellDropEventArgs(eventArgument));
            }
        }

        #region IPostBackEventHandler Members

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            OnCellDrop(eventArgument);
        }

        #endregion IPostBackEventHandler Members
    }

    public class roCellDropEventArgs : EventArgs
    {
        public roCellDropEventArgs(string eventArgument)
        {
            string[] data = eventArgument.Split(new char[] { ':' });
            this._sourceGridViewID = data[0];
            this._sourceRowIndex = Convert.ToInt32(data[1]);
            this._sourceColIndex = Convert.ToInt32(data[2]);
        }

        private int _sourceRowIndex;
        private int _sourceColIndex;
        private string _sourceGridViewID;

        public string SourceGridViewID
        {
            get { return _sourceGridViewID; }
            set { _sourceGridViewID = value; }
        }

        public int SourceRowIndex
        {
            get { return _sourceRowIndex; }
            set { _sourceRowIndex = value; }
        }

        public int SourceColIndex
        {
            get { return _sourceColIndex; }
            set { _sourceColIndex = value; }
        }
    }
}
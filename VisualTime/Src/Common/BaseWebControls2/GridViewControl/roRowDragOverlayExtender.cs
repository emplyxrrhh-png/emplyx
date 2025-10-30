using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

#region Assembly Resource Attribute

[assembly: System.Web.UI.WebResource("Robotics.WebControls2.GridViewControl.RowDragOverlayBehavior.js", "text/javascript")]

#endregion Assembly Resource Attribute

namespace Robotics.WebControls2.GridViewControl
{
    [TargetControlType(typeof(Control))]
    public class roRowDragOverlayExtender : ExtenderControl, IPostBackEventHandler
    {
        internal const string Invalid_Container = "The roRowDragOverlayExtender can only be placed inside a GridViewRow";

        public delegate void RowDropEventHandler(object sender, roRowDropEventArgs e);

        public event RowDropEventHandler RowDrop;

        private GridViewRow _parentGridViewRow;

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
                new ScriptBehaviorDescriptor("Robotics.RowDragOverlayBehavior", targetControl.ClientID);

            descriptor.AddProperty("rowIndex", this.RowIndex);
            descriptor.AddProperty("UniqueID", this.UniqueID);
            descriptor.AddProperty("gridViewUniqueID", this.GridViewUniqueID);
            return new ScriptDescriptor[] { descriptor };
        }

        protected override IEnumerable<ScriptReference> GetScriptReferences()
        {
            var reference = new ScriptReference(
               "PreviewScript.js", "Microsoft.Web.Preview");
            var reference2 = new ScriptReference(
               "PreviewDragDrop.js", "Microsoft.Web.Preview");
            var reference3 = new ScriptReference(
                "Robotics.WebControls2.GridViewControl.RowDragOverlayBehavior.js", "Robotics.WebControls2");

            return new ScriptReference[] { reference, reference2, reference3 };
        }

        private void OnRowDrop(string eventArgument)
        {
            if (this.RowDrop != null)
            {
                RowDrop(this, new roRowDropEventArgs(eventArgument));
            }
        }

        #region IPostBackEventHandler Members

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            OnRowDrop(eventArgument);
        }

        #endregion IPostBackEventHandler Members
    }

    public class roRowDropEventArgs : EventArgs
    {
        public roRowDropEventArgs(string eventArgument)
        {
            string[] data = eventArgument.Split(new char[] { ':' });
            this._sourceGridViewID = data[0];
            this._sourceRowIndex = Convert.ToInt32(data[1]);
        }

        private int _sourceRowIndex;
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
    }
}
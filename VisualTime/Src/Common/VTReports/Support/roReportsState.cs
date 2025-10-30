using Robotics.Base.DTOs;
using Robotics.Security.Base;
using System;
using System.Web;

namespace VTReports.Domain
{
    public class roReportsState : roBusinessState
    {
        private ReportResultEnum _result;

        public roReportsState(int idpassport = -1, HttpContext context = null, string clientAddress = "") : base("VTBussines.Report", "ReportService", idpassport, context, clientAddress)
        {
            _result = ReportResultEnum.NoError;
        }

        public ReportResultEnum Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                if (_result != ReportResultEnum.NoError && _result != ReportResultEnum.Exception)
                {
                    ErrorText = Language.Translate("ResultEnum." + _result.ToString(), "");
                }
            }
        }

        public string GetAuditObjectName(int? id, string name, bool? operationSucceeded)
        {
            return $"{(id != null ? "ID: " + id + " " : String.Empty)}" +
                   $"{(name != null && !name.Equals(String.Empty) ? "NAME: " + name + " " : String.Empty)}" +
                   $"{(operationSucceeded != null && operationSucceeded.Value ? "RESULT: " + operationSucceeded.Value.ToString() : String.Empty)}";
        }

        public string GetStateLocationInfo(object sender)
        {
            return $"{sender.GetType().FullName}::{new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name}";
        }
    }
}
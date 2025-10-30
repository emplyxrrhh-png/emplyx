using DevExpress.XtraReports.UI;
using Robotics.Security;
using Robotics.Security.Base;
using Robotics.VTBase;
using System;

namespace ReportGenerator.Services
{
    public class ReportTranslationService : IReportTranslationService
    {
        public static string language = null;

        #region Constructor

        public ReportTranslationService()
        {
        }

        #endregion Constructor

        public void setTranslateLanguage(string language, int passportId)
        {
            ReportTranslationService.language = language;
            roLanguage languageHelper = new roLanguage();
            roSecurityState securityState = new roSecurityState(passportId);
            languageHelper.SetLanguageReference("LiveDXReport", (ReportTranslationService.language != null && !ReportTranslationService.language.Equals("---") ? ReportTranslationService.language : roPassportManager.GetPassportTicket(passportId,Robotics.Base.DTOs.LoadType.Passport, ref securityState).Language.Key));
        }

        public void unsetTranslateLanguage()
        {
            ReportTranslationService.language = null;
        }

        public void TranslateXtraReport(XtraReport report, string reportName, int passportId)
        {
            try
            {
                roLanguage languageHelper = new roLanguage();
                roSecurityState securityState = new roSecurityState(passportId);
                try
                {
                    languageHelper.SetLanguageReference("LiveDXReport", (ReportTranslationService.language != null && !ReportTranslationService.language.Equals("---") ? ReportTranslationService.language : roPassportManager.GetPassportTicket(passportId, Robotics.Base.DTOs.LoadType.Passport, ref securityState).Language.Key));
                }
                catch (Exception ex)
                {
                    roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportGeneratorService::TranslateXtraReport:: Error on SetLanguageReference: ", ex);
                    throw new Exception("ReportGeneratorService::TranslateXtraReport:: Error on SetLanguageReference: ", ex);
                }

                foreach (DevExpress.XtraReports.UI.XRControl control in report.AllControls<XRControl>())
                {
                    if (control.Name.StartsWith("_translate_"))
                    {
                        string stext = languageHelper.Translate(control.Name.Replace("_translate_", ""), reportName.Replace(" ", ""), false, control.Text);
                        if (stext != "NotFound") control.Text = stext;
                    }
                    else
                    {
                        try
                        {
                            if (control.GetType().Name.Equals("XRLabel"))
                            {
                                if (languageHelper.GetLanguageCulture().Name.Equals("en-US"))
                                {
                                    if (((XRLabel)control).TextFormatString.Trim().Equals("{0:dd/MM/yyyy}"))
                                        ((XRLabel)control).TextFormatString = "{0:MM/dd/yyyy}";
                                    else
                                    {
                                        if (((XRLabel)control).TextFormatString.Trim().Equals("{0:dd/MM/yyyy HH:mm}"))
                                        {
                                            ((XRLabel)control).TextFormatString = "{0:MM/dd/yyyy HH:mm}";
                                        }
                                        else
                                        {
                                            if (((XRLabel)control).TextFormatString.Trim().Equals("{0:dd/MM/yyyy HH:mm:ss}"))
                                            {
                                                ((XRLabel)control).TextFormatString = "{0:MM/dd/yyyy HH:mm:ss}";
                                            }
                                        }
                                    }

                                }
                            }
                            else
                            {
                                if (control.GetType().Name.Equals("XRPageInfo"))
                                {
                                    if (languageHelper.GetLanguageCulture().Name.Equals("en-US"))
                                    {
                                        if (((XRPageInfo)control).TextFormatString.Trim().Equals("{0:dd/MM/yyyy}"))
                                            ((XRPageInfo)control).TextFormatString = "{0:MM/dd/yyyy}";
                                        else
                                        {
                                            if (((XRPageInfo)control).TextFormatString.Trim().Equals("{0:dd/MM/yyyy HH:mm}"))
                                            {
                                                ((XRPageInfo)control).TextFormatString = "{0:MM/dd/yyyy HH:mm}";
                                            }
                                            else
                                            {
                                                if (((XRPageInfo)control).TextFormatString.Trim().Equals("{0:dd/MM/yyyy HH:mm:ss}"))
                                                {
                                                    ((XRPageInfo)control).TextFormatString = "{0:MM/dd/yyyy HH:mm:ss}";
                                                }
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    if (control.GetType().Name.Equals("XRTableCell"))
                                    {
                                        if (languageHelper.GetLanguageCulture().Name.Equals("en-US"))
                                        {
                                            if (((XRTableCell)control).TextFormatString.Trim().Equals("{0:dd/MM/yyyy}"))
                                                ((XRTableCell)control).TextFormatString = "{0:MM/dd/yyyy}";
                                            else
                                            {
                                                if (((XRTableCell)control).TextFormatString.Trim().Equals("{0:dd/MM/yyyy HH:mm}"))
                                                {
                                                    ((XRTableCell)control).TextFormatString = "{0:MM/dd/yyyy HH:mm}";
                                                }
                                                else
                                                {
                                                    if (((XRTableCell)control).TextFormatString.Trim().Equals("{0:dd/MM/yyyy HH:mm:ss}"))
                                                    {
                                                        ((XRTableCell)control).TextFormatString = "{0:MM/dd/yyyy HH:mm:ss}";
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportGeneratorService::TranslateXtraReport:: Error formating labels: ", ex);
                            throw new Exception("ReportGeneratorService::TranslateXtraReport:: Error formating labels: ", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportGeneratorService::TranslateXtraReport:: Error: ", ex);
                throw new Exception("ReportGeneratorService::TranslateXtraReport:: Error: ", ex);
            }
            
        }
    }
}
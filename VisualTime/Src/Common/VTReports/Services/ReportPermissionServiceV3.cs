using ReportGenerator.Repositories;
using Robotics.Base;
using Robotics.Base.DTOs;
using Robotics.Portal.Business;
using Robotics.Security;
using Robotics.VTBase;
using System;
using System.Collections.Generic;

namespace ReportGenerator.Services
{
    public class ReportPermissionServiceV3 : IReportPermissionService
    {
        private List<(int, int)> _userCategoriesList = new List<(int, int)>();

        private IPassportService _passportService;
        private IReportCategoryService _reportCategoryService;

        #region Constructor

        public ReportPermissionServiceV3()
        {
            this._passportService = new PassportService();
            this._reportCategoryService = new ReportCategoryService();
        }

        #endregion Constructor
        public bool GetGeneralReportPermission(int passportId, ReportPermissionTypes action)
        {
            bool isDesignerUser = (Robotics.Security.WLHelper.GetPermissionOverFeature(passportId, "Reports", "U") > Robotics.Base.DTOs.Permission.Read);

            if (this._passportService.IsPassportIdARoboticsUser(passportId))
            {
                return true;
            }

            if (isDesignerUser)
            {
                return true;
            }

            switch (action)
            {
                case ReportPermissionTypes.Read:
                case ReportPermissionTypes.Execute:
                case ReportPermissionTypes.Schedule:
                case ReportPermissionTypes.DeleteExecutions:
                case ReportPermissionTypes.CreateSchedules:
                case ReportPermissionTypes.UpdateSchedules:
                case ReportPermissionTypes.DeleteSchedules:
                    return true;

                case ReportPermissionTypes.Create:
                case ReportPermissionTypes.Update:
                case ReportPermissionTypes.Delete:
                case ReportPermissionTypes.CopyReport:
                    return false;
            }

            return false;
        }

        public bool GetPermissionOverReportAction(int passportId, int reportId, ReportPermissionTypes action)
        {
            Report report = new ReportRepository().Get(reportId);

            if(report != null)
            {
                return GetPermissionOverReportAction(passportId, report, action);
            }
            return false;
        }

        public bool GetPermissionOverReportAction(int passportId, Report report, ReportPermissionTypes action)
        {
            if (report == null) return false;
            bool IsDesignerUser = (Robotics.Security.WLHelper.GetPermissionOverFeature(passportId, "Reports", "U") > Robotics.Base.DTOs.Permission.Read);
            bool permissionGranted = false;
            this._userCategoriesList = this._reportCategoryService.GetUserCategoriesList(passportId);

            // Si nos pasan un report, miramos los permisos sobre ese report
            // Si somos Consultor o el creador del report, tenemos todos los permisos
            if (this._passportService.IsPassportIdARoboticsUser(passportId) || report.CreatorPassportId == passportId)
            {
                permissionGranted = true;
            }
            else
            {
                // Si es de sistema, si tenemos permisos de licencia y funcionalidad, tendremos permisos de todo menos copiar el report, el cual
                // lo tendremos si tenemos ejecucion sobre reports
                if (IsSystemReport(report.CreatorPassportId))
                {
                    if (DoesUserHaveCategoryPermission(report.Id) && IsReportAuthorized(passportId, report))
                    {
                        switch (action)
                        {
                            case ReportPermissionTypes.Read:
                            case ReportPermissionTypes.Execute:
                            case ReportPermissionTypes.Schedule:
                            case ReportPermissionTypes.DeleteExecutions:
                            case ReportPermissionTypes.CreateSchedules:
                            case ReportPermissionTypes.UpdateSchedules:
                            case ReportPermissionTypes.DeleteSchedules:
                                permissionGranted = true;
                                break;

                            case ReportPermissionTypes.CopyReport:
                                permissionGranted = IsDesignerUser;
                                break;
                        }
                    }
                }
                else
                {
                    //Si el report es creado por un usuario, si tenemos permisos de licencia y funcionalidad, si tenemos ejecucion tendremos permisos sobre ese report
                    //Si no, no podremos crear, actualizar, borrar o copiar el report
                    if (DoesUserHaveCategoryPermission(report.Id) && IsReportAuthorized(passportId, report))
                    {
                        permissionGranted = GetGeneralReportPermission(passportId, action);
                    }
                }
            }

            return permissionGranted;
        }

        public ReportPermissions GetPermissionsOverReport(Report report, int passportId)
        {
            ReportPermissions auxiliarReportPermissions = new ReportPermissions();
            bool IsDesignerUser = (Robotics.Security.WLHelper.GetPermissionOverFeature(passportId, "Reports", "U") > Robotics.Base.DTOs.Permission.Read);

            if (this._passportService.IsPassportIdARoboticsUser(passportId))
            {
                auxiliarReportPermissions.Edit = true;
                auxiliarReportPermissions.Execute = true;
                auxiliarReportPermissions.Remove = true;
            }
            else if (IsSystemReport(report.CreatorPassportId))
            {
                auxiliarReportPermissions.Edit = false;
                auxiliarReportPermissions.Execute = true;
                auxiliarReportPermissions.Remove = false;
            }
            else
            {
                auxiliarReportPermissions.Edit = IsDesignerUser;
                auxiliarReportPermissions.Execute = true;
                auxiliarReportPermissions.Remove = IsDesignerUser;
            }

            return auxiliarReportPermissions;
        }

        private bool IsReportAuthorized(int passportId, Report report)
        {
            bool isFeatureAuthorized = true;
            bool isFunctionalityAuthorized = true;

            if (!string.IsNullOrEmpty(report.RequieredFeature))
            {
                isFeatureAuthorized = LicenseCheck.IsAuthorized(report.RequieredFeature, passportId);
            }

            if (!string.IsNullOrEmpty(report.RequiredFunctionalities))
            {
                isFunctionalityAuthorized = FeaturesCheck.IsAuthorized(report.RequiredFunctionalities, "U", passportId);
            }

            return isFeatureAuthorized && isFunctionalityAuthorized;
        }

        #region Private Methods (Helpers)

        private bool DoesUserHaveCategoryPermission(int reportId)
        {
            bool result = true;

            try
            {
                List<(int, int)> reportCategoriesList = this._reportCategoryService.GetCategoriesIdFromReport(reportId);

                if (reportCategoriesList.Count == 0)
                {
                    //FOR THE MOMENT, IF THE REPORT HAS NO CATEGORIES, THE PERMISSION IS GRANTED
                }
                else
                {
                    //IF THE REPORT HAS CATEGORIES, WE CHECK FOR THE USER CATEGORIES TO MATCH
                    foreach ((int, int) categoryId in reportCategoriesList)
                    {
                        foreach ((int, int) userCatId in this._userCategoriesList)
                        {
                            if (userCatId.Item1 == categoryId.Item1 && userCatId.Item2 <= categoryId.Item2)
                            {
                                result = true;
                                break;
                            }
                        }
                        if (result) break;
                    }
                }
            }
            catch (System.Exception)
            {
                result = false;
            }

            return result;
        }

        private bool IsPassportIdTheReportCreator(int passportId, int? creatorPassportId)
        {
            return creatorPassportId == null ? false : passportId == creatorPassportId;
        }

        private bool IsSystemReport(int? reportCreatorPassportId)
        {
            return reportCreatorPassportId == null;
        }

        #endregion Private Methods (Helpers)
    }
}
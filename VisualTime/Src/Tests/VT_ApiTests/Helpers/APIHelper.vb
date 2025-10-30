Imports Robotics.Base
Imports Robotics.ExternalSystems
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess

Public Class APIHelper



    Function XStandardAbsenceToAbsenceConverterStub()
        VTLiveApi.Fakes.ShimAbsencesService_v2.AllInstances.XStandardAbsenceToAbsenceConverterroDatalinkStandarAbsence =
            Function(oAbsencesService_v2 As VTLiveApi.AbsencesService_v2, ByVal absence As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarAbsence)
                Return New roAbsence
            End Function
    End Function

    Function XAbsenceToStandardAbsenceConverterStub()
        VTLiveApi.Fakes.ShimAbsencesService_v2.AllInstances.XAbsenceToStandardAbsenceConverterroAbsence =
            Function(oAbsencesService_v2 As VTLiveApi.AbsencesService_v2, ByVal absence As roAbsence)
                Return New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarAbsence
            End Function
    End Function



    Function XDocumentToStandardDocumentConverterStub()
        VTLiveApi.Fakes.ShimDocumentsService_v2.AllInstances.XDocumentToStandardDocumentConverteroDocument =
            Function(oDocumentsService_v2 As VTLiveApi.DocumentsService_v2, ByVal document As roDocument)
                Return New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardDocument
            End Function
    End Function




    Function XCauseConverter()
        VTLiveApi.Fakes.ShimDailyCausesService_v2.AllInstances.XCauseConverterroDatalinkStandarDailyCause =
            Function(oDailyCausesService_v2 As VTLiveApi.DailyCausesService_v2, ByVal dailyCause As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarDailyCause)
                Return New roDailyCause
            End Function
    End Function

End Class
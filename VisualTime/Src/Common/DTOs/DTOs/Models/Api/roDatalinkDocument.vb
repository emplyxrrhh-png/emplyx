Imports System.Runtime.Serialization

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess

    <DataContract>
    Public Class roDatalinkStandardDocument

        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey

        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF

        <DataMember>
        Public Property DocumentType As String 'Nombre de la plantilla de documentos. Debe coincidir con una de las plantillas de documentos definidas en Visualtime

        <DataMember>
        Public Property DocumentData As String ' Documento en base64

        <DataMember>
        Public Property DocumentTitle As String ' Título del documento

        <DataMember>
        Public Property DocumentExtension As String ' Extensión del documento

        <DataMember>
        Public Property DocumentRemarks As String ' Notas del documento

        <DataMember>
        Public Property DocumentExternalId As String ' ID único que se le puede asignar a un documento

    End Class

    <DataContract>
    Public Class roDatalinkStandarDocumentResponse

        <DataMember>
        Public Property ResultCode As Integer ' Codigo resultado

        <DataMember>
        Public Property ResultDetails As String ' Explicación
        <DataMember>
        Public Property Documents As Generic.List(Of roDocument)

    End Class

    <DataContract>
    Public Class roDatalinkStandarDocumentCriteria
        Implements IDatalinkDocumentCriteria

        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA
        <DataMember>
        Public Property Type As String
        <DataMember>
        Public Property Title As String
        <DataMember>
        Public Property Company As String
        <DataMember>
        Public Property Timestamp As Date?
        <DataMember>
        Public Property UpdateType As String
        <DataMember>
        Public Property Extension As String
        <DataMember>
        Public Property Template As String

        Public Function GetEmployeeColumnsDefinitionCriteria(ByRef ColumnsVal() As String, ByRef ColumnsPos() As Integer) As Boolean Implements ExternalSystems.DataLink.RoboticsExternAccess.IDatalinkDocumentCriteria.GetEmployeeColumnsDefinitionCriteria
            Dim bolRet As Boolean = True

            Try
                ReDim ColumnsPos(System.Enum.GetValues(GetType(DocumentsCriteriaAsciiColumns)).Length - 1)
                ReDim ColumnsVal(System.Enum.GetValues(GetType(DocumentsCriteriaAsciiColumns)).Length - 1)

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For n As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsPos(n) = -1
                Next

                ColumnsPos(DocumentsCriteriaAsciiColumns.NIF) = CInt(DocumentsCriteriaAsciiColumns.NIF)
                ColumnsVal(CInt(DocumentsCriteriaAsciiColumns.NIF)) = NifEmpleado

                If NifLetter <> String.Empty Then
                    ColumnsVal(CInt(DocumentsCriteriaAsciiColumns.NIF)) = NifEmpleado & NifLetter
                End If

                ColumnsPos(DocumentsCriteriaAsciiColumns.ImportPrimaryKey) = CInt(DocumentsCriteriaAsciiColumns.ImportPrimaryKey)
                ColumnsVal(CInt(DocumentsCriteriaAsciiColumns.ImportPrimaryKey)) = UniqueEmployeeID

                ColumnsPos(DocumentsCriteriaAsciiColumns.NIF_Letter) = CInt(DocumentsCriteriaAsciiColumns.NIF_Letter)
                ColumnsVal(CInt(DocumentsCriteriaAsciiColumns.NIF_Letter)) = String.Empty

                ColumnsPos(DocumentsCriteriaAsciiColumns.Type) = CInt(DocumentsCriteriaAsciiColumns.Type)
                ColumnsVal(CInt(DocumentsCriteriaAsciiColumns.Type)) = Type

                ColumnsPos(DocumentsCriteriaAsciiColumns.Title) = CInt(DocumentsCriteriaAsciiColumns.Title)
                If Title IsNot Nothing Then
                    ColumnsVal(CInt(DocumentsCriteriaAsciiColumns.Title)) = Title.ToString()
                End If

                ColumnsPos(DocumentsCriteriaAsciiColumns.Company) = CInt(DocumentsCriteriaAsciiColumns.Company)
                If Company IsNot Nothing Then
                    ColumnsVal(CInt(DocumentsCriteriaAsciiColumns.Company)) = Company.ToString()
                End If

                ColumnsPos(DocumentsCriteriaAsciiColumns.Timestamp) = CInt(DocumentsCriteriaAsciiColumns.Timestamp)
                If Timestamp IsNot Nothing AndAlso Timestamp <> New Date(1, 1, 1) Then
                    ColumnsVal(CInt(DocumentsCriteriaAsciiColumns.Timestamp)) = Timestamp.ToString()
                End If


                ColumnsPos(DocumentsCriteriaAsciiColumns.UpdateType) = CInt(DocumentsCriteriaAsciiColumns.UpdateType)
                If UpdateType IsNot Nothing Then
                    ColumnsVal(CInt(DocumentsCriteriaAsciiColumns.UpdateType)) = UpdateType.ToString()
                End If

                ColumnsPos(DocumentsCriteriaAsciiColumns.Extension) = CInt(DocumentsCriteriaAsciiColumns.Extension)
                If Extension IsNot Nothing Then
                    ColumnsVal(CInt(DocumentsCriteriaAsciiColumns.Extension)) = Extension.ToString()
                End If

                ColumnsPos(DocumentsCriteriaAsciiColumns.Template) = CInt(DocumentsCriteriaAsciiColumns.Template)
                If Template IsNot Nothing Then
                    ColumnsVal(CInt(DocumentsCriteriaAsciiColumns.Template)) = Template.ToString()
                End If

                bolRet = True

                If ColumnsVal(DocumentsCriteriaAsciiColumns.Type) = "" OrElse (ColumnsVal(DocumentsCriteriaAsciiColumns.Type).ToLower() <> "employee" AndAlso ColumnsVal(DocumentsCriteriaAsciiColumns.Type).ToLower() <> "company") Then bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

    End Class

    Public Interface IDatalinkDocumentCriteria

        Function GetEmployeeColumnsDefinitionCriteria(ByRef ColumnsVal As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

    Public Enum DocumentsCriteriaAsciiColumns
        <EnumMember> NIF
        <EnumMember> NIF_Letter
        <EnumMember> ImportPrimaryKey
        <EnumMember> Title
        <EnumMember> Type
        <EnumMember> Company
        <EnumMember> Timestamp
        <EnumMember> UpdateType
        <EnumMember> Extension
        <EnumMember> Template
    End Enum

End Namespace
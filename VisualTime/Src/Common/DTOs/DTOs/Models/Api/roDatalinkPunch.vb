Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess

    Public Enum PunchesAsciiColumns
        <EnumMember> NIF
        <EnumMember> NIF_Letter
        <EnumMember> Type
        <EnumMember> DateTime
        <EnumMember> IDTerminal
        <EnumMember> TypeData
        <EnumMember> GPS
        <EnumMember> Field1
        <EnumMember> Field2
        <EnumMember> Field3
        <EnumMember> Field4
        <EnumMember> Field5
        <EnumMember> Field6
        <EnumMember> ImportPrimaryKey
        <EnumMember> ID
    End Enum

    Public Interface IDatalinkPunch

        Function GetEmployeeColumnsDefinition(ByRef ColumnsVal As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

    <DataContract>
    Public Class roDatalinkStandardPunch
        Implements IDatalinkPunch
        <DataMember>
        Public Property ID As String 'PunchID
        <DataMember>
        Public Property UniqueEmployeeID As String 'ImportKey
        <DataMember>
        Public Property NifEmpleado As String 'NIF / CIF
        <DataMember>
        Public Property NifLetter As String 'CIF LETRA
        <DataMember>
        Public Property Type As PunchTypeEnum 'Tipo de fichaje
        <DataMember>
        Public Property ActualType As PunchTypeEnum 'Tipo de fichaje
        <DataMember>
        Public Property ShiftDate As DateTime 'FECHA/Hora en la que se le asigna el fichaje
        <DataMember>
        Public Property DateTime As DateTime 'FECHA/Hora del momento del fichaje
        <DataMember>
        Public Property IDTerminal As Integer ' Identificador de terminal
        <DataMember>
        Public Property TypeData As String ' Nombre corto del motivo, en caso necesario
        <DataMember>
        Public Property GPS As String ' latitud, longitud del fichaje si tiene
        <DataMember>
        Public Property Field1 As String ' Informacion extendia, en caso necesario
        <DataMember>
        Public Property Field2 As String ' Informacion extendia, en caso necesario
        <DataMember>
        Public Property Field3 As String ' Informacion extendia, en caso necesario
        <DataMember>
        Public Property Field4 As Nullable(Of Double) ' Informacion extendia, en caso necesario
        <DataMember>
        Public Property Field5 As Nullable(Of Double) ' Informacion extendia, en caso necesario
        <DataMember>
        Public Property Field6 As Nullable(Of Double) ' Informacion extendia, en caso necesario
        <DataMember>
        Public Property ResultCode As Integer ' Código resultado de guardar el fichaje (se informa únicamente en la respuesta de la llamada)
        <DataMember>
        Public Property ResultDescription As String ' Informacion extendia, en caso necesario
        <DataMember>
        Public Property Timestamp As DateTime 'Fecha modificación del fichaje

        Public Function GetEmployeeColumnsDefinition(ByRef ColumnsVal() As String, ByRef ColumnsPos() As Integer) As Boolean Implements ExternalSystems.DataLink.RoboticsExternAccess.IDatalinkPunch.GetEmployeeColumnsDefinition
            Dim bolRet As Boolean = True

            Try
                ReDim ColumnsPos(System.Enum.GetValues(GetType(PunchesAsciiColumns)).Length - 1)
                ReDim ColumnsVal(System.Enum.GetValues(GetType(PunchesAsciiColumns)).Length - 1)

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For n As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsPos(n) = -1
                Next

                ColumnsPos(PunchesAsciiColumns.ID) = CInt(PunchesAsciiColumns.ID)
                ColumnsVal(CInt(PunchesAsciiColumns.ID)) = ID

                ColumnsPos(PunchesAsciiColumns.NIF) = CInt(PunchesAsciiColumns.NIF)
                ColumnsVal(CInt(PunchesAsciiColumns.NIF)) = NifEmpleado

                If NifLetter <> String.Empty Then
                    ColumnsVal(CInt(PunchesAsciiColumns.NIF)) = NifEmpleado & NifLetter
                End If

                ColumnsPos(PunchesAsciiColumns.ImportPrimaryKey) = CInt(PunchesAsciiColumns.ImportPrimaryKey)
                ColumnsVal(CInt(PunchesAsciiColumns.ImportPrimaryKey)) = UniqueEmployeeID

                ColumnsPos(PunchesAsciiColumns.NIF_Letter) = CInt(PunchesAsciiColumns.NIF_Letter)
                ColumnsVal(CInt(PunchesAsciiColumns.NIF_Letter)) = String.Empty

                ColumnsPos(PunchesAsciiColumns.Type) = CInt(PunchesAsciiColumns.Type)
                ColumnsVal(CInt(PunchesAsciiColumns.Type)) = Type

                ColumnsPos(PunchesAsciiColumns.DateTime) = CInt(PunchesAsciiColumns.DateTime)
                ColumnsVal(CInt(PunchesAsciiColumns.DateTime)) = DateTime.ToString("yyyy/MM/dd HH:mm:ss")

                ColumnsPos(PunchesAsciiColumns.IDTerminal) = CInt(PunchesAsciiColumns.IDTerminal)
                ColumnsVal(CInt(PunchesAsciiColumns.IDTerminal)) = IDTerminal

                ColumnsPos(PunchesAsciiColumns.TypeData) = CInt(PunchesAsciiColumns.TypeData)
                ColumnsVal(CInt(PunchesAsciiColumns.TypeData)) = TypeData

                ColumnsPos(PunchesAsciiColumns.GPS) = CInt(PunchesAsciiColumns.GPS)
                ColumnsVal(CInt(PunchesAsciiColumns.GPS)) = GPS

                ColumnsPos(PunchesAsciiColumns.Field1) = CInt(PunchesAsciiColumns.Field1)
                ColumnsVal(CInt(PunchesAsciiColumns.Field1)) = Field1

                ColumnsPos(PunchesAsciiColumns.Field2) = CInt(PunchesAsciiColumns.Field2)
                ColumnsVal(CInt(PunchesAsciiColumns.Field2)) = Field2

                ColumnsPos(PunchesAsciiColumns.Field3) = CInt(PunchesAsciiColumns.Field3)
                ColumnsVal(CInt(PunchesAsciiColumns.Field3)) = Field3

                ColumnsPos(PunchesAsciiColumns.Field4) = CInt(PunchesAsciiColumns.Field4)
                If Field4.HasValue Then
                    ColumnsVal(CInt(PunchesAsciiColumns.Field4)) = Field4.Value
                Else
                    ColumnsVal(CInt(PunchesAsciiColumns.Field4)) = String.Empty
                End If

                ColumnsPos(PunchesAsciiColumns.Field5) = CInt(PunchesAsciiColumns.Field5)
                If Field5.HasValue Then
                    ColumnsVal(CInt(PunchesAsciiColumns.Field5)) = Field5.Value
                Else
                    ColumnsVal(CInt(PunchesAsciiColumns.Field5)) = String.Empty
                End If

                ColumnsPos(PunchesAsciiColumns.Field6) = CInt(PunchesAsciiColumns.Field6)
                If Field6.HasValue Then
                    ColumnsVal(CInt(PunchesAsciiColumns.Field6)) = Field6.Value
                Else
                    ColumnsVal(CInt(PunchesAsciiColumns.Field6)) = String.Empty
                End If

                bolRet = True
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

    End Class

    <DataContract>
    Public Class roDatalinkStandardPunchResponse

        <DataMember>
        Public Property ResultCode As Integer ' Codigo resultado
        <DataMember>
        Public Property Punches As Generic.List(Of roDatalinkStandardPunch)

        <DataMember>
        Public Property PunchesListError As Generic.List(Of roDatalinkStandardPunch)

    End Class

End Namespace
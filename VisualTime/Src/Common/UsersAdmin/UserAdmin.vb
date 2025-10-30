Imports System.IO
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace Business

    <DataContract>
    Public Class wscUserAdmin

#Region "Declarations - Constructor"

        Private oPassport As roPassport
        Private oChilds As wscUserAdminList

        Private strLogin As String = ""
        Private strPassword As String = ""
        Private strNameOnDomain As String = ""

        Private oStateInfo As wscState

        Public Sub New()
            oPassport = Nothing
        End Sub

        Public Sub New(ByVal _Passport As roPassport)

            Me.oStateInfo = New wscState

            Me.Passport = _Passport
            Me.oChilds = New wscUserAdminList(Me.oPassport)

            If Me.Passport IsNot Nothing AndAlso Me.Passport.AuthenticationMethods IsNot Nothing Then
                Dim oAuth As roPassportAuthenticationMethodsRow = Me.Passport.AuthenticationMethods.PasswordRow
                Me.strLogin = oAuth.Credential
                Me.strPassword = oAuth.Password
            End If
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)

            Me.oStateInfo = New wscState

            Me.Passport = roPassportManager.GetPassport(_IDPassport)
            Me.oChilds = New wscUserAdminList(Me.oPassport)

            If Me.Passport IsNot Nothing AndAlso Me.Passport.AuthenticationMethods IsNot Nothing Then
                Dim oAuth As roPassportAuthenticationMethodsRow = Me.Passport.AuthenticationMethods.PasswordRow
                Me.strLogin = oAuth.Credential
                Me.strPassword = oAuth.Password
            End If

        End Sub

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.oStateInfo Is Nothing Then
                Me.oStateInfo = New wscState()
            End If
        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property Passport() As roPassport
            Get
                Return Me.oPassport
            End Get
            Set(ByVal value As roPassport)
                Me.oPassport = value
            End Set
        End Property

        <DataMember>
        Public Property Childs() As wscUserAdminList
            Get
                Return Me.oChilds
            End Get
            Set(ByVal value As wscUserAdminList)
                Me.oChilds = value
            End Set
        End Property

        <DataMember>
        Public Property Login() As String
            Get
                Return Me.strLogin
            End Get
            Set(ByVal value As String)
                Me.strLogin = value
            End Set
        End Property

        <DataMember>
        Public Property Password() As String
            Get
                Return Me.strPassword
            End Get
            Set(ByVal value As String)
                Me.strPassword = value
            End Set
        End Property

        <DataMember>
        Public Property NameOnDomain() As String
            Get
                Return Me.strNameOnDomain
            End Get
            Set(ByVal value As String)
                Me.strNameOnDomain = value
            End Set
        End Property

        <IgnoreDataMember>
        Public Property StateInfo() As wscState
            Get
                Return Me.oStateInfo
            End Get
            Set(ByVal value As wscState)
                Me.oStateInfo = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Update() As Boolean

            Dim bolRet As Boolean = False

            Me.oStateInfo.UpdateStateInfo()

            Try
                Dim bolNew As Boolean = False

                Dim oPassportManager As New roPassportManager
                If Me.oPassport Is Nothing Then
                    Me.oPassport = New roPassport()
                    bolNew = True
                End If

                With Me.oPassport
                    .Name = Me.oPassport.Name
                    .Description = Me.oPassport.Description
                    .Email = Me.oPassport.Email
                    .IDParentPassport = Me.oPassport.IDParentPassport
                End With

                If oPassport.AuthenticationMethods() Is Nothing Then
                    oPassport.AuthenticationMethods = New roPassportAuthenticationMethods
                End If

                Dim oAuth As roPassportAuthenticationMethodsRow = oPassport.AuthenticationMethods().PasswordRow
                Dim bolAddmethod As Boolean = False
                Dim oldCredential As String = String.Empty

                If oAuth Is Nothing Then
                    bolAddmethod = True
                    oAuth = New roPassportAuthenticationMethodsRow
                    With oAuth
                        .IDPassport = oPassport.ID
                        .Method = AuthenticationMethod.Password
                        .Version = ""
                        .BiometricID = 0
                        .RowState = RowState.UpdateRow
                    End With
                Else
                    oldCredential = oAuth.Credential
                End If

                oAuth.Credential = Me.strLogin

                If Not bolAddmethod Then
                    roPassportManager.SavePasswordHistory(Me.oPassport.ID, oAuth.Password, Now)
                End If
                oAuth.LastUpdatePassword = Now
                oAuth.Password = Me.strPassword
                oAuth.Enabled = True
                oAuth.RowState = RowState.UpdateRow
                oPassport.AuthenticationMethods.PasswordRow = oAuth

                oPassportManager.Save(oPassport, False)

                Me.strPassword = oAuth.Password

                bolRet = True
            Catch Ex As DbException
                Me.oStateInfo.UpdateStateInfo(Ex, "wscUserAdmin::Update")
            Catch Ex As Exception
                Me.oStateInfo.UpdateStateInfo(Ex, "wscUserAdmin::Update")
            End Try

            Return bolRet

        End Function

        Public Function Delete() As Boolean

            Dim bolRet As Boolean = False

            Me.oStateInfo.UpdateStateInfo()

            Try

                Dim oPassportManager As New roPassportManager
                Dim oPass As roPassport = oPassportManager.LoadPassport(Me.oPassport.ID, LoadType.Passport)

                Me.oStateInfo.Audit(Audit.Action.aDelete, Audit.ObjectType.tPassport, Me.oPassport.Name, Nothing, -1)

                oPassportManager.Delete(oPass)

                bolRet = True
            Catch Ex As DbException
                Me.oStateInfo.UpdateStateInfo(Ex, "wscUserAdmin::Delete")
            Catch Ex As Exception
                Me.oStateInfo.UpdateStateInfo(Ex, "wscUserAdmin::Delete")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper Methods"



        Public Shared Function BusinessGroupListInUse(ByVal strBusinessGroup As String, ByVal oState As roSecurityState) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim strQuery As String
                strQuery = "@SELECT# BusinessGroupList FROM sysroGroupFeatures WHERE ISNULL(BusinessGroupList, '') <> ''"
                Dim oRet As System.Data.DataTable = CreateDataTable(strQuery)
                If oRet IsNot Nothing Then
                    If oRet.Rows.Count > 0 Then
                        For Each rw As System.Data.DataRow In oRet.Rows
                            bolRet = roTypes.Any2String(rw("BusinessGroupList")).Contains(strBusinessGroup)
                            If bolRet Then Exit For
                        Next
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "wscUserAdmin::BusinessGroupListInUse")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

    <DataContract,
        KnownType(GetType(wscUserAdmin))>
    Public Class wscUserAdminList

#Region "Declarations - Constructor"

        Private oList As ArrayList

        Private oStateInfo As wscState

        Public Sub New()
            'Me.oStateInfo = New wscState
            'Me.oList = New ArrayList
            'LoadList(Nothing)
        End Sub

        Public Sub New(ByVal _PassportParent As roPassport)
            Me.oStateInfo = New wscState
            Me.oList = New ArrayList
            LoadList(_PassportParent)
        End Sub

#End Region

#Region "Properties"

        <XmlArray("Elements"), XmlArrayItem("wscUserAdmin", GetType(wscUserAdmin))>
        <DataMember>
        Public Property Elements() As ArrayList
            Get
                Return Me.oList
            End Get
            Set(ByVal value As ArrayList)
                Me.oList = value
            End Set
        End Property

        <IgnoreDataMember>
        Public Property StateInfo() As wscState
            Get
                Return Me.oStateInfo
            End Get
            Set(ByVal value As wscState)
                Me.oStateInfo = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub LoadList(ByVal oPassportParent As roPassport)

            Me.oStateInfo.UpdateStateInfo()

            Me.oList.Clear()

            Dim intIDPassportParent As New System.Nullable(Of Integer)
            Dim oUserAdmin As wscUserAdmin
            If oPassportParent IsNot Nothing Then
                intIDPassportParent = oPassportParent.ID
            Else
                intIDPassportParent = Nothing
            End If

            Try
                Dim oManager As New roPassportManager
                For Each intIDPassport As Integer In oManager.GetPassportsByParent(intIDPassportParent)
                    Dim tmpPassport As roPassport = oManager.LoadPassport(intIDPassport, LoadType.Passport)
                    oUserAdmin = New wscUserAdmin(tmpPassport)
                    Me.oList.Add(oUserAdmin)
                Next
            Catch Ex As DbException
                Me.oStateInfo.UpdateStateInfo(Ex, "wscUserAdminList::LoadList")
            Catch Ex As Exception
                Me.oStateInfo.UpdateStateInfo(Ex, "wscUserAdminList::LoadList")
            End Try

        End Sub

#End Region

    End Class

End Namespace
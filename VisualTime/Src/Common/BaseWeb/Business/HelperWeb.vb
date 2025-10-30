Imports System.Drawing
Imports System.Runtime.Serialization
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase

Public NotInheritable Class HelperWeb

    Public Const roNullDate As String = "1/1/2079"
    Public Shared RandomGenerator As System.Random = New System.Random()

    Public Shared Function getSeason(ByVal curDate As DateTime) As Integer
        Try
            Dim value As Double = CDbl(curDate.Month) + (curDate.Day / 100)

            If value < 3.21 Or value >= 12.22 Then
                Return 1 ' Winter
            ElseIf value < 6.21 Then
                Return 2 ' Spring
            ElseIf value < 9.23 Then
                Return 3 ' Summer
            Else
                Return 4 ' Autumn
            End If
        Catch ex As Exception

        End Try

        Return 2
    End Function

    Public Shared Function GetCookie(ByVal CookieID As String) As String
        If HttpContext.Current.Request.Cookies(CookieID) IsNot Nothing Then
            Return HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.Cookies(CookieID).Value)
        Else
            Return ""
        End If
    End Function

    Public Shared Sub CreateCookie(ByVal CookieID As String, ByVal Value As String, Optional bHttpOnly As Boolean = True, Optional dtExpireCookie As Nullable(Of DateTime) = Nothing)
        If HttpContext.Current.Request.Cookies(CookieID) IsNot Nothing Then
            Dim oCookie As New System.Web.HttpCookie(CookieID, HttpContext.Current.Server.UrlEncode(Value))
            oCookie.Expires = If(dtExpireCookie IsNot Nothing, dtExpireCookie.Value.ToUniversalTime(), Date.Now.AddDays(30).ToUniversalTime())
            oCookie.Path = "/"
            oCookie.SameSite = SameSiteMode.Strict
            oCookie.HttpOnly = bHttpOnly
            If HttpContext.Current.Request.IsSecureConnection Then
                oCookie.Secure = True
            End If

            HttpContext.Current.Response.Cookies.Add(oCookie)
        Else
            Dim oCookie As New System.Web.HttpCookie(CookieID, HttpContext.Current.Server.UrlEncode(Value))
            oCookie.Expires = If(dtExpireCookie IsNot Nothing, dtExpireCookie.Value.ToUniversalTime(), Date.Now.AddDays(30).ToUniversalTime())
            oCookie.SameSite = SameSiteMode.Strict
            oCookie.HttpOnly = bHttpOnly
            If HttpContext.Current.Request.IsSecureConnection Then
                oCookie.Secure = True
            End If
            oCookie.Path = "/"
            HttpContext.Current.Response.Cookies.Add(oCookie)
        End If
    End Sub

    Public Shared Sub EraseCookie(ByVal CookieID As String)
        If HttpContext.Current.Request.Cookies(CookieID) IsNot Nothing Then
            Dim myCookie As HttpCookie
            myCookie = New HttpCookie(CookieID)
            myCookie.Expires = DateTime.Now.AddDays(-1D)
            HttpContext.Current.Response.Cookies.Add(myCookie)
        End If
    End Sub

    'Public Shared Sub EditCookie(ByVal CookieID As String, ByVal Value As String)
    '    If HttpContext.Current.Request.Cookies(CookieID) IsNot Nothing Then
    '        HttpContext.Current.Response.Cookies(CookieID).Value = Value
    '    End If
    'End Sub

    Public Shared Function GetControl(ByVal Controls As ControlCollection, ByVal strControlID As String) As Control
        '
        ' Obtiene el objeto 'Control' con el identificador 'strControlID', de forma recursiva.
        '
        Dim oRet As Control = Nothing

        For Each oControl As Control In Controls
            If oControl.ID IsNot Nothing AndAlso oControl.ID.ToLower = strControlID.ToLower Then
                oRet = oControl
                Exit For
            ElseIf Not TypeOf oControl Is GridView Then
                oRet = GetControl(oControl.Controls, strControlID)
                If oRet IsNot Nothing Then Exit For
            End If
        Next

        Return oRet

    End Function

    Public Shared Function GetProperty(ByVal oObjParent As Object, ByVal strProperties() As String, ByRef oObjContainer As Object) As System.Reflection.PropertyInfo
        '
        ' Obtiene el objeto 'System.Reflection.PropertyInfo' correspondiente a la/s propiedad/es strProperties(0).strProperties(1). ...
        ' Establece el parámetro por referencia 'oObjContainer' con el objecto al que pertenece la propiedad.
        ' Para obtener una propiedad indexada, se informa  'NombrePropiedad#índice'
        ' Si no existe la propiedad o el ínidice especificado, devuelve nothing
        '
        Dim oRet As System.Reflection.PropertyInfo = Nothing
        Dim oObj As Object = Nothing

        Try

            Dim strID As String
            Dim intIndex As Integer

            For Each strProperty As String In strProperties
                If strProperty.IndexOf("#") > -1 Then
                    strID = strProperty.Split("#")(0)
                    intIndex = CInt(strProperty.Split("#")(1))
                Else
                    strID = strProperty
                    intIndex = -1
                End If
                If oRet Is Nothing Then
                    If Not oObjParent Is Nothing Then 'PPR: si el objeto es nothing (No provocamos la excepcion voluntariamente)
                        oRet = GetProperty(oObjParent.GetType.GetProperties, strID)
                        If Not oRet Is Nothing Then 'PPR: si el objeto es nothing (No provocamos la excepcion voluntariamente)
                            oObjContainer = oObjParent
                            oObj = oRet.GetValue(oObjContainer, Nothing)
                            If intIndex > -1 And oObj IsNot Nothing Then
                                oObj = oObj.Item(intIndex)
                            End If
                        End If
                    End If
                ElseIf oObj IsNot Nothing Then
                    oRet = GetProperty(oObj.GetType.GetProperties, strID)
                    oObjContainer = oObj
                    oObj = oRet.GetValue(oObjContainer, Nothing)
                    If intIndex > -1 And oObj IsNot Nothing Then
                        oObj = oObj.Item(intIndex)
                    End If
                Else
                    oRet = Nothing
                End If

                If oRet Is Nothing Then Exit For
            Next
        Catch
            oRet = Nothing
        End Try

        Return oRet

    End Function

    Public Shared Function GetProperty(ByVal Properties As System.Reflection.PropertyInfo(), ByVal strProperty As String) As System.Reflection.PropertyInfo
        '
        ' Busca la propiedad con nombre 'strProperty' en la lista de propiedades 'Properties'
        ' Devuelve el objeto 'System.Reflection.PropertyInfo' correspondiente a la propiedad.
        '
        Dim oRet As System.Reflection.PropertyInfo = Nothing

        For Each oProperty As System.Reflection.PropertyInfo In Properties
            If oProperty.Name = strProperty Then
                oRet = oProperty
                Exit For
            End If
        Next

        Return oRet

    End Function

    Public Enum MsgBoxIcons
        InformationIcon
        QuestionIcon
        ErrorIcon
        AlertIcon
        SuccessIcon
    End Enum

    Public Shared MsgBoxIconsUrl() As String = {"~/Base/Images/MessageFrame/dialog-information.png",
                                                "~/Base/Images/MessageFrame/dialog-question.png",
                                                "~/Base/Images/MessageFrame/stock_dialog-error.png",
                                                "~/Base/Images/MessageFrame/Alert32.png",
                                                "~/Base/Images/MessageFrame/dialog-warning.png"}

    Public Shared Sub EmployeeSelector_SetSelection(ByVal lstEmployeeSelection As ArrayList, ByVal lstGroupSelection As ArrayList, Optional ByVal strCookiePrefix As String = "")

        Dim strSelection As String = ""
        For Each strID As String In lstGroupSelection
            strSelection &= "A" & strID & ","
        Next
        For Each strID As String In lstEmployeeSelection
            strSelection &= "B" & strID & ","
        Next
        If strSelection <> "" Then strSelection = strSelection.Substring(0, strSelection.Length - 1)

        ' Actualizar cookie con la selección pasada
        HttpContext.Current.Session.Remove("EmployeeSelector_Selection")
        HttpContext.Current.Session.Add("EmployeeSelector_Selection", strSelection)

    End Sub

    Public Shared Sub EmployeeSelector_SetUserFieldsFilter(ByVal strFilter As String)

        HttpContext.Current.Session.Remove("EmployeeSelector_FilterUserFieldsValues")
        If strFilter <> "" Then
            HttpContext.Current.Session.Add("EmployeeSelector_FilterUserFieldsValues", strFilter)
        End If
        ' Activar o desactivar opción de filtro en función de si se ha establecido algún filtro
        HttpContext.Current.Session("EmployeeSelector_FilterUserFields") = (HttpContext.Current.Session("EmployeeSelector_FilterUserFieldsValues") IsNot Nothing)

    End Sub

    Public Shared Function LastEmployeeSelection(Optional ByRef lstEmployeeNamesSelection As ArrayList = Nothing) As ArrayList
        '
        ' Obtiene las listas de la última selección del selector de empleados y grupos
        '

        Dim strSelection As String = ""
        Dim strNameSelection As String = ""

        If HttpContext.Current.Request.Cookies("EmployeeSelector_Selection") IsNot Nothing Then
            strSelection = HttpContext.Current.Request.Cookies("EmployeeSelector_Selection").Value
        End If
        If HttpContext.Current.Request.Cookies("EmployeeSelector_NameSelection") IsNot Nothing Then
            strNameSelection = HttpContext.Current.Request.Cookies("EmployeeSelector_NameSelection").Value
        End If

        Dim lstEmployeeSelection As New ArrayList
        If lstEmployeeNamesSelection IsNot Nothing Then
            lstEmployeeNamesSelection = New ArrayList
        End If

        If strSelection <> "" Then
            Dim Selection() As String = strSelection.Trim.Split(",")
            If Selection.Length > 0 Then
                For Each Sel As String In Selection
                    Sel = Sel.Trim
                    If Sel <> "" Then
                        Select Case Sel.Substring(0, 1)
                            Case "A" ' Grupo

                            Case "B" ' Empleado
                                lstEmployeeSelection.Add(CInt(Sel.Substring(1)))

                        End Select
                    End If
                Next
            End If
        End If
        If strNameSelection <> "" Then
            Dim Selection() As String = strNameSelection.Trim.Split("%")
            If Selection.Length > 0 Then
                For Each Sel As String In Selection
                    Sel = Sel.Trim
                    If Sel <> "" Then
                        If Sel.Split("#").Length = 2 Then
                            Select Case Sel.Split("#")(0).Substring(0, 1)
                                Case "A" ' Grupo

                                Case "B" ' Empleado
                                    If lstEmployeeNamesSelection IsNot Nothing Then _
                                        lstEmployeeNamesSelection.Add(Sel.Split("#")(1).Trim)

                            End Select
                        End If
                    End If
                Next
            End If
        End If

        Return lstEmployeeSelection

    End Function

    Public Shared Function roSelector_GetTreeState(ByVal strPrefixTree As String) As roTreeState

        Dim oRet As New roTreeState

        Try

            If Not strPrefixTree.StartsWith("TreeState_") Then strPrefixTree = "TreeState_" & strPrefixTree

            Dim strJSONTreeState As String = API.SecurityV3ServiceMethods.GetTreeState(Nothing, WLHelperWeb.CurrentPassportID, strPrefixTree)

            If strJSONTreeState <> "" Then
                'strJSONTreeState = strJSONTreeState.Replace("..EQUAL..", "=")
                oRet = roJSONHelper.Deserialize(strJSONTreeState, oRet.GetType)
            Else
                oRet.ID = strPrefixTree
            End If
        Catch ex As Exception

        End Try

        Return oRet

    End Function

    Public Shared Function roSelector_SetTreeState(ByVal oTreeState As roTreeState) As Boolean

        Dim bolRet As Boolean

        Try

            EraseCookie("TreeState_" & oTreeState.ID)
            EraseCookie("TreeState_" & oTreeState.ID & "_Big")

            'System.Web.HttpContext.Current.Session("TreeState_" & oTreeState.ID) = roJSONHelper.Serialize(oTreeState) '.Replace("=", "..EQUAL..")


            If Not oTreeState.ID.StartsWith("TreeState_") Then oTreeState.ID = "TreeState_" & oTreeState.ID

            bolRet = API.SecurityV3ServiceMethods.SaveTreeState(Nothing, WLHelperWeb.CurrentPassportID, oTreeState.ID, roJSONHelper.Serialize(oTreeState))

        Catch ex As Exception

        End Try

        Return bolRet

    End Function

    Public Shared Function roSelector_DeleteTreeState(ByVal oTreeState As roTreeState) As Boolean

        Dim bolRet As Boolean = False

        Try

            HttpContext.Current.Response.Cookies.Remove("TreeState_" & oTreeState.ID)

            HttpContext.Current.Response.Cookies.Remove("TreeState_" & oTreeState.ID & "_Big")

            bolRet = True
        Catch ex As Exception

        End Try

        Return bolRet

    End Function

    Public Shared Function roSelector_DeleteTreeState(ByVal strPrefixTree As String) As Boolean

        Dim bolRet As Boolean = False

        Try

            HttpContext.Current.Response.Cookies.Remove("TreeState_" & strPrefixTree)

            HttpContext.Current.Response.Cookies.Remove("TreeState_" & strPrefixTree & "_Big")

            bolRet = True
        Catch ex As Exception

        End Try

        Return bolRet

    End Function

    ''' <summary>
    ''' Establece las cookies de selección necesarias para establecer la selección en el árbol.
    ''' </summary>
    ''' <param name="lstEmployeeSelection">Lista de empleados a seleccionar</param>
    ''' <param name="lstGroupSelection">Lista de grupos a seleccionar</param>
    ''' <param name="strPrefixTree">Prefijo del árbol de selección</param>
    ''' <param name="bolSetExpanded">Actualiza la cookie para que se expanden los nodos del árbol marcados</param>
    ''' <remarks></remarks>
    Public Shared Sub roSelector_SetSelection(ByVal lstEmployeeSelection As Generic.List(Of Integer), ByVal lstGroupSelection As Generic.List(Of Integer), ByVal strPrefixTree As String, Optional ByVal bolSetExpanded As Boolean = True, Optional ByVal strFiltersVisible As String = "11110")

        Dim strSelection As String = ""
        For Each ID As Integer In lstGroupSelection
            strSelection &= "A" & ID.ToString & ","
        Next
        For Each ID As Integer In lstEmployeeSelection
            strSelection &= "B" & ID.ToString & ","
        Next
        If strSelection <> "" Then strSelection = strSelection.Substring(0, strSelection.Length - 1)

        Dim oTreeState As roTreeState = roSelector_GetTreeState(strPrefixTree)

        ' Actualizar cookie con la selección pasada
        oTreeState.Selected1 = strSelection
        oTreeState.Filter = strFiltersVisible

        If bolSetExpanded Then
            Dim lstExpanded As Generic.List(Of Integer) = API.EmployeeGroupsServiceMethods.GetGroupSelectionPath(Nothing, lstGroupSelection, lstEmployeeSelection)
            Dim strExpanded As String = "source"
            For Each id As Integer In lstExpanded
                strExpanded &= ",A" & id.ToString
            Next
            oTreeState.Expanded1 = strExpanded
        End If

        roSelector_SetTreeState(oTreeState)

    End Sub

    ''' <summary>
    ''' Establece las cookies de selección necesarias para establecer la selección en el árbol.
    ''' </summary>
    ''' <param name="strSelection">Lista de empleados y grupos a seleccionar</param>
    ''' <param name="strPrefixTree">Prefijo del árbol de selección</param>
    ''' <remarks></remarks>
    Public Shared Sub roSelector_SetSelection(ByVal strSelection As String, ByVal strSelectionPath As String, ByVal strPrefixTree As String, Optional ByVal strFiltersVisible As String = "11110", Optional ByVal strUserFieldFilter As String = "", Optional ByVal strActiveTree As String = "")

        Dim oTreeState As roTreeState = roSelector_GetTreeState(strPrefixTree)

        oTreeState.Selected1 = strSelection
        oTreeState.SelectedPath1 = strSelectionPath

        If strActiveTree <> "" Then oTreeState.ActiveTree = strActiveTree
        If strFiltersVisible <> "ignore" Then oTreeState.Filter = strFiltersVisible
        If strUserFieldFilter <> "ignore" Then oTreeState.UserFieldFilter = HttpUtility.UrlEncode(strUserFieldFilter)

        roSelector_SetTreeState(oTreeState)

    End Sub

    ''' <summary>
    ''' Establece las cookies de selección necesarias para establecer los filtros en el árbol.
    ''' </summary>
    ''' <param name="strFilters"></param>
    ''' <param name="strPrefixTree">Prefijo del árbol de selección</param>
    ''' <remarks></remarks>
    Public Shared Sub roSelector_SetFilters(ByVal strFilters As String, ByVal strPrefixTree As String)

        Dim oTreeState As roTreeState = roSelector_GetTreeState(strPrefixTree)
        oTreeState.Filter = strFilters
        roSelector_SetTreeState(oTreeState)
    End Sub

    ''' <summary>
    ''' Establece las cookies de selección necesarias para establecer los filtros por campos de la ficha en el árbol.
    ''' </summary>
    '''<param name="strUserFieldsFilter"></param>
    ''' <param name="strPrefixTree">Prefijo del árbol de selección</param>
    ''' <remarks></remarks>
    Public Shared Sub roSelector_SetUserFieldsFilter(ByVal strUserFieldsFilter As String, ByVal strPrefixTree As String)
        Dim oTreeState As roTreeState = roSelector_GetTreeState(strPrefixTree)
        oTreeState.UserFieldFilter = HttpUtility.UrlEncode(strUserFieldsFilter)
        roSelector_SetTreeState(oTreeState)
    End Sub

    ''' <summary>
    ''' Inicializa las cookies de configuración para el selector de árbol indicado.
    ''' </summary>
    ''' <param name="strPrefixTree">Prefijo del árbol de selección</param>
    ''' <remarks></remarks>
    Public Shared Sub roSelector_Initialize(ByVal strPrefixTree As String)
        Dim oTreeState As New roTreeState(strPrefixTree)
        roSelector_SetTreeState(oTreeState)
    End Sub

    ''' <summary>
    ''' Destruye las cookies de configuración para el selector de árbol indicado.
    ''' </summary>
    ''' <param name="strPrefixTree">Prefijo del árbol de selección</param>
    ''' <remarks></remarks>
    Public Shared Sub roSelector_Finalize(ByVal strPrefixTree As String)
        roSelector_DeleteTreeState(strPrefixTree)
    End Sub

    Public Enum EmployeeSelector_IDType
        _Employee
        _Group
    End Enum

    Public Shared Sub EmptyGridFix(ByVal oGridView As GridView, Optional ByVal strTableName As String = Nothing, Optional ByVal oEmptyControl As Control = Nothing)
        ' Determina si hay una sola fila en el origen de datos y si está vacía, la hace invisible en el 'GridView'
        ' Si se le pasa el control opcional 'oEmptyControl' lo hace visible si hay una sola fila y está vacía.
        Dim bolVisible As Boolean = True

        If oGridView.Rows.Count = 1 Then
            Dim tb As DataTable = Nothing
            If TypeOf oGridView.DataSource Is DataSet Then
                If strTableName Is Nothing Then
                    tb = CType(oGridView.DataSource, DataSet).Tables(0)
                Else
                    tb = CType(oGridView.DataSource, DataSet).Tables(strTableName)
                End If
            ElseIf TypeOf oGridView.DataSource Is DataTable Then
                tb = oGridView.DataSource
            End If
            If tb IsNot Nothing Then
                Dim intRowIndex As Integer = 0
                For n As Integer = 0 To tb.Rows.Count - 1
                    If n <= intRowIndex Then
                        If tb.Rows(n).RowState = DataRowState.Deleted Then
                            intRowIndex += 1
                        End If
                    Else
                        Exit For
                    End If
                Next
                If tb.Rows(intRowIndex).RowState = DataRowState.Added Then
                    bolVisible = Not EmptyRow(tb.Rows(intRowIndex))
                End If
                oGridView.Rows(0).Visible = bolVisible
            End If
        End If

        If oEmptyControl IsNot Nothing Then
            oGridView.Visible = bolVisible
            oEmptyControl.Visible = (Not bolVisible)
        End If

    End Sub

    Public Shared Function ArrayList2ObjectList(ByVal lst As ArrayList) As Object()

        Dim oRet() As Object

        If lst IsNot Nothing Then
            ReDim oRet(lst.Count - 1)
            For n As Integer = 0 To lst.Count - 1
                oRet(n) = lst(n)
            Next
        Else
            ReDim oRet(-1)
        End If

        Return oRet

    End Function

    Public Shared Function ObjectList2ArrayList(ByVal obj() As Object) As ArrayList

        Dim lst As New ArrayList
        If obj IsNot Nothing Then
            For Each item As Object In obj
                lst.Add(item)
            Next
        End If
        Return lst

    End Function

    Public Shared Function EmptyRow(ByVal oRow As DataRow) As Boolean
        Dim bolRet As Boolean = True
        For n As Integer = 0 To oRow.Table.Columns.Count - 1
            If Not oRow.IsNull(n) Then
                bolRet = False
                Exit For
            End If
        Next
        Return bolRet
    End Function

    Public Shared Sub DoMasterInvisible(ByVal oPage As PageBase)

        If oPage.Master IsNot Nothing Then

            Try
                CType(oPage.Master, Object).DoInvisible()
            Catch
            End Try

        End If

    End Sub

    Public Shared Function GetIDList(ByVal IdsArray As ArrayList) As String

        Dim strResult As String
        Dim strId As String

        strResult = ""

        If IdsArray IsNot Nothing Then
            For Each strId In IdsArray
                If strResult = "" Then
                    strResult = strId
                Else
                    strResult = strResult & ", " & strId
                End If
            Next
        End If

        Return strResult

    End Function

    Public Shared Function GetDecimalDigitFormat() As String
        Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
        Return oInfo.NumberDecimalSeparator
    End Function

    Public Shared Function GetShortDateFormat() As String
        Dim oInfo As System.Globalization.DateTimeFormatInfo = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat
        Return oInfo.ShortDatePattern
    End Function

    Public Shared Function GetShortDateSeparator() As String
        Dim oInfo As System.Globalization.DateTimeFormatInfo = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat
        Return oInfo.DateSeparator
    End Function

    Public Shared Function GetShortTimeFormat() As String
        Dim oInfo As System.Globalization.DateTimeFormatInfo = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat
        Return "HH:mm" 'oInfo.ShortTimePatter
    End Function

    Public Shared Function GetMonthAndDayDateFormat() As String
        Dim oInfo As System.Globalization.DateTimeFormatInfo = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat
        Dim strResult As String = oInfo.ShortDatePattern
        strResult = strResult.Replace("yy", "")
        If strResult.StartsWith(oInfo.DateSeparator) Then strResult = strResult.Substring(oInfo.DateSeparator.Length)
        If strResult.EndsWith(oInfo.DateSeparator) Then strResult = strResult.Substring(0, strResult.Length - oInfo.DateSeparator.Length)
        Return strResult
    End Function

    Public Shared Function ResizeImage(ByVal imgPhoto As System.Drawing.Image, ByVal Width As Integer, ByVal Height As Integer, Optional ByVal fillColor As String = "#FFFFFF") As System.Drawing.Image
        Dim sourceWidth As Integer = imgPhoto.Width
        Dim sourceHeight As Integer = imgPhoto.Height
        Dim sourceX As Integer = 0
        Dim sourceY As Integer = 0
        Dim destX As Integer = 0
        Dim destY As Integer = 0

        Dim nPercent As Double = 0
        Dim nPercentW As Double = 0
        Dim nPercentH As Double = 0

        nPercentW = (CType(Width, Double) / CType(sourceWidth, Double))
        nPercentH = (CType(Height, Double) / CType(sourceHeight, Double))

        'if we have to pad the height pad both the top and the bottom
        'with the difference between the scaled height and the desired height

        If (nPercentH < nPercentW) Then
            nPercent = nPercentH
            destX = ((Width - (sourceWidth * nPercent)) / 2)
        Else
            nPercent = nPercentW
            destY = ((Height - (sourceHeight * nPercent)) / 2)
        End If

        Dim destWidth As Integer = (sourceWidth * nPercent)
        Dim destHeight As Integer = (sourceHeight * nPercent)

        Dim bmPhoto As Bitmap = New Bitmap(Width, Height, Imaging.PixelFormat.Format24bppRgb)
        'bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution)

        Dim grPhoto As Graphics = Graphics.FromImage(bmPhoto)
        grPhoto.Clear(System.Drawing.ColorTranslator.FromHtml(fillColor))
        'grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic

        grPhoto.DrawImage(imgPhoto,
         New Rectangle(destX, destY, destWidth, destHeight),
         New Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
         GraphicsUnit.Pixel)

        grPhoto.Dispose()
        Return bmPhoto
    End Function

    '=====================================================================
    'FUNCIONES OBSOLETAS =================================================
    '=====================================================================

#Region "Funciones Show"

    Public Enum roJSErrorType
        roJsSuccess
        roJsError
        roJsWarning
        roJsInfo
        roJsQuestion
    End Enum

    <DataContract>
    Private Class roJSError

        Public Sub New()
            MsgType = roJSErrorType.roJsInfo
            ReturnCode = String.Empty

            DescriptionKey = String.Empty
            TitleKey = roJSErrorType.roJsInfo.ToString
            DescriptionText = String.Empty
            TitleText = String.Empty

            Button1Text = String.Empty
            Button1TextKey = String.Empty
            Button1Script = String.Empty
            Button1Description = String.Empty
            Button1DescriptionKey = String.Empty

            Button2Text = String.Empty
            Button2TextKey = String.Empty
            Button2Script = String.Empty
            Button2Description = String.Empty
            Button2DescriptionKey = String.Empty

            Button3Text = String.Empty
            Button3TextKey = String.Empty
            Button3Script = String.Empty
            Button3Description = String.Empty
            Button3DescriptionKey = String.Empty

            Button4Text = String.Empty
            Button4TextKey = String.Empty
            Button4Script = String.Empty
            Button4Description = String.Empty
            Button4DescriptionKey = String.Empty
        End Sub

        <DataMember>
        Public Property MsgType As roJSErrorType

        <DataMember>
        Public Property TitleText As String

        <DataMember>
        Public Property TitleKey As String

        <DataMember>
        Public Property ReturnCode As String

        <DataMember>
        Public Property DescriptionText As String

        <DataMember>
        Public Property DescriptionKey As String

        <DataMember>
        Public Property Button1Text As String

        <DataMember>
        Public Property Button1TextKey As String

        <DataMember>
        Public Property Button1Script As String

        <DataMember>
        Public Property Button1Description As String

        <DataMember>
        Public Property Button1DescriptionKey As String

        <DataMember>
        Public Property Button2Text As String

        <DataMember>
        Public Property Button2TextKey As String

        <DataMember>
        Public Property Button2Script As String

        <DataMember>
        Public Property Button2Description As String

        <DataMember>
        Public Property Button2DescriptionKey As String

        <DataMember>
        Public Property Button3Text As String

        <DataMember>
        Public Property Button3TextKey As String

        <DataMember>
        Public Property Button3Script As String

        <DataMember>
        Public Property Button3Description As String

        <DataMember>
        Public Property Button3DescriptionKey As String

        <DataMember>
        Public Property Button4Text As String

        <DataMember>
        Public Property Button4TextKey As String

        <DataMember>
        Public Property Button4Script As String

        <DataMember>
        Public Property Button4Description As String

        <DataMember>
        Public Property Button4DescriptionKey As String

    End Class

    Public Shared Sub GetControlList(Of T As Control)(ByVal controlCollection As ControlCollection, ByRef resultCollection As List(Of T))
        For Each control As Control In controlCollection
            If TypeOf control Is T Then resultCollection.Add(CType(control, T))
            If control.HasControls() Then GetControlList(control.Controls, resultCollection)
        Next
    End Sub

    Public Shared Sub ShowError(ByVal oPage As Page, ByVal oState As Object, Optional ByVal oErrorCode As String = "", Optional ByVal Icon As MsgBoxIcons = MsgBoxIcons.AlertIcon)
        If oPage IsNot Nothing Then
            Dim strMessage As String = String.Empty
            If oState IsNot Nothing AndAlso oState.ErrorText IsNot Nothing Then
                strMessage = oState.ErrorText.ToString
                If strMessage = "" AndAlso oState.Result IsNot Nothing Then
                    strMessage = oState.Result.ToString
                End If
            End If

            If oErrorCode = String.Empty AndAlso oState.ReturnCode <> String.Empty Then
                oErrorCode = oState.ReturnCode
            End If

            HelperWeb.ShowMessage(oPage, "", strMessage, , True, False, Icon,, oErrorCode)
        End If
    End Sub

    Public Shared Function ShowMessage(ByVal oPage As Page, ByVal strTitle As String, ByVal strMessage As String, Optional ByVal strAcceptButtonKey As String = "",
                                       Optional ByVal bolCloseClient As Boolean = True, Optional ByVal bolEventServer As Boolean = False,
                                       Optional ByVal iconType As MsgBoxIcons = MsgBoxIcons.InformationIcon, Optional ByVal strClientClickScript As String = "", Optional ByVal oErrorCode As String = "") As Object
        If oPage IsNot Nothing Then

            Dim oLogLevel As Robotics.Base.DTOs.VTLiveLogLevel = ActualLogLevel()
            Dim bShowMessage As Boolean = False

            Select Case oLogLevel
                Case Robotics.Base.DTOs.VTLiveLogLevel.None
                    bShowMessage = False
                Case Robotics.Base.DTOs.VTLiveLogLevel.Debug
                    bShowMessage = True
                Case Robotics.Base.DTOs.VTLiveLogLevel.Coded
                    If oErrorCode <> String.Empty Then bShowMessage = True
                Case Robotics.Base.DTOs.VTLiveLogLevel.Fail
                    If iconType = MsgBoxIcons.ErrorIcon OrElse oErrorCode <> String.Empty Then bShowMessage = True
                Case Robotics.Base.DTOs.VTLiveLogLevel.Warning
                    If iconType = MsgBoxIcons.ErrorIcon OrElse oErrorCode <> String.Empty OrElse iconType = MsgBoxIcons.AlertIcon Then bShowMessage = True
                Case Robotics.Base.DTOs.VTLiveLogLevel.Info
                    If iconType = MsgBoxIcons.ErrorIcon OrElse oErrorCode <> String.Empty OrElse iconType = MsgBoxIcons.AlertIcon OrElse iconType = MsgBoxIcons.InformationIcon OrElse iconType = MsgBoxIcons.SuccessIcon Then bShowMessage = True
            End Select

            Dim oError As New roJSError

            Select Case iconType
                Case MsgBoxIcons.AlertIcon
                    oError.MsgType = roJSErrorType.roJsWarning
                Case MsgBoxIcons.ErrorIcon
                    oError.MsgType = roJSErrorType.roJsError
                Case MsgBoxIcons.InformationIcon
                    oError.MsgType = roJSErrorType.roJsInfo
                Case MsgBoxIcons.SuccessIcon
                    oError.MsgType = roJSErrorType.roJsSuccess
                Case MsgBoxIcons.QuestionIcon
                    oError.MsgType = roJSErrorType.roJsQuestion
            End Select

            oError.ReturnCode = oErrorCode
            oError.TitleKey = oError.MsgType.ToString
            oError.DescriptionText = strMessage
            oError.Button1TextKey = If(strAcceptButtonKey <> String.Empty, strAcceptButtonKey, "roErrorClose")
            oError.Button1Script = strClientClickScript
            oError.Button1DescriptionKey = ""

            If oPage.IsCallback Then
                SendMessageToCallbacks(oPage, oError)

                Return Nothing
            Else
                Dim strAcceptText As String = String.Empty
                Try
                    Dim pageB As Robotics.Web.Base.NoCachePageBase = CType(oPage, Robotics.Web.Base.NoCachePageBase)
                    strAcceptText = pageB.Language.Keyword("Button.Accept")
                Catch ex As Exception
                    Try
                        Dim x As New roLanguage()
                        x.SetLanguageReference("", WLHelperWeb.CurrentPassport(True).Language.Key)
                        strAcceptText = x.Keyword("Button.Accept")
                    Catch
                        strAcceptText = "Aceptar"
                    End Try
                End Try

                Dim oMessageFrame As Object = Nothing

                If oPage.Master IsNot Nothing Then
                    oMessageFrame = GetControl(oPage.Master.Controls, "MessageFrame1")
                End If

                If oMessageFrame Is Nothing Then
                    oMessageFrame = GetControl(oPage.Controls, "MessageFrame1")
                End If

                If oMessageFrame IsNot Nothing Then
                    'TODO se tiene que quitar esto para añadir un mostrar mensjae codificados cuando los encuentre
                    bShowMessage = True
                    If bShowMessage OrElse iconType = MsgBoxIcons.QuestionIcon Then

                        If Not strMessage.Contains(oErrorCode) Then strMessage = oErrorCode & ": " & strMessage

                        With oMessageFrame
                            .IconUrl = MsgBoxIconsUrl(iconType)
                            .Title = If(strTitle = String.Empty, "Error", strTitle)
                            .TitleDescription = strMessage
                            .Option1Text = strAcceptText
                            .Option1Key = strAcceptButtonKey
                            If strClientClickScript = "" Then
                                .Option1Click = "HideOptionMessage(1); return OptionEventServer(1);"
                            Else
                                .Option1Click = strClientClickScript
                            End If
                            .Option1Description = .AcceptDescription
                            .Option1CloseClient = bolCloseClient
                            .Option1EventServer = bolEventServer
                            .Option2Text = ""
                            .Option2Key = ""
                            .Option2Description = ""
                            .Option3Text = ""
                            .Option3Key = ""
                            .Option3Description = ""
                            .AlertText = ""

                        End With

                        oMessageFrame.width = 400
                        oMessageFrame.show()
                        Try
                            oMessageFrame.Update()
                        Catch ex As Exception

                        End Try
                    End If

                    Return oMessageFrame
                Else

                    Try
                        If bShowMessage OrElse iconType = MsgBoxIcons.QuestionIcon Then
                            Dim script As ClientScriptManager
                            script = oPage.ClientScript
                            script.RegisterStartupScript(GetType(String), "ErrorDuringLoad", "<script type=""text/javascript"" language=""JavaScript""> " & vbCrLf & "setTimeout(function(){Robotics.Client.JSErrors.showJSerror('" & roJSONHelper.Serialize(oError) & "', '');},1000); " & vbCrLf & "</script>")
                        End If
                    Catch ex As Exception

                    End Try

                    Return Nothing
                End If
            End If
        Else
            Return Nothing
        End If

    End Function

    Private Shared Function ActualLogLevel() As Robotics.Base.DTOs.VTLiveLogLevel
        Dim strLogLevel As String = roTypes.Any2String(HelperSession.AdvancedParametersCache("VTLive.Debug.LogLevel"))
        Dim eLogLevel As Robotics.Base.DTOs.VTLiveLogLevel = Robotics.Base.DTOs.VTLiveLogLevel.None

        Select Case strLogLevel.Trim.ToUpper
            Case "NONE"
                eLogLevel = Robotics.Base.DTOs.VTLiveLogLevel.None
            Case "CODED"
                eLogLevel = Robotics.Base.DTOs.VTLiveLogLevel.Coded
            Case "INFO"
                eLogLevel = Robotics.Base.DTOs.VTLiveLogLevel.Info
            Case "WARNING"
                eLogLevel = Robotics.Base.DTOs.VTLiveLogLevel.Warning
            Case "FAIL"
                eLogLevel = Robotics.Base.DTOs.VTLiveLogLevel.Fail
            Case "DEBUG"
                eLogLevel = Robotics.Base.DTOs.VTLiveLogLevel.Debug
            Case Else
                eLogLevel = Robotics.Base.DTOs.VTLiveLogLevel.Coded
        End Select

        Return eLogLevel
    End Function

    Private Shared Sub SendMessageToCallbacks(oPage As Page, oError As roJSError)

        Dim oLogLevel As Robotics.Base.DTOs.VTLiveLogLevel = ActualLogLevel()
        Dim bShowMessage As Boolean = False

        Select Case oLogLevel
            Case Robotics.Base.DTOs.VTLiveLogLevel.None
                bShowMessage = False
            Case Robotics.Base.DTOs.VTLiveLogLevel.Debug
                bShowMessage = True
            Case Robotics.Base.DTOs.VTLiveLogLevel.Coded
                If oError.ReturnCode <> String.Empty Then bShowMessage = True
            Case Robotics.Base.DTOs.VTLiveLogLevel.Fail
                If oError.MsgType = roJSErrorType.roJsError OrElse oError.ReturnCode <> String.Empty Then bShowMessage = True
            Case Robotics.Base.DTOs.VTLiveLogLevel.Warning
                If oError.MsgType = roJSErrorType.roJsError OrElse oError.ReturnCode <> String.Empty OrElse oError.MsgType = roJSErrorType.roJsWarning Then bShowMessage = True
            Case Robotics.Base.DTOs.VTLiveLogLevel.Info
                If oError.MsgType = roJSErrorType.roJsError OrElse oError.ReturnCode <> String.Empty OrElse oError.MsgType = roJSErrorType.roJsWarning OrElse oError.MsgType = roJSErrorType.roJsInfo OrElse oError.MsgType = roJSErrorType.roJsSuccess Then bShowMessage = True
        End Select

        If bShowMessage OrElse oError.MsgType = roJSErrorType.roJsQuestion Then
            oError.DescriptionText = oError.DescriptionText.Replace("\", "/").Replace("'", "")
            Dim jsErrorScript As String = "Robotics.Client.JSErrors.showJSerror('" & roJSONHelper.Serialize(oError) & "', '');"
            Dim exScript As New WebControl(HtmlTextWriterTag.Script)
            exScript.Attributes("id") = "dxss_" & DateTime.Now.ToString("yyyyMMddHHss")
            exScript.Attributes("type") = "text/javascript"
            exScript.Controls.Add(New LiteralControl(jsErrorScript))

            Dim oCallbackPanelCollection As New Generic.List(Of ASPxCallbackPanel)
            GetControlList(Of ASPxCallbackPanel)(oPage.Controls, oCallbackPanelCollection)

            Dim bExecuted As Boolean = False
            Try
                If oCallbackPanelCollection.Count > 0 Then
                    For Each oControl As ASPxCallbackPanel In oCallbackPanelCollection
                        If oControl.IsCallback Then
                            oControl.Controls.Add(exScript)
                            bExecuted = True
                            Exit For
                        End If
                    Next
                End If
            Catch ex As Exception
            End Try

            If Not bExecuted Then
                Dim oCallbackCollection As New Generic.List(Of ASPxCallback)
                GetControlList(Of ASPxCallback)(oPage.Controls, oCallbackCollection)

                Try
                    If oCallbackCollection.Count > 0 Then
                        For Each oControl As ASPxCallback In oCallbackCollection
                            If oControl.IsCallback Then
                                oControl.JSProperties("cpJsErrorScript") = jsErrorScript
                                Exit For
                            End If
                        Next
                    End If
                Catch ex As Exception
                End Try
            End If
        End If

    End Sub

    Public Shared Sub ShowOptionMessage(ByVal oPage As Page, ByVal Title As String, ByVal Description As String,
                                             ByVal Option1text As String, ByVal Option1Key As String, ByVal Option1Description As String, ByVal Option1ClientScript As String, ByVal Option1CloseClient As Boolean, ByVal Option1EventServer As Boolean,
                                             ByVal Option2Text As String, ByVal Option2Key As String, ByVal Option2Description As String, ByVal Option2ClientScript As String, ByVal Option2CloseClient As Boolean, ByVal Option2EventServer As Boolean,
                                             ByVal Option3Text As String, ByVal Option3Key As String, ByVal Option3Description As String, ByVal Option3ClientScript As String, ByVal Option3CloseClient As Boolean, ByVal Option3EventServer As Boolean,
                                             ByVal Option4Text As String, ByVal Option4Key As String, ByVal Option4Description As String, ByVal Option4ClientScript As String, ByVal Option4CloseClient As Boolean, ByVal Option4EventServer As Boolean,
                                             ByVal AlertText As String, Optional ByVal iconType As MsgBoxIcons = MsgBoxIcons.InformationIcon, Optional ByVal oErrorCode As String = "")

        If oPage IsNot Nothing Then
            If oErrorCode <> String.Empty Then Description = oErrorCode & ":" & Description

            If Not oPage.IsPostBack AndAlso oPage.IsCallback Then
                Dim oError As New roJSError

                Select Case iconType
                    Case MsgBoxIcons.AlertIcon
                        oError.MsgType = roJSErrorType.roJsWarning
                    Case MsgBoxIcons.ErrorIcon
                        oError.MsgType = roJSErrorType.roJsError
                    Case MsgBoxIcons.InformationIcon
                        oError.MsgType = roJSErrorType.roJsInfo
                    Case MsgBoxIcons.SuccessIcon
                        oError.MsgType = roJSErrorType.roJsSuccess
                    Case MsgBoxIcons.QuestionIcon
                        oError.MsgType = roJSErrorType.roJsQuestion
                End Select

                oError.ReturnCode = oErrorCode
                oError.TitleText = oError.MsgType.ToString
                oError.DescriptionText = Description
                oError.Button1Text = Option1text
                oError.Button1Description = Option1Description
                oError.Button1Script = Option1ClientScript

                oError.Button2Text = Option1text
                oError.Button2Description = Option2Description
                oError.Button2Script = Option2ClientScript

                oError.Button3Text = Option3Text
                oError.Button3Description = Option3Description
                oError.Button3Script = Option3ClientScript

                oError.Button4Text = Option4Text
                oError.Button4Description = Option4Description
                oError.Button4Script = Option4ClientScript

                SendMessageToCallbacks(oPage, oError)
            Else
                Dim oMessageFrame As Object = Nothing

                If oPage.Master IsNot Nothing Then
                    oMessageFrame = GetControl(oPage.Master.Controls, "MessageFrame1")
                End If

                If oMessageFrame Is Nothing Then
                    oMessageFrame = GetControl(oPage.Controls, "MessageFrame1")
                End If

                If oMessageFrame IsNot Nothing Then

                    With oMessageFrame
                        .IconUrl = MsgBoxIconsUrl(iconType)
                        .Title = Title
                        .TitleDescription = Description
                        .Option1Text = Option1text
                        .Option1Key = Option1Key
                        .Option1Description = Option1Description
                        .Option1Click = Option1ClientScript & " HideOptionMessage(1); return OptionEventServer(1);"
                        .Option1CloseClient = Option1CloseClient
                        .Option1EventServer = Option1EventServer
                        .Option2Text = Option2Text
                        .Option2Key = Option2Key
                        .Option2Description = Option2Description
                        .Option2Click = Option2ClientScript & " HideOptionMessage(2); return OptionEventServer(2);"
                        .Option2CloseClient = Option2CloseClient
                        .Option2EventServer = Option2EventServer
                        .Option3Text = Option3Text
                        .Option3Key = Option3Key
                        .Option3Description = Option3Description
                        .Option3Click = Option3ClientScript & " HideOptionMessage(3); return OptionEventServer(3);"
                        .Option3CloseClient = Option3CloseClient
                        .Option3EventServer = Option3EventServer
                        .Option4Text = Option4Text
                        .Option4Key = Option4Key
                        .Option4Description = Option4Description
                        .Option4Click = Option4ClientScript & " HideOptionMessage(4); return OptionEventServer(4);"
                        .Option4CloseClient = Option4CloseClient
                        .Option4EventServer = Option4EventServer
                        .AlertText = AlertText
                    End With

                    oMessageFrame.width = 400
                    oMessageFrame.show()

                    Try

                        Dim type As Type = oMessageFrame.GetType()
                        If type.GetMethod("Update") IsNot Nothing Then oMessageFrame.Update()
                    Catch ex As Exception
                    End Try
                End If
            End If
        End If

    End Sub

#End Region

    'obtener MAC Address del servidor
    Public Shared Function GetMACClientAddress() As String
        Dim macAddresses As String = ""
        For Each nic As System.Net.NetworkInformation.NetworkInterface In System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
            If nic.OperationalStatus = Net.NetworkInformation.OperationalStatus.Up Then
                macAddresses += nic.GetPhysicalAddress().ToString()
                Exit For
            End If
        Next
        Return macAddresses
    End Function

#Region "Gestion Empleados para páginas de Analítica"

    Public Shared Function GetNumOfEmployeesAnalitics(ByVal strEmployees As String, ByVal strFilter As String, ByVal strFilterUser As String, ByVal strFeature As String,
                                                      ByVal DateInf As DateTime, ByVal DateSup As DateTime) As Integer
        Dim tmp As Generic.List(Of Integer) = Robotics.Security.roSelector.GetEmployeeList(WLHelperWeb.CurrentPassportID, strFeature, "U", Nothing, strEmployees, strFilter, strFilterUser, False, DateInf, DateSup)

        Return tmp.Count

    End Function

#End Region

    Public Shared Function ParseFilterUserFields(ByVal _Filter As String) As String

        Dim strRet As String = ""

        If _Filter <> "" Then
            _Filter = HttpUtility.UrlDecode(_Filter)

            Dim strFilter As String = ""
            Dim strFieldName As String = ""
            Dim arrFilters() As String = _Filter.Split(Chr(127))
            Dim arrParams() As String
            Dim bolIsNullComp As Boolean = False

            Dim oUserFieldCondition As roUserFieldCondition = Nothing

            For Each str As String In arrFilters
                If str.Trim <> "" Then

                    oUserFieldCondition = New roUserFieldCondition

                    arrParams = str.Split("~")

                    ' Obtenemos el nombre del campo
                    strFieldName = arrParams(0).Split("|")(0)
                    If strFieldName.ToUpper.StartsWith("USR_") Then strFieldName = strFieldName.Substring(4)

                    oUserFieldCondition.UserField = New roUserField
                    oUserFieldCondition.UserField.FieldName = strFieldName
                    oUserFieldCondition.UserField.Type = Types.EmployeeField
                    Select Case arrParams(1)
                        Case "=" : oUserFieldCondition.Compare = CompareType.Equal
                        Case "<>" : oUserFieldCondition.Compare = CompareType.Distinct
                        Case ">" : oUserFieldCondition.Compare = CompareType.Major
                        Case ">=" : oUserFieldCondition.Compare = CompareType.MajorEqual
                        Case "<" : oUserFieldCondition.Compare = CompareType.Minor
                        Case "<=" : oUserFieldCondition.Compare = CompareType.MinorEqual
                        Case "*" : oUserFieldCondition.Compare = CompareType.StartWith
                        Case "**" : oUserFieldCondition.Compare = CompareType.Contains
                    End Select
                    oUserFieldCondition.ValueType = CompareValueType.DirectValue

                    'Desproteger valor (problemas al codificar/decodificar) le quitamos parentesis
                    If arrParams(2).StartsWith("(") And arrParams(2).EndsWith(")") Then
                        oUserFieldCondition.Value = arrParams(2).Substring(1, arrParams(2).Length - 2)
                    Else
                        oUserFieldCondition.Value = arrParams(2)
                    End If

                    strFilter = API.UserFieldServiceMethods.GetUserFieldConditionFilter(Nothing, oUserFieldCondition) & " " & arrParams(3) & " "

                    strRet &= strFilter

                End If
            Next

            If strRet.EndsWith("AND ") Or strRet.EndsWith("OR ") Then strRet = strRet.Substring(0, strRet.Length - 4)
            If strRet.EndsWith("OR ") Then strRet = strRet.Substring(0, strRet.Length - 3)

            If strRet <> "" Then strRet = "(" & strRet & ")"

        End If

        Return strRet

    End Function

#Region "QueryStringHelper"

    Public Shared Function GetURL_EmployeesByEmployee(ByVal IDEmployee As Integer, ByVal oPage As System.Web.UI.Page)
        Return oPage.Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Employees/Employees?idemployee=" + IDEmployee.ToString
    End Function

    Public Shared Function GetURL_EmployeesByGroup(ByVal IDGroup As Integer, ByVal oPage As System.Web.UI.Page)
        Return oPage.Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Employees/Employees?idgroup=" + IDGroup.ToString
    End Function

#End Region

End Class
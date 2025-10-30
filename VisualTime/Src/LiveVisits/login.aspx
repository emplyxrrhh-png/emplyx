<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="login.aspx.vb" Inherits="LiveVisits.login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />

    <title>Visualtime Live Visits</title>
    <link rel="shortcut icon" href="img/logovtl.png" />

    <link type="text/css" href="css/visits.min.css<%= "" & Me.MasterVersion %>" rel="stylesheet" />
    <link type="text/css" rel="stylesheet" href="css/src/normalize.css<%= "" & Me.MasterVersion %>" />
    <link type="text/css" rel="stylesheet" href="css/src/foundation-5.5.3.min.css<%= "" & Me.MasterVersion %>" />
    <link type="text/css" rel="stylesheet" href="css/src/font-awesome.min.css<%= "" & Me.MasterVersion %>" />
    <link type="text/css" rel="stylesheet" href="css/src/dataTables.foundation.css<%= "" & Me.MasterVersion %>" />
    <link type="text/css" rel="stylesheet" href="css/src/jquery.datetimepicker.css<%= "" & Me.MasterVersion %>" />

    <script type="text/javascript" src="js/login.min.js<%= "" & Me.MasterVersion %>"></script>
</head>
<body>
    <div class="loginfull">
        <div class="" id="backgroundcolumn" runat="server"></div>
        <div id="logincolumn" class="text-center">
            <div class="logincenter">

                <div class="login-form-space">&nbsp;</div>
                <div id="formlogin">
                    <div class="rbLogoInformation LogoLogin">
                        <div id="vtLogoVersionDiv" class="tbd_bar_textVersion" translate="no">
                            <img id="titleVT" src="img/Logo_VT5.png" style="height: 75%; width: 75%; border-width: 0px;">

                            <p align="right" style="padding-right: 27%;"></p>
                        </div>
                    </div>
                    <div class="formtext">Introduzca el usuario y contraseña</div>
                    <div>
                        <div class="loginfrmlabel">Cliente:</div>
                        <div class="loginfrminput">
                            <input type="text" id="companyId" autofocus />
                        </div>
                    </div>
                    <div>
                        <div class="loginfrmlabel">Usuario:</div>
                        <div class="loginfrminput">
                            <input type="text" id="login" autofocus />
                        </div>
                    </div>
                    <div>
                        <div class="loginfrmlabel">Contraseña:</div>
                        <div class="loginfrminput">
                            <input type="password" class="" id="password" />
                        </div>
                    </div>
                    <div id="login-response"></div>
                    <div id="login-buttons">
                        <div><a class="button tiny" onclick="login()" id="loginbtn">Iniciar</a> </div>
                    </div>
                </div>
                <div id="loginfooter">©2025 Visualtime</div>
            </div>
        </div>
    </div>
    <div id="changepassword" class="reveal-modal tiny" data-reveal>
        <div id="changepasswordform">
            <div class="formtext text-center" data-i18n="changepassword">Cambio de contraseña contraseña</div>
            <div>
                <div class="loginfrmlabel" data-i18n="currentpass">Contraseña actual:</div>
                <div class="loginfrmchginput">
                    <input type="password" id="currentpass" />
                </div>
            </div>
            <div>
                <div class="loginfrmlabel" data-i18n="newpass">Contraseña nueva:</div>
                <div class="loginfrmchginput">
                    <input type="password" class="" id="newpassword" />
                </div>
            </div>
            <div>
                <div class="loginfrmlabel" data-i18n="newpass2">Repetir contraseña:</div>
                <div class="loginfrmchginput">
                    <input type="password" class="" id="newpassword2" />
                </div>
            </div>
            <div id="changepassword-response"></div>
            <div id="changepassword-buttons">
                <div class="text-center"><a class="button small" onclick="changepassword()" id="changepasswordbtn" data-i18n="change">Modficar</a> </div>
            </div>
        </div>
    </div>
    <div id="tempkey" class="reveal-modal tiny" data-reveal>
        <div id="tempkeyform">
            <div class="formtext text-center" data-i18n="askcode">Solicitud de codigo</div>
            <div>
                <div class="loginfrmlabel" data-i18n="code">Codigo:</div>
                <div class="loginfrmchginput">
                    <input type="text" id="code" />
                </div>
            </div>
            <div id="changepassword-response"></div>
            <div id="changepassword-buttons">
                <div class="text-center"><a class="button small" onclick="validationcode()" id="A1" data-i18n="calidate">Validar</a> </div>
            </div>
        </div>
    </div>
</body>
</html>
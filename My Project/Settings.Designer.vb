﻿'------------------------------------------------------------------------------
' <auto-generated>
'     O código foi gerado por uma ferramenta.
'     Versão de Tempo de Execução:4.0.30319.42000
'
'     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
'     o código for gerado novamente.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace My
    
    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.14.0.0"),  _
     Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase
        
        Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
        
#Region "Funcionalidade de salvamento automático do My.Settings"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(sender As Global.System.Object, e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
        
        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
                Return defaultInstance
            End Get
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property apiCMCKey() As String
            Get
                Return CType(Me("apiCMCKey"),String)
            End Get
            Set
                Me("apiCMCKey") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest")>  _
        Public Property apiUrlHistorical() As String
            Get
                Return CType(Me("apiUrlHistorical"),String)
            End Get
            Set
                Me("apiUrlHistorical") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest")>  _
        Public Property apiUrl() As String
            Get
                Return CType(Me("apiUrl"),String)
            End Get
            Set
                Me("apiUrl") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("https://sandbox-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest")>  _
        Public Property apiUrlSandbox() As String
            Get
                Return CType(Me("apiUrlSandbox"),String)
            End Get
            Set
                Me("apiUrlSandbox") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property lastView() As String
            Get
                Return CType(Me("lastView"),String)
            End Get
            Set
                Me("lastView") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property activeAPI() As String
            Get
                Return CType(Me("activeAPI"),String)
            End Get
            Set
                Me("activeAPI") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property lastUpdate() As String
            Get
                Return CType(Me("lastUpdate"),String)
            End Get
            Set
                Me("lastUpdate") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property JSONBinURL() As String
            Get
                Return CType(Me("JSONBinURL"),String)
            End Get
            Set
                Me("JSONBinURL") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property JSONBinMasterKey() As String
            Get
                Return CType(Me("JSONBinMasterKey"),String)
            End Get
            Set
                Me("JSONBinMasterKey") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property JSONBinID() As String
            Get
                Return CType(Me("JSONBinID"),String)
            End Get
            Set
                Me("JSONBinID") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("j2aE1GUK0v2IRBDwcDIXpc6KsO6zVwGLlKL6XLAPrZKQFj1DxfvNb52YCrfZKpLM")>  _
        Public Property BinanceAPIKey() As String
            Get
                Return CType(Me("BinanceAPIKey"),String)
            End Get
            Set
                Me("BinanceAPIKey") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("BCXEC3pcnqYfZl7Fi7BkEGSIkla2JEZTA5HHpduAIZJ8iZkm9XSBoetawTfyU6Ne")>  _
        Public Property BinanceSecretKey() As String
            Get
                Return CType(Me("BinanceSecretKey"),String)
            End Get
            Set
                Me("BinanceSecretKey") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("98db4f63584bb557c0e5d162324edf43")>  _
        Public Property GateAPIKey() As String
            Get
                Return CType(Me("GateAPIKey"),String)
            End Get
            Set
                Me("GateAPIKey") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("6c7a7c9aa890301a5078d09adfeb2d5e6eef4c81a7da556232d5b6e8d81685ec")>  _
        Public Property GateSecretKey() As String
            Get
                Return CType(Me("GateSecretKey"),String)
            End Get
            Set
                Me("GateSecretKey") = value
            End Set
        End Property
    End Class
End Namespace

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.PortfolioCripto.My.MySettings
            Get
                Return Global.PortfolioCripto.My.MySettings.Default
            End Get
        End Property
    End Module
End Namespace

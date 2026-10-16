Imports System.ComponentModel
Imports System.Runtime.InteropServices

''' <summary>
''' Acessa as credenciais das exchanges pelo Gerenciador de Credenciais do Windows.
''' </summary>
Public NotInheritable Class ExchangeCredentials
    Private Sub New()
    End Sub

    Private Const CredentialTypeGeneric As UInteger = 1UI
    Private Const CredentialPersistLocalMachine As UInteger = 2UI
    Private Const BinanceApiTarget As String = "PortfolioCripto/Binance/APIKey"
    Private Const BinanceSecretTarget As String = "PortfolioCripto/Binance/SecretKey"
    Private Const GateApiTarget As String = "PortfolioCripto/GateIO/APIKey"
    Private Const GateSecretTarget As String = "PortfolioCripto/GateIO/SecretKey"

    Private Shared ReadOnly SyncRoot As New Object()
    Private Shared _initialized As Boolean
    Private Shared _binanceApiKey As String = String.Empty
    Private Shared _binanceSecretKey As String = String.Empty
    Private Shared _gateApiKey As String = String.Empty
    Private Shared _gateSecretKey As String = String.Empty

    Public Shared ReadOnly Property BinanceAPIKey As String
        Get
            EnsureInitialized()
            Return _binanceApiKey
        End Get
    End Property

    Public Shared ReadOnly Property BinanceSecretKey As String
        Get
            EnsureInitialized()
            Return _binanceSecretKey
        End Get
    End Property

    Public Shared ReadOnly Property GateAPIKey As String
        Get
            EnsureInitialized()
            Return _gateApiKey
        End Get
    End Property

    Public Shared ReadOnly Property GateSecretKey As String
        Get
            EnsureInitialized()
            Return _gateSecretKey
        End Get
    End Property

    Public Shared Sub SaveBinanceCredentials(apiKey As String, secretKey As String)
        SaveExchangeCredentials(BinanceApiTarget, BinanceSecretTarget, apiKey, secretKey)
        _binanceApiKey = apiKey.Trim()
        _binanceSecretKey = secretKey.Trim()
    End Sub

    Public Shared Sub SaveGateCredentials(apiKey As String, secretKey As String)
        SaveExchangeCredentials(GateApiTarget, GateSecretTarget, apiKey, secretKey)
        _gateApiKey = apiKey.Trim()
        _gateSecretKey = secretKey.Trim()
    End Sub

    Private Shared Sub EnsureInitialized()
        If _initialized Then Return

        SyncLock SyncRoot
            If _initialized Then Return
            MigrateLegacySettings()
            _binanceApiKey = ReadCredential(BinanceApiTarget)
            _binanceSecretKey = ReadCredential(BinanceSecretTarget)
            _gateApiKey = ReadCredential(GateApiTarget)
            _gateSecretKey = ReadCredential(GateSecretTarget)
            _initialized = True
        End SyncLock
    End Sub

    Private Shared Sub MigrateLegacySettings()
        Dim migrated =
            MigrateSetting(BinanceApiTarget, My.Settings.BinanceAPIKey) AndAlso
            MigrateSetting(BinanceSecretTarget, My.Settings.BinanceSecretKey) AndAlso
            MigrateSetting(GateApiTarget, My.Settings.GateAPIKey) AndAlso
            MigrateSetting(GateSecretTarget, My.Settings.GateSecretKey)

        If Not migrated Then Return

        Try
            My.Settings.BinanceAPIKey = String.Empty
            My.Settings.BinanceSecretKey = String.Empty
            My.Settings.GateAPIKey = String.Empty
            My.Settings.GateSecretKey = String.Empty
            My.Settings.Save()
        Catch ex As Exception
            Debug.WriteLine("Não foi possível limpar as credenciais legadas: " & ex.Message)
        End Try
    End Sub

    Private Shared Function MigrateSetting(target As String, legacyValue As String) As Boolean
        If Not String.IsNullOrWhiteSpace(ReadCredential(target)) Then Return True
        If String.IsNullOrWhiteSpace(legacyValue) Then Return True

        Try
            WriteCredential(target, legacyValue.Trim())
            Return True
        Catch ex As Exception
            Debug.WriteLine("Não foi possível migrar uma credencial para o Windows: " & ex.Message)
            Return False
        End Try
    End Function

    Private Shared Sub SaveExchangeCredentials(
        apiTarget As String,
        secretTarget As String,
        apiKey As String,
        secretKey As String)

        If String.IsNullOrWhiteSpace(apiKey) OrElse String.IsNullOrWhiteSpace(secretKey) Then
            Throw New ArgumentException("A chave API e a chave secreta são obrigatórias.")
        End If

        EnsureInitialized()

        SyncLock SyncRoot
            WriteCredential(apiTarget, apiKey.Trim())
            WriteCredential(secretTarget, secretKey.Trim())

            Try
                My.Settings.BinanceAPIKey = String.Empty
                My.Settings.BinanceSecretKey = String.Empty
                My.Settings.GateAPIKey = String.Empty
                My.Settings.GateSecretKey = String.Empty
                My.Settings.Save()
            Catch ex As Exception
                Debug.WriteLine("Não foi possível limpar as configurações legadas: " & ex.Message)
            End Try
        End SyncLock
    End Sub

    Private Shared Function ReadCredential(target As String) As String
        Dim credentialPointer As IntPtr = IntPtr.Zero
        If Not CredRead(target, CredentialTypeGeneric, 0UI, credentialPointer) Then Return String.Empty

        Try
            Dim credential = Marshal.PtrToStructure(Of NativeCredential)(credentialPointer)
            If credential.CredentialBlob = IntPtr.Zero OrElse credential.CredentialBlobSize = 0UI Then
                Return String.Empty
            End If

            Dim byteCount = CInt(credential.CredentialBlobSize)
            Dim bytes(byteCount - 1) As Byte
            Marshal.Copy(credential.CredentialBlob, bytes, 0, byteCount)
            Return Text.Encoding.Unicode.GetString(bytes).TrimEnd(ChrW(0))
        Finally
            CredFree(credentialPointer)
        End Try
    End Function

    Private Shared Sub WriteCredential(target As String, value As String)
        Dim targetPointer As IntPtr = IntPtr.Zero
        Dim userPointer As IntPtr = IntPtr.Zero
        Dim blobPointer As IntPtr = IntPtr.Zero

        Try
            Dim bytes = Text.Encoding.Unicode.GetBytes(value)
            targetPointer = Marshal.StringToCoTaskMemUni(target)
            userPointer = Marshal.StringToCoTaskMemUni(Environment.UserName)
            blobPointer = Marshal.AllocCoTaskMem(bytes.Length)
            Marshal.Copy(bytes, 0, blobPointer, bytes.Length)

            Dim credential As New NativeCredential With {
                .Type = CredentialTypeGeneric,
                .TargetName = targetPointer,
                .CredentialBlobSize = CUInt(bytes.Length),
                .CredentialBlob = blobPointer,
                .Persist = CredentialPersistLocalMachine,
                .UserName = userPointer
            }

            If Not CredWrite(credential, 0UI) Then
                Throw New Win32Exception(Marshal.GetLastWin32Error())
            End If
        Finally
            If blobPointer <> IntPtr.Zero Then Marshal.FreeCoTaskMem(blobPointer)
            If userPointer <> IntPtr.Zero Then Marshal.FreeCoTaskMem(userPointer)
            If targetPointer <> IntPtr.Zero Then Marshal.FreeCoTaskMem(targetPointer)
        End Try
    End Sub

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure NativeCredential
        Public Flags As UInteger
        Public Type As UInteger
        Public TargetName As IntPtr
        Public Comment As IntPtr
        Public LastWritten As Long
        Public CredentialBlobSize As UInteger
        Public CredentialBlob As IntPtr
        Public Persist As UInteger
        Public AttributeCount As UInteger
        Public Attributes As IntPtr
        Public TargetAlias As IntPtr
        Public UserName As IntPtr
    End Structure

    <DllImport("advapi32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
    Private Shared Function CredRead(targetName As String, type As UInteger, flags As UInteger, ByRef credential As IntPtr) As Boolean
    End Function

    <DllImport("advapi32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
    Private Shared Function CredWrite(ByRef credential As NativeCredential, flags As UInteger) As Boolean
    End Function

    <DllImport("advapi32.dll", SetLastError:=True)>
    Private Shared Function CredFree(credential As IntPtr) As Boolean
    End Function
End Class

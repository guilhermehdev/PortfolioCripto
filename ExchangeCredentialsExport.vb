Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.Json

Public NotInheritable Class ExchangeCredentialsExportService

    Private Const ExportVersion As Integer = 1
    Private Const KdfIterations As Integer = 210000
    Private Const KeySize As Integer = 32
    Private Const NonceSize As Integer = 12
    Private Const TagSize As Integer = 16

    Private Sub New()
    End Sub

    Public Shared Sub ExportEncrypted(filePath As String, password As String)
        If String.IsNullOrWhiteSpace(filePath) Then
            Throw New ArgumentException("Informe o caminho do arquivo de exportação.", NameOf(filePath))
        End If

        If String.IsNullOrWhiteSpace(password) Then
            Throw New ArgumentException("A senha de exportação é obrigatória.", NameOf(password))
        End If

        Dim payload = JsonSerializer.Serialize(New ExchangeCredentialsPayload With {
            .BinanceAPIKey = ExchangeCredentials.BinanceAPIKey,
            .BinanceSecretKey = ExchangeCredentials.BinanceSecretKey,
            .GateAPIKey = ExchangeCredentials.GateAPIKey,
            .GateSecretKey = ExchangeCredentials.GateSecretKey
        })

        Dim salt = RandomNumberGenerator.GetBytes(32)
        Dim nonce = RandomNumberGenerator.GetBytes(NonceSize)
        Dim plaintext = Encoding.UTF8.GetBytes(payload)
        Dim ciphertext(plaintext.Length - 1) As Byte
        Dim tag(TagSize - 1) As Byte

        Dim key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            KdfIterations,
            HashAlgorithmName.SHA256,
            KeySize)

        Using aes As New AesGcm(key, TagSize)
            aes.Encrypt(nonce, plaintext, ciphertext, tag)
        End Using

        Dim envelope = New ExchangeCredentialsExportEnvelope With {
            .Version = ExportVersion,
            .Kdf = "PBKDF2-SHA256",
            .Iterations = KdfIterations,
            .Salt = Convert.ToBase64String(salt),
            .Nonce = Convert.ToBase64String(nonce),
            .Tag = Convert.ToBase64String(tag),
            .Ciphertext = Convert.ToBase64String(ciphertext)
        }

        Dim options As New JsonSerializerOptions With {.WriteIndented = True}
        File.WriteAllText(filePath, JsonSerializer.Serialize(envelope, options), New UTF8Encoding(False))
    End Sub

    Public Shared Function ImportEncrypted(filePath As String, password As String) As ExchangeCredentialsImportResult
        If String.IsNullOrWhiteSpace(filePath) OrElse Not File.Exists(filePath) Then
            Throw New FileNotFoundException("O arquivo de credenciais não foi encontrado.", filePath)
        End If

        If String.IsNullOrWhiteSpace(password) Then
            Throw New ArgumentException("A senha de importação é obrigatória.", NameOf(password))
        End If

        Dim envelope As ExchangeCredentialsExportEnvelope
        Try
            envelope = JsonSerializer.Deserialize(Of ExchangeCredentialsExportEnvelope)(File.ReadAllText(filePath))
        Catch ex As Exception
            Throw New InvalidDataException("O arquivo de credenciais é inválido.", ex)
        End Try

        If envelope Is Nothing OrElse
           envelope.Version <> ExportVersion OrElse
           Not String.Equals(envelope.Kdf, "PBKDF2-SHA256", StringComparison.Ordinal) OrElse
           envelope.Iterations <> KdfIterations Then
            Throw New InvalidDataException("O formato do arquivo de credenciais não é compatível.")
        End If

        Dim salt = DecodeBase64(envelope.Salt, "salt")
        Dim nonce = DecodeBase64(envelope.Nonce, "nonce")
        Dim tag = DecodeBase64(envelope.Tag, "tag")
        Dim ciphertext = DecodeBase64(envelope.Ciphertext, "conteúdo")
        If salt.Length <> 32 OrElse nonce.Length <> NonceSize OrElse tag.Length <> TagSize OrElse ciphertext.Length = 0 Then
            Throw New InvalidDataException("O arquivo de credenciais está incompleto ou corrompido.")
        End If

        Dim key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            envelope.Iterations,
            HashAlgorithmName.SHA256,
            KeySize)
        Dim plaintext(ciphertext.Length - 1) As Byte

        Try
            Using aes As New AesGcm(key, TagSize)
                aes.Decrypt(nonce, ciphertext, tag, plaintext)
            End Using
        Catch ex As CryptographicException
            Throw New CryptographicException("Não foi possível abrir o arquivo. Verifique a senha.", ex)
        Finally
            CryptographicOperations.ZeroMemory(key)
        End Try

        Dim payload As ExchangeCredentialsPayload
        Try
            payload = JsonSerializer.Deserialize(Of ExchangeCredentialsPayload)(Encoding.UTF8.GetString(plaintext))
        Catch ex As Exception
            Throw New InvalidDataException("O conteúdo descriptografado é inválido.", ex)
        Finally
            CryptographicOperations.ZeroMemory(plaintext)
        End Try

        If payload Is Nothing Then Throw New InvalidDataException("O conteúdo descriptografado está vazio.")
        ValidateCredentialPair(payload.BinanceAPIKey, payload.BinanceSecretKey, "Binance")
        ValidateCredentialPair(payload.GateAPIKey, payload.GateSecretKey, "Gate.io")

        Return New ExchangeCredentialsImportResult(
            payload.BinanceAPIKey,
            payload.BinanceSecretKey,
            payload.GateAPIKey,
            payload.GateSecretKey)
    End Function

    Private Shared Function DecodeBase64(value As String, fieldName As String) As Byte()
        Try
            If String.IsNullOrWhiteSpace(value) Then Throw New FormatException()
            Return Convert.FromBase64String(value)
        Catch ex As FormatException
            Throw New InvalidDataException($"O campo {fieldName} do arquivo de credenciais é inválido.", ex)
        End Try
    End Function

    Private Shared Sub ValidateCredentialPair(apiKey As String, secretKey As String, exchangeName As String)
        Dim hasApi = Not String.IsNullOrWhiteSpace(apiKey)
        Dim hasSecret = Not String.IsNullOrWhiteSpace(secretKey)
        If hasApi Xor hasSecret Then
            Throw New InvalidDataException($"O arquivo possui apenas uma das chaves da {exchangeName}.")
        End If
    End Sub

    Private NotInheritable Class ExchangeCredentialsPayload
        Public Property BinanceAPIKey As String = String.Empty
        Public Property BinanceSecretKey As String = String.Empty
        Public Property GateAPIKey As String = String.Empty
        Public Property GateSecretKey As String = String.Empty
    End Class

    Private NotInheritable Class ExchangeCredentialsExportEnvelope
        Public Property Version As Integer
        Public Property Kdf As String = String.Empty
        Public Property Iterations As Integer
        Public Property Salt As String = String.Empty
        Public Property Nonce As String = String.Empty
        Public Property Tag As String = String.Empty
        Public Property Ciphertext As String = String.Empty
    End Class
End Class

Public NotInheritable Class ExchangeCredentialsImportResult
    Public Sub New(binanceApiKey As String, binanceSecretKey As String, gateApiKey As String, gateSecretKey As String)
        Me.BinanceAPIKey = binanceApiKey
        Me.BinanceSecretKey = binanceSecretKey
        Me.GateAPIKey = gateApiKey
        Me.GateSecretKey = gateSecretKey
    End Sub

    Public ReadOnly Property BinanceAPIKey As String
    Public ReadOnly Property BinanceSecretKey As String
    Public ReadOnly Property GateAPIKey As String
    Public ReadOnly Property GateSecretKey As String
End Class

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

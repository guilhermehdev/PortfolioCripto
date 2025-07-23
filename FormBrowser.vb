
Imports Microsoft.Web.WebView2.WinForms

Public Class FormBrowser
    Dim webview As New WebView2()
    Dim b As New Binance
    Dim g As New Gateio
    Private symbol As String

    Public Sub New(cripto As String)
        InitializeComponent()
        symbol = cripto
    End Sub

    Public Sub loadChart(symbol As String, source As String)
        Dim html As String = "
        <html>
        <head>
          <meta charset='utf-8'>
          <script type='text/javascript' src='https://s3.tradingview.com/tv.js'></script>
        </head>
        <body style='margin:0;padding:0;'>
          <div id='tradingview_widget'></div>
          <script type='text/javascript'>
            new TradingView.widget({
              'container_id': 'tradingview_widget',            
              'symbol': '" & source & ":" & symbol & "USDT',
              'interval': '240',
              'timezone': 'Etc/UTC',
              'theme': 'dark',
              'style': '1',
              'locale': 'br',
              'hide_top_toolbar': false,
              'hide_legend': false,
              'hide_volume': false,
              'allow_symbol_change': true,
              'save_image': false,
              'withdateranges': false,
              'details': false,
              'studies': [
                             {
                                id: 'RSI@tv-basicstudies',
                                inputs: {         
                                  ma: 14 // Adiciona uma Média Móvel de 14 períodos ao RSI
                                }
                              }
                          ],
              'autosize': true
            });
          </script>
        </body>
        </html>"
        webview.NavigateToString(html)
    End Sub
    Private Async Sub FormBrowser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = symbol
        webview.Dock = DockStyle.Fill
        Me.Controls.Add(webview)
        Await webview.EnsureCoreWebView2Async()
        ' webview.CoreWebView2.Navigate("https://br.tradingview.com/chart/?symbol=BINANCE:SOLUSDT&theme=dark")
        If Await b.ParExisteNaBinance(symbol) Then
            loadChart(symbol, "BINANCE")
        ElseIf Await g.ParExisteNaGateIo(symbol) Then
            loadChart(symbol, "GATEIO")
        Else
            loadChart(symbol, "BYBIT")
        End If
    End Sub

End Class
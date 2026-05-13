using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;
using VIC_EVO_Dashboard.Resources.Strings;
using FluentModbus;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.ApplicationModel;

namespace VIC_EVO_Dashboard;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string pressureUnit;

    [ObservableProperty]
    private string tempUnit;

    [ObservableProperty]
    private string flowValue = "0";

    [ObservableProperty]
    private string positionValue = "0";

    [ObservableProperty]
    private string p1Value = "0";

    [ObservableProperty]
    private string p2Value = "0";

    [ObservableProperty]
    private string p3Value = "0";

    [ObservableProperty]
    private string p4Value = "0";

    [ObservableProperty]
    private string fuelTempValue = "0";

    private int FuelTempValueSign = 0;

    [ObservableProperty]
    private Color error1Led = Colors.Gray;

    [ObservableProperty]
    private Color error2Led = Colors.Gray;

    [ObservableProperty]
    private Color error3Led = Colors.Gray;

    [ObservableProperty]
    private Color error4Led = Colors.Gray;

    [ObservableProperty]
    private Color error5Led = Colors.Gray;

    [ObservableProperty]
    private Color error6Led = Colors.Gray;

    [ObservableProperty]
    private Color error7Led = Colors.Gray;

    [ObservableProperty]
    private Color error8Led = Colors.Gray;

    [ObservableProperty]
    private Color error9Led = Colors.Gray;

    [ObservableProperty]
    private Color error10Led = Colors.Gray;

    [ObservableProperty]
    private Color error11Led = Colors.Gray;


    private CancellationTokenSource? _modbusCts;
    private Task? _modbusTask;

    public LocalizedStrings Localization => new();

    public MainPageViewModel()
    {
        PressureUnit = Preferences.Get("SelectedPressureUnit", "BAR");
        TempUnit = "°C";
    }

    public void RefreshUI()
    {
        AppResources.Culture = CultureInfo.CurrentUICulture;

        OnPropertyChanged(nameof(Localization));

        OnPropertyChanged(string.Empty);
    }

    public void StartModbusPolling(string ip = "127.0.0.1", int port = 502, int pollingIntervalMs = 1000)
    {
        if (_modbusCts != null) return;

        _modbusCts = new CancellationTokenSource();
        var token = _modbusCts.Token;

        _modbusTask = Task.Run(async () =>
        {
            var client = new ModbusTcpClient();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (!client.IsConnected)
                    {
                        try
                        {
                            client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Modbus connect failed: {ex.Message}");
                            await Task.Delay(2000, token);
                            continue;
                        }
                    }

                    var regs = await client.ReadHoldingRegistersAsync<short>(0, 0, 15);
                    var arr = regs.ToArray();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (arr.Length > 0) FlowValue = arr[0].ToString();
                        if (arr.Length > 11) PositionValue = arr[11].ToString();
                        if (arr.Length > 1) P1Value = arr[1].ToString();
                        if (arr.Length > 3) P2Value = arr[3].ToString();
                        if (arr.Length > 5) P3Value = arr[5].ToString();
                        if (arr.Length > 7) P4Value = arr[7].ToString();
                        if (arr.Length > 9) FuelTempValue = arr[9].ToString();
                        if (arr.Length > 10) FuelTempValueSign = arr[10];

                        if (FuelTempValueSign != 0)
                        {
                            fuelTempValue = $"{FuelTempValue}";
                        }


                        if (arr.Length > 15)
                        {
                            ushort errorWord = (ushort)arr[15];

                            Color GetLedForBit(int bitIndex) => ((errorWord & (1 << bitIndex)) != 0) ? Colors.Green : Colors.Gray;

                            Error1Led = GetLedForBit(8);
                            Error2Led = GetLedForBit(10);
                            Error3Led = GetLedForBit(12);
                            Error4Led = GetLedForBit(9);
                            Error5Led = GetLedForBit(0);
                            Error6Led = GetLedForBit(1);
                            Error7Led = GetLedForBit(2);
                            Error8Led = GetLedForBit(15);
                            Error9Led = GetLedForBit(11);
                            Error10Led = GetLedForBit(13);
                            Error11Led = GetLedForBit(14);
                        }
                    });
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Modbus polling error: {ex.Message}");
                }

                try { await Task.Delay(pollingIntervalMs, token); } catch { break; }
            }

            try { client.Dispose(); } catch { }
        }, token);
    }

    public void StopModbusPolling()
    {
        if (_modbusCts == null) return;
        try
        {
            _modbusCts.Cancel();
        }
        catch { }
        _modbusCts = null;
    }
}
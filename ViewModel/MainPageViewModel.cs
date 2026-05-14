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
                            client.Connect(IPAddress.Parse(ip), ModbusEndianness.BigEndian);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Modbus connect failed: {ex.Message}");
                            await Task.Delay(2000, token);
                            continue;
                        }
                    }

                    var regs = await client.ReadHoldingRegistersAsync<ushort>(0, 0, 15);
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
                            FuelTempValue = $"-{FuelTempValue}";
                        }

                        Debug.WriteLine(FuelTempValueSign);
                        Debug.WriteLine(fuelTempValue);


                        if (arr.Length > 13)
                        {
                            ushort errorWord = (ushort)arr[13];

                            Color GetLedForBit(int bitIndex) => ((errorWord & (1 << bitIndex)) != 0) ? Colors.Green : Colors.Red;

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

                            Debug.WriteLine(errorWord);
                            Debug.WriteLine($"LEDs: {Error1Led}, {Error2Led}, {Error3Led}, {Error4Led}, {Error5Led}, {Error6Led}, {Error7Led}, {Error8Led}, {Error9Led}, {Error10Led}, {Error11Led}");
                        }

                        ChangeUnit(Preferences.Get("SelectedUnitIndex", 0) == 0 ? "SI" : "IMPERIAL");

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

    public void ChangeUnit(string unit)
    {
        //Position valve in percentage
        PositionValue = (float.Parse(PositionValue) / 100).ToString("N2");

        if (unit == "SI")
        {
            //SI Units (BAR, °C)
            P1Value = (float.Parse(P1Value) / 1000).ToString("N2");
            P2Value = (float.Parse(P2Value) / 1000).ToString("N2");
            P3Value = (float.Parse(P3Value) / 1000).ToString("N2");
            P4Value = (float.Parse(P4Value) / 1000).ToString("N2");

            FuelTempValue = (float.Parse(FuelTempValue) / 100).ToString();

        }

        if (unit == "IMPERIAL")
        {
            //Imperial Units (PSI, °F)
            P1Value = ((float.Parse(P1Value) / 1000) * 14.5038).ToString("N2");
            P2Value = ((float.Parse(P2Value) / 1000) * 14.5038).ToString("N2");
            P3Value = ((float.Parse(P3Value) / 1000) * 14.5038).ToString("N2");
            P4Value = ((float.Parse(P4Value) / 1000) * 14.5038).ToString("N2");
            FuelTempValue = ((float.Parse(FuelTempValue) / 100) * 1.8 + 32).ToString("N2");
        }
    }
}
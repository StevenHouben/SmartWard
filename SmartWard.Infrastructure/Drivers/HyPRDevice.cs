using SmartWard.Devices;
using SmartWard.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;

using LibUsbDotNet;
using LibUsbDotNet.DeviceNotify;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;

namespace SmartWard.Infrastructure.Driver
{
    public delegate void RFIDDataReceivedHandler(object sender, RFDIDataReceivedEventArgs e);
    public class HyPRDevice:Device
    {
        public event RFIDDataReceivedHandler RFIDDataReceived = null;
        public event EventHandler RFIDResetReceived = null;

        public string Port { get; private set; }
        public string CurrentRFID { get; private set; }

        private const string handShakeCommand = "A";
        private const string handShakeReply = "B";
        private const int baudRate = 9600;
        private const int readDelay = 100; //ms
        private const int readTimeOut = 200; //ms

        private SafeSerialPort serialPort = null;
        private string output;

        private UsbDevice MyUsbDevice;
        private UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(0x2341, 0x0001);
        private IDeviceNotifier UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();

        public HyPRDevice()
        {
            Connect();
            UsbDeviceNotifier.OnDeviceNotify += UsbDeviceNotifier_OnDeviceNotify;
        }

        private void Connect()
        {

            var port = FindDevice();
            if (port != null)
            {
                ConnectToDevice(port);
            }
            else
                Console.WriteLine("No HyPR Device found");
        }
        private void ResetConnection()
        {
            try
            {
                if(serialPort !=null)
                    serialPort.Write("Any value");
            }
            catch (IOException)
            {
                serialPort.Dispose();
                serialPort.Close();
            }
        }
        void UsbDeviceNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            if (e.Object.ToString().Split('\n')[1].Contains("0x2341"))
            {
                if (e.EventType == EventType.DeviceArrival)
                {
                    Connect();
                }
                else if (e.EventType == EventType.DeviceRemoveComplete)
                {
                    ResetConnection();
                }
            }
        }

        private string FindDevice()
        {
            var ports = SerialPort.GetPortNames();
            foreach (var portname in ports)
            {
                Console.WriteLine("Attempt to connect to {0}", portname);
                SerialPort sp = new SerialPort(portname, baudRate);
                try
                {
                    sp.ReadTimeout = readTimeOut;
                    sp.Open();
                    sp.Write(handShakeCommand);
                    Thread.Sleep(readDelay);

                    string received = sp.ReadExisting();

                    if (received == handShakeReply)
                        return portname;
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                finally
                {
                    sp.Close();
                }
            }
            return null;
        }
        public HyPRDevice(string port)
        {
            ConnectToDevice(port);
        }

        private void ConnectToDevice(string port)
        {
            this.Port = port;
            ConnectToHyPRDevice(port);
        }
        
        ~HyPRDevice()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    serialPort.Dispose();
                }
            }
        }

        private void ConnectToHyPRDevice(string portname)
        {
            try
            {
                serialPort = null;
                serialPort = new SafeSerialPort(portname, baudRate);
                serialPort.DataReceived += serialPort_DataReceived;
                serialPort.Open();
                Console.WriteLine("Found HyPR device at: " + portname);
            }
            catch (Exception ex)
            {
                Console.WriteLine("NOT connected to: " + portname);
                Console.WriteLine(ex.ToString());
            }
        }
        public void UpdateColor(RGB color)
        {
            WriteToDevice(color.ToString());
        }
        private void WriteToDevice(string msg)
        {
            try
            {
                if(serialPort != null)
                    if(serialPort.IsOpen)
                        serialPort.Write(msg);
            }
            catch (IOException)
            {
                ResetConnection();
                //var success  = PortHelper.TryResetPortByName(Port);
                //Thread.Sleep(10000);
                //ConnectToDevice(Port);
            }
        }


        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            output += serialPort.ReadExisting();

            if (output.EndsWith("#"))
            {
                if (!output.Contains("RESET#"))
                {
                    CurrentRFID = output;
                    OnRFIDDataReceived(new RFDIDataReceivedEventArgs(output));
                }
                else
                    OnRFIDResetReceived(new EventArgs());
               // Console.WriteLine("Received:\t" + output);
                output = "";
            }
        }
        protected void OnRFIDDataReceived(RFDIDataReceivedEventArgs e)
        {
            if (RFIDDataReceived != null)
                RFIDDataReceived(this, e);
        }
        protected void OnRFIDResetReceived(EventArgs e)
        {
            if (RFIDResetReceived != null)
                RFIDResetReceived(this, e);
        }
    }
    public class RFDIDataReceivedEventArgs:EventArgs
    {
        public string RFID{get;set;}
        public RFDIDataReceivedEventArgs(string rfid)
        {
            RFID=rfid;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Location.Sonitor
{
    public class SonitorTracker:ITracker
    {
        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                RunTCPClient();
            });
        }

        public void Stop()
        {
            Running = false;
        }

        public bool Running { get; private set; }
        private void RunTCPClient()
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(IPAddress.Parse(
                    global::SmartWard.Infrastructure.Properties.Settings.Default.LocationTracker_IP), 
                    global::SmartWard.Infrastructure.Properties.Settings.Default.LocationTracker_Port);

                Running = true;
                var reader = new StreamReader(client.GetStream(), Encoding.ASCII);

                var message = new List<string>();
                while (Running)
                {
                    var line = reader.ReadLine();
                    if (line == "")
                    {
                        ParseRawMessage(message);
                        message.Clear();
                    }
                    else
                        message.Add(line);
                }

                client.Close();
                Console.WriteLine("Location Tracker closing");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
        }
        private void ParseRawMessage(List<string> msg)
        {
            var head = msg[0];

            switch (SonitorConverter.DetermineMessage(head))
            {
                case SonitorMessages.DETECTION:
                    HandleDetectionMessage(msg);
                    break;
                case SonitorMessages.DETECTORS:
                    HandleDetectorsMessage(msg);
                    break;
                case SonitorMessages.DETECTORSTATUS:
                    HandleDetectorStatusMessage(msg);
                    break;
                case SonitorMessages.MAPS:
                    HandleMapsMessage(msg);
                    break;
                case SonitorMessages.PROTOCOLVERSION:
                    HandleProtocolMessage(msg);
                    break;
                case SonitorMessages.TAGS:
                    HandleTagsMessage(msg);
                    break;
                default:
                    break;
            }
        }

        public event SonitorMessageReceivedHandler TagsReceived = delegate { };
        private void HandleTagsMessage(List<string> msg)
        {
            var message = new TagsMessage();
            for (int i = 1; i < msg.Count; i++)
            {
                var rawDetection = msg[i].Split(',');
                message.Tags.Add(
                    new Tag()
                    {
                        Id = rawDetection[0],
                        Name = rawDetection[1],
                        ImageUrl = rawDetection[2]
                    });

            }
            TagsReceived(this, new SonitorEventArgs(message));
        }

        public event SonitorMessageReceivedHandler MapsReceived = delegate { };
        private void HandleMapsMessage(List<string> msg)
        {
            var message = new MapsMessage();
            for (int i = 1; i < msg.Count; i++)
            {
                var rawDetection = msg[i].Split(',');
                message.Maps.Add(
                    new Map()
                    {
                        FloorNumber = Convert.ToInt16(rawDetection[0]),
                        Name = rawDetection[1],
                        ImageUrl = rawDetection[2]
                    });

            }
            MapsReceived(this, new SonitorEventArgs(message));
        }

        public event SonitorMessageReceivedHandler DetectorStatusReceived = delegate { };
        private void HandleDetectorStatusMessage(List<string> msg)
        {
            var message = new DetectorStatusMessage();
            for (int i = 1; i < msg.Count; i++)
            {
                var rawDetection = msg[i].Split(',');

                message.DetectorStates.Add(
                    new DetectorStatus()
                    {
                        HostName = rawDetection[0],
                        Channel = Convert.ToInt16(rawDetection[1]),
                        Online = Convert.ToInt16(rawDetection[2]) == 1
                    });

            }
            DetectorStatusReceived(this, new SonitorEventArgs(message));
        }

        public event SonitorMessageReceivedHandler DetectorsReceived = delegate { };
        private void HandleDetectorsMessage(List<string> msg)
        {
            var message = new DetectorsMessage();
            for (int i = 1; i < msg.Count; i++)
            {
                var rawDetection = msg[i].Split(',');
                message.Detectors.Add(
                    new Detector()
                    {
                        HostName = rawDetection[0],
                        Channel = Convert.ToInt16(rawDetection[1]),
                        Name = rawDetection[2],
                        Location = new GenericLocation<float>(float.Parse(rawDetection[3], CultureInfo.InvariantCulture.NumberFormat),
                                                                    float.Parse(rawDetection[4], CultureInfo.InvariantCulture.NumberFormat)),
                        FloorPlan = Convert.ToInt16(rawDetection[5]),
                        Radius = float.Parse((rawDetection[6]), CultureInfo.InvariantCulture.NumberFormat),

                    });
            }
            DetectorsReceived(this, new SonitorEventArgs(message));
        }

        public event SonitorMessageReceivedHandler ProtocolReceived = delegate { };
        private void HandleProtocolMessage(List<string> msg)
        {
            var message = new ProtocolVersionMessage(msg[1]);
            ProtocolReceived(this, new SonitorEventArgs(message));
        }

        public event SonitorMessageReceivedHandler DetectionsReceived = delegate { };
        private void HandleDetectionMessage(List<string> msg)
        {
            var message = new DetectionsMessage();

            for (int i = 1; i < msg.Count; i++)
            {
                var rawDetection = msg[i].Split(',');
                message.Detections.Add(
                    new Detection()
                    {
                        DateTime = new DateTime(
                                                Convert.ToInt16(rawDetection[0]),
                                                Convert.ToInt16(rawDetection[1]),
                                                Convert.ToInt16(rawDetection[2]),
                                                Convert.ToInt16(rawDetection[3]),
                                                Convert.ToInt16(rawDetection[4]),
                                                Convert.ToInt16(rawDetection[5]),
                                                Convert.ToInt16(rawDetection[6])),
                        TagId = rawDetection[7],
                        HostName = rawDetection[8],
                        Channel = Convert.ToInt16(rawDetection[9]),
                        Amplitude = float.Parse((rawDetection[10]), CultureInfo.InvariantCulture.NumberFormat),
                        ConfidenceLevel = float.Parse((rawDetection[11]), CultureInfo.InvariantCulture.NumberFormat),
                        MovingStatus = SonitorConverter.ConvertToMovingStatus(Convert.ToInt16(rawDetection[12])),
                        BatteryStatus = SonitorConverter.ConvertToBatteryStatus(Convert.ToInt16(rawDetection[13])),
                        ButtonAState = SonitorConverter.ConvertToButtonState(Convert.ToInt16(rawDetection[14])),
                        ButtonBState = SonitorConverter.ConvertToButtonState(Convert.ToInt16(rawDetection[15])),
                        ButtonCState = SonitorConverter.ConvertToButtonState(Convert.ToInt16(rawDetection[16])),
                        ButtonDState = SonitorConverter.ConvertToButtonState(Convert.ToInt16(rawDetection[17])),
                        SelectedField = SonitorConverter.ConvertToField(Convert.ToInt16(rawDetection[18]))
                    });
            }

            DetectionsReceived(this, new SonitorEventArgs(message));
        }

    }
}

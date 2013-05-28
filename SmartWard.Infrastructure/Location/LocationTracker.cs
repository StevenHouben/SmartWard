using SmartWard.Infrastructure.Location.Sonitor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Location
{
    public class LocationTracker:ITracker
    {
        public Dictionary<string, Tag> Tags = new Dictionary<string, Tag>();
        public Dictionary<string, Detector> Detectors = new Dictionary<string, Detector>();

        private SonitorTracker tracker = new SonitorTracker();
        public LocationTracker()
        {
            tracker.DetectionsReceived += tracker_DetectionsReceived;
            tracker.DetectorsReceived += tracker_DetectorsReceived;
            tracker.DetectorStatusReceived += tracker_DetectorStatusReceived;
            tracker.MapsReceived += tracker_MapsReceived;
            tracker.ProtocolReceived += tracker_ProtocolReceived;
            tracker.TagsReceived += tracker_TagsReceived;
        }

        public void Start()
        {
            tracker.Start();
        }
        public void Stop()
        {
            tracker.Stop();
        }

        public event TagFoundHandler TagFound = delegate { };
        public event TagStateChangedHandler TagStateChanged = delegate { };
        void tracker_TagsReceived(object sender, SonitorEventArgs e)
        {
            var msg = (TagsMessage)e.Message;
            //Console.WriteLine(msg.KeyWord + " received");
            foreach (var tag in msg.Tags)
            {
                if (!Tags.ContainsKey(tag.Id))
                {
                    Tags.Add(tag.Id, tag);
                    TagFound(this, new TagEventArgs(tag));
                }
                else
                {
                    Tags[tag.Id] = tag;
                    TagStateChanged(this, new TagEventArgs(tag));
                }
            }
        }

        void tracker_ProtocolReceived(object sender, SonitorEventArgs e)
        {
            var msg = (ProtocolVersionMessage)e.Message;
        }

        void tracker_MapsReceived(object sender, SonitorEventArgs e)
        {
            var msg = (MapsMessage)e.Message;
        }


        public event DetectorAddedHandler DetectorAdded = delegate{};
        public event DetectorRemovedHandler DetectorRemoved = delegate{};
        public event DetectorStateChangedHandler DetectorStateChanged = delegate { };
        void tracker_DetectorStatusReceived(object sender, SonitorEventArgs e)
        {
            var msg = (DetectorStatusMessage)e.Message;
            //Console.WriteLine(msg.KeyWord + " received");

            foreach (var state in msg.DetectorStates)
            {
                Detectors[state.HostName].Channel = state.Channel;
                Detectors[state.HostName].Status = (state.Online ? OperationStatus.Online : OperationStatus.Offline);
                DetectorStateChanged(this,new DetectorEventArgs(Detectors[state.HostName]));
            }
            
        }

        void tracker_DetectorsReceived(object sender, SonitorEventArgs e)
        {
            var msg = (DetectorsMessage)e.Message;
            //Console.WriteLine(msg.KeyWord + " received");

            foreach (var det in msg.Detectors)
            {
                if (!Detectors.ContainsKey(det.HostName))
                {
                    Detectors.Add(det.HostName, det);
                    DetectorAdded(this, new DetectorEventArgs(Detectors[det.HostName]));
                }
                else
                {
                    Detectors[det.HostName] = det;
                    DetectorStateChanged(this, new DetectorEventArgs(Detectors[det.HostName]));
                }
            }
        }

        public event DetectionHandler Detection = delegate { };
        void tracker_DetectionsReceived(object sender, SonitorEventArgs e)
        {
            var msg = (DetectionsMessage)e.Message;
            //Console.WriteLine(msg.KeyWord + " received");

            foreach (var detection in msg.Detections)
            {
                Tags[detection.TagId].BatteryStatus = detection.BatteryStatus;
                Tags[detection.TagId].ButtonA = detection.ButtonAState;
                Tags[detection.TagId].ButtonB = detection.ButtonBState;
                Tags[detection.TagId].ButtonC = detection.ButtonCState;
                Tags[detection.TagId].ButtonD = detection.ButtonDState;
                Tags[detection.TagId].MovingStatus = detection.MovingStatus;

                Detection(Detectors[detection.HostName],
                    new DetectionEventArgs()
                    {
                        Aplitude = detection.Amplitude,
                        Confidence = detection.ConfidenceLevel,
                        Detector = Detectors[detection.HostName],
                        Tag = Tags[detection.TagId],
                        TimeStamp = detection.DateTime
                    });
            }
        }

    }
}

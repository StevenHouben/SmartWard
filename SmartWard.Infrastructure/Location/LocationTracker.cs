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
        #region Events
        public event TagAddedHandler TagAdded= delegate { };
        public event TagStateChangedHandler TagStateChanged = delegate { };
        public event TagMovedHandler TagMoved = delegate { };

        public event DetectorAddedHandler DetectorAdded = delegate { };
        public event DetectorRemovedHandler DetectorRemoved = delegate { };
        public event DetectorStateChangedHandler DetectorStateChanged = delegate { };

        public event TagEnterHandler TagEnter = delegate { };
        public event TagLeaveHandler TagLeave = delegate { };


        public event DetectionHandler Detection = delegate { };

        public event TagBatteryHandler TagBatteryDataReceived = delegate { };
        public event TagButtonHandler TagButtonDataReceived = delegate { };
        #endregion

        #region Properties
        public Dictionary<string, Tag> Tags {get; private set;}
        public Dictionary<string, Detector> Detectors { get; private set; }
        #endregion

        #region Members
        private SonitorTracker tracker = new SonitorTracker();
        #endregion

        #region Constructor
        public LocationTracker()
        {
            Tags = new Dictionary<string, Tag>();
            Detectors = new Dictionary<string, Detector>();

            tracker.DetectionsReceived += tracker_DetectionsReceived;
            tracker.DetectorsReceived += tracker_DetectorsReceived;
            tracker.DetectorStatusReceived += tracker_DetectorStatusReceived;
            tracker.MapsReceived += tracker_MapsReceived;
            tracker.ProtocolReceived += tracker_ProtocolReceived;
            tracker.TagsReceived += tracker_TagsReceived;
        }
        #endregion

        #region ITracker
        public void Start()
        {
            tracker.Start();
        }
        public void Stop()
        {
            tracker.Stop();
        }
        #endregion

        #region Event Handlers
        private void tracker_TagsReceived(object sender, SonitorEventArgs e)
        {
            var msg = (TagsMessage)e.Message;
            foreach (var tag in msg.Tags)
            {
                if (!Tags.ContainsKey(tag.Id))
                {
                    Tags.Add(tag.Id, tag);
                    TagAdded(null, new TagEventArgs(tag));
                }
                else
                {
                    Tags[tag.Id] = tag;
                    TagStateChanged(null, new TagEventArgs(tag));
                }
            }
        }
        private void tracker_ProtocolReceived(object sender, SonitorEventArgs e)
        {
            var msg = (ProtocolVersionMessage)e.Message;
        }
        private void tracker_MapsReceived(object sender, SonitorEventArgs e)
        {
            var msg = (MapsMessage)e.Message;
        }
        private void tracker_DetectorStatusReceived(object sender, SonitorEventArgs e)
        {
            var msg = (DetectorStatusMessage)e.Message;

            foreach (var state in msg.DetectorStates)
            {
                Detectors[state.HostName].Channel = state.Channel;
                Detectors[state.HostName].Status = (state.Online ? OperationStatus.Online : OperationStatus.Offline);
                DetectorStateChanged(this,new DetectorEventArgs(Detectors[state.HostName]));
            }
            
        }
        private void tracker_DetectorsReceived(object sender, SonitorEventArgs e)
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
        private void tracker_DetectionsReceived(object sender, SonitorEventArgs e)
        {
            var msg = (DetectionsMessage)e.Message;

            foreach (var detection in msg.Detections)
            {
                CheckDetectorChanges(detection);
                CheckBatteryData(detection);
                CheckTagButtonData(detection);
                CheckTagMove(detection);

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

        private void CheckTagMove(Detection detection)
        {
            if (Tags[detection.TagId].MovingStatus != detection.MovingStatus)
            {
                Tags[detection.TagId].MovingStatus = detection.MovingStatus;
                TagMoved(Detectors[detection.HostName], new TagEventArgs(Tags[detection.TagId]));
            }
        }

        private void CheckTagButtonData(Detection detection)
        {
            if (Tags[detection.TagId].ButtonA != detection.ButtonAState ||
                Tags[detection.TagId].ButtonB != detection.ButtonBState ||
                Tags[detection.TagId].ButtonC != detection.ButtonCState ||
                Tags[detection.TagId].ButtonD != detection.ButtonDState)
            {
                Tags[detection.TagId].ButtonA = detection.ButtonAState;
                Tags[detection.TagId].ButtonB = detection.ButtonBState;
                Tags[detection.TagId].ButtonC = detection.ButtonCState;
                Tags[detection.TagId].ButtonD = detection.ButtonDState;

                TagButtonDataReceived(Tags[detection.TagId], new TagEventArgs(Tags[detection.TagId]));
            }
        }

        private void CheckBatteryData(Detection detection)
        {
            if (Tags[detection.TagId].BatteryStatus != detection.BatteryStatus)
            {
                Tags[detection.TagId].BatteryStatus = detection.BatteryStatus;
                TagBatteryDataReceived(Tags[detection.TagId], new TagEventArgs(Tags[detection.TagId]));
            }
        }

        private void CheckDetectorChanges(Detection detection)
        {
            if (Tags[detection.TagId].Detector != null)
            {

                if (detection.HostName != Tags[detection.TagId].Detector.HostName)
                {
                    Tags[detection.TagId].Detector.DetachTag(Tags[detection.TagId]);
                    TagLeave(Tags[detection.TagId].Detector, new TagEventArgs(Tags[detection.TagId]));

                    Detectors[detection.HostName].AttachTag(Tags[detection.TagId]);
                    Tags[detection.TagId].Detector = Detectors[detection.HostName];
                    TagEnter(Detectors[detection.HostName], new TagEventArgs(Tags[detection.TagId]));
                }

            }
            else
            {
                Detectors[detection.HostName].AttachTag(Tags[detection.TagId]);
                Tags[detection.TagId].Detector = Detectors[detection.HostName];
                TagEnter(Detectors[detection.HostName], new TagEventArgs(Tags[detection.TagId]));
            }
        }

        #endregion

    }
}

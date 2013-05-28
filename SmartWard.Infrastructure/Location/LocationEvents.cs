using SmartWard.Infrastructure.Location.Sonitor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Location
{
        public delegate void TagFoundHandler(Object sender, TagEventArgs e);
        public delegate void TagLostHandler(Object sender, TagEventArgs e);
        public delegate void TagMovedHandler(Object sender, TagEventArgs e);
        public delegate void TagStateChangedHandler(Object sender, TagEventArgs e);

        public delegate void DetectorAddedHandler(Object sender, DetectorEventArgs e);
        public delegate void DetectorRemovedHandler(Object sender, DetectorEventArgs e);
        public delegate void DetectorStateChangedHandler(Object sender, DetectorEventArgs e);

        public delegate void DetectionHandler(Detector detector, DetectionEventArgs e);

        public class TagEventArgs
        {
            public Tag Tag { get; set; }
            public TagEventArgs(Tag tag)
            {
                Tag = tag;
            }
        }
        public class DetectorEventArgs
        {
            public Detector Detector { get; set; }
            public DetectorEventArgs(Detector detector)
            {
                Detector = detector;
            }
        }
        public class DetectionEventArgs
        {
            public Detector Detector { get; set; }
            public Tag Tag { get; set; }
            public DateTime TimeStamp { get; set; }
            public float Aplitude { get; set; }
            public float Confidence { get; set; }
        }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Location.Sonitor
{
    public class SonitorMessage
    {
        public string KeyWord { get; set; }
    }
    public class ProtocolVersionMessage:SonitorMessage
    {
        public string Version { get; set; }
        public ProtocolVersionMessage(string version)
        {
            this.KeyWord = this.GetType().Name.ToUpper();
            this.Version = version;
        }
    }
    
    public class DetectorsMessage : SonitorMessage
    {
        public List<Detector> Detectors { get; set; }
        public DetectorsMessage()
        {
            this.KeyWord = this.GetType().Name.ToUpper();
            this.Detectors = new List<Detector>();
        }
    }
    public class DetectorStatusMessage : SonitorMessage
    {
        public List<DetectorStatus> DetectorStates { get; set; }
        public DetectorStatusMessage()
        {
            this.KeyWord = this.GetType().Name.ToUpper();
            this.DetectorStates = new List<DetectorStatus>();
        }

    }
   
    public class TagsMessage : SonitorMessage
    {
        public List<Tag> Tags { get; set; }
        public TagsMessage()
        {
            this.KeyWord = this.GetType().Name.ToUpper();
            Tags = new List<Tag>();
        }
    }
    
    public class MapsMessage : SonitorMessage
    {
        public List<Map> Maps { get; set; }
        public MapsMessage()
        {
            this.KeyWord = this.GetType().Name.ToUpper();
            Maps = new List<Map>();
        }
    }
    
    public class DetectionsMessage : SonitorMessage
    { 
        public List<Detection> Detections { get; set; }
        public DetectionsMessage()
        {
            this.KeyWord = this.GetType().Name.ToUpper();
            Detections = new List<Detection>();
        }
    }
    public enum SonitorMessages
    { 
        PROTOCOLVERSION,
        DETECTORS,
        DETECTORSTATUS,
        TAGS,
        MAPS,
        DETECTION
    }

}

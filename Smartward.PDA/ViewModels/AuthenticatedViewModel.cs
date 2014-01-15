using SmartWard.Commands;
using SmartWard.Infrastructure;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Networking.Proximity;
using Windows.Storage.Streams;

namespace SmartWard.PDA.ViewModels
{
    class AuthenticatedViewModel : ViewModelBase
    {
        private string _data; 
        private bool _NfcDetected;
        private ProximityDevice _proximityDevice; 
        private long _MessageType;

        public AuthenticatedViewModel() 
        { 
            _proximityDevice = ProximityDevice.GetDefault(); 
            if (_proximityDevice != null) 
            { 
                _proximityDevice.DeviceArrived += _proximityDevice_DeviceArrived; 
                _proximityDevice.DeviceDeparted += _proximityDevice_DeviceDeparted; 
                _MessageType = _proximityDevice.SubscribeForMessage("Windows", MessageReceivedHandler); 
            }
            InitializeNotifications(WardNode.StartWardNodeAsSystem(WebConfiguration.DefaultWebConfiguration));
        } 
 
 
        void _proximityDevice_DeviceDeparted(ProximityDevice sender) 
        { 
            NfcDetected = false; 
            Data = null; 
        } 
 
        void _proximityDevice_DeviceArrived(ProximityDevice sender) 
        { 
            NfcDetected = true; 
        } 
 
 
        private void MessageReceivedHandler(ProximityDevice sender, ProximityMessage message) 
        { 
            try 
            { 
                using (var reader = DataReader.FromBuffer(message.Data)) 
                { 
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf16LE; 
                    string receivedString = reader.ReadString(reader.UnconsumedBufferLength / 2 - 1); 
                    Debug.WriteLine("Received message from NFC: " + receivedString); 
                    Data = receivedString; 
                } 
 
            } 
            catch (Exception e) 
            { 
                Debug.WriteLine(e.StackTrace); 
            } 
 
        } 
         
 
        private void DoWriteTag() 
        { 
            try 
            { 
                using (var writer = new DataWriter{ UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf16LE } ) 
                { 
                    Debug.WriteLine("Writing message to NFC: " + Data); 
                    writer.WriteString(Data); 
                    long id = _proximityDevice.PublishBinaryMessage("WindowsUri:WriteTag", writer.DetachBuffer()); 
                    _proximityDevice.StopPublishingMessage(id); 
                } 
            } 
            catch (Exception e) 
            { 
                Debug.WriteLine(e.StackTrace); 
            } 
 
        } 
 
        RelayCommand _writeCommand; 
        public ICommand WriteCommand 
        { 
            get 
            { 
                if (_writeCommand == null) 
                { 
                    _writeCommand = new RelayCommand(p => this.DoWriteTag(), p => this.NfcDetected); 
                } 
                return _writeCommand; 
            } 
        } 
 
        public string Data 
        { 
            get { return _data; } 
            set 
            { 
                if (value.Equals(_data)) return; 
                OnPropertyChanged(); 
                _data = value; 
            } 
        } 
 
        public bool NfcDetected 
        { 
            get { return _NfcDetected; } 
            set 
            { 
                _NfcDetected = value; 
                OnPropertyChanged("NfcDetected"); 
                OnPropertyChanged("NfcSearching"); 
            } 
        } 
 
        public bool NfcSearching 
        { 
            get { return !_NfcDetected; } 
        } 
 
 
 
        public event PropertyChangedEventHandler PropertyChanged; 
 
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) 
        { 
            PropertyChangedEventHandler handler = PropertyChanged; 
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName)); 
        } 
    }
}

using SmartWard.Infrastructure.Location.Sonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Location
{
        /// <summary>
        /// Sonitor Events
        /// </summary>
        public delegate void SonitorMessageReceivedHandler(Object sender, SonitorEventArgs e);

        public class SonitorEventArgs
        {
            public SonitorMessage Message { get; set; }
            public SonitorEventArgs(SonitorMessage message)
            {
                Message = message;
            }
        }

}

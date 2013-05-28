using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure
{
    /// <summary>
    /// Events used to distributed activity model
    /// </summary>
    public delegate void ActivityAddedHandler(Object sender, ActivityEventArgs e);
    public delegate void ActivityRemovedHandler(Object sender, ActivityRemovedEventArgs e);
    public delegate void ActivityChangedHandler(Object sender, ActivityEventArgs e);
    public delegate void ActivitySwitchedHandler(Object sender, ActivityEventArgs e);

    /// <summary>
    /// Participant event
    /// </summary>
    public delegate void UserAddedHandler(Object sender, UserEventArgs e);
    public delegate void UserRemovedHandler(Object sender, UserRemovedEventArgs e);
    public delegate void UserChangedHandler(Object sender, UserEventArgs e);

    public delegate void TCPDataReceivedHandler(Object sender,NetEventArgs e);
}


﻿using System;

namespace SmartWard.Infrastructure.ActivityBase
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

    public delegate void TcpDataReceivedHandler(Object sender,NetEventArgs e);

    /// <summary>
    /// Connection event
    /// </summary>>
    public delegate void ConnectionEstablishedHandler(Object sender, EventArgs e);

    /// <summary>
    /// Intialize event
    /// </summary>
    public delegate void InitializedHandler(Object sender, EventArgs e);
}


/****************************************************************************
 (c) 2013 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)

 Pervasive Interaction Technology Laboratory (pIT lab)
 IT University of Copenhagen

 This library is free software; you can redistribute it and/or 
 modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
 as published by the Free Software Foundation. Check 
 http://www.gnu.org/licenses/gpl.html for details.
****************************************************************************/

using SmartWard.Users;
using System;

namespace SmartWard.Infrastructure
{
    public class UserEventArgs
    {
        public User User { get; set; }
        public UserEventArgs() { }
        public UserEventArgs(User user)
        {
            User = user;
        }
    }
    public class UserRemovedEventArgs
    {
        public string Id { get; set; }
        public UserRemovedEventArgs() { }
        public UserRemovedEventArgs(string id)
        {
            Id = id;
        }
    }
}

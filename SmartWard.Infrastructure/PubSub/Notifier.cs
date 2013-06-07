﻿using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.PubSub
{
    public class Notifier
    {
        private static IPersistentConnectionContext context = GlobalHost.ConnectionManager.GetConnectionContext<EventDispatcher>();

        public static void Subscribe(Guid connectionId, Guid groupId)
        {
            context.Groups.Add(connectionId.ToString(), groupId.ToString());
        }
        public static void Unsubscribe(Guid connectionId, Guid groupId)
        {
            context.Groups.Remove(connectionId.ToString(), groupId.ToString());
        }

        public static void Subscribe(Guid connectionId, string groupName)
        {
            context.Groups.Add(connectionId.ToString(), groupName);
        }

        public static void Unsubscribe(Guid connectionId, string groupName)
        {
            context.Groups.Remove(connectionId.ToString(), groupName);
        }

        public static void NotifyGroup(Guid groupId, NotificationType type, object obj)
        {
            context.Groups.Send(groupId.ToString(), ConstructEvent(type, obj));
        }

        public static void NotifyGroup(string groupName, NotificationType type, object obj)
        {
            context.Groups.Send(groupName, ConstructEvent(type, obj));
        }

        public static void NotifyAll(NotificationType type, object obj)
        {
            context.Connection.Broadcast(ConstructEvent(type, obj));
        }

        public static object ConstructEvent(NotificationType type, object obj)
        {
            return new { Event = type.ToString(), Data = obj };
        }
    }
    public enum NotificationType
    {
        ActivityAdded,
        ActivityUpdated,
        ActivityDeleted,
        FileDownload,
        FileUpload,
        FileDelete,
        FriendRequest,
        FriendAdded,
        FriendDeleted,
        UserConnected,
        UserDisconnected,
        UserStatusChanged,
        Message,
        None,
        ParticipantAdded,
        ParticipantRemoved
    }
}
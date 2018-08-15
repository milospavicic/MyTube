using MyTube.Models;
using System;
using System.Collections.Generic;

namespace MyTube.Repository
{
    interface ISubscribeRepository : IDisposable
    {
        bool SubscriptionExists(string channelSubscribedTo, string subscriber);

        IEnumerable<Subscriber> GetSubscribersForUser(string subscriber);

        void NewSubscription(string channelSubscribedTo, string subscriber);

        void DeleteSubscription(string channelSubscribedTo, string subscriber);
    }
}

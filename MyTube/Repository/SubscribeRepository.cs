using MyTube.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTube.Repository
{
    public class SubscribeRepository : ISubscribeRepository
    {
        private MyTubeDBEntities db;

        public SubscribeRepository(MyTubeDBEntities db)
        {
            this.db = db;
        }


        public bool SubscriptionExists(string channelSubscribedTo, string subscriber)
        {
            bool subExists = db.Subscribers.Any(u => u.ChannelSubscribed == channelSubscribedTo && u.Subscriber1 == subscriber);
            return subExists;
        }

        public IEnumerable<Subscriber> GetSubscribersForUser(string subscriber)
        {
            throw new NotImplementedException();
        }

        public void NewSubscription(string channelSubscribedTo, string subscriber)
        {
            Subscriber newSub = new Subscriber
            {
                ChannelSubscribed = channelSubscribedTo,
                Subscriber1 = subscriber
            };
            db.Subscribers.Add(newSub);

            db.SaveChanges();
        }

        public void DeleteSubscription(string channelSubscribedTo, string subscriber)
        {
            Subscriber sub = db.Subscribers.Single(x => x.ChannelSubscribed == channelSubscribedTo && x.Subscriber1 == subscriber);
            db.Subscribers.Remove(sub);

            db.SaveChanges();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
﻿// ReSharper disable InconsistentNaming

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EasyNetQ.Tests.Integration
{
    [TestFixture]
    public class SubscribeToErrorQueueSpike
    {
        private IBus bus;

        [SetUp]
        public void SetUp()
        {
            bus = RabbitHutch.CreateBus("host=localhost");
        }

        [TearDown]
        public void TearDown()
        {
            bus.Dispose();
        }

        [Test]
        [Explicit("Requires a RabbitMQ server on localhost")]
        public void Should_create_some_error_messages()
        {
            bus.Subscribe<MyMessage>("error_read_test", message =>
            {
                throw new Exception("Something bad happened!");
            });

            bus.Publish(new MyMessage{ Text = "this will inevitably fail"});

            // allow time for the subscription exception to throw and the 
            // error message to get written.
            Thread.Sleep(1000);
        }

        //[Test]
        //[Explicit("Requires a RabbitMQ server on localhost")]
        //public void Should_be_able_to_subscribe_to_error_messages()
        //{
        //    var errorQueueName = new Conventions(new TypeNameSerializer()).ErrorQueueNamingConvention();

        //    var queue = bus.Advanced.QueueDeclare(errorQueueName);
        //    var autoResetEvent = new AutoResetEvent(false);

        //    bus.Advanced.Consume<SystemMessages.Error>(queue, (message, info) =>
        //    {
        //        var error = message.Body;

        //        Console.Out.WriteLine("error.DateTime = {0}", error.DateTime);
        //        Console.Out.WriteLine("error.Exception = {0}", error.Exception);
        //        Console.Out.WriteLine("error.Message = {0}", error.Message);
        //        Console.Out.WriteLine("error.RoutingKey = {0}", error.RoutingKey);

        //        autoResetEvent.Set();
        //        return Task.Factory.StartNew(() => { });
        //    });

        //    autoResetEvent.WaitOne(1000);
        //}
    }
}

// ReSharper restore InconsistentNaming
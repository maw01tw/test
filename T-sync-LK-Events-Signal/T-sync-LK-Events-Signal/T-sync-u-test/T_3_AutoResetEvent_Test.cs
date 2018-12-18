using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace T_sync_u_test
{
    [TestClass]
    public class T_3_AutoResetEvent_Test
    {
        [TestMethod]
        public void C_AutoResetEvent_Test()
        {
            AutoResetEvent are = new AutoResetEvent(false);

            object tlock = new object();
            int count = 0;

            Action action_pass_counter = () =>
            {
                // WaitOne() returns true if AutoResetEvent is set,
                // returns false if AutoResetEvent is not set, but due to
                // a timeout (2 * 1000 milliseconds)
                if (are.WaitOne(2 * 1000))
                {
                    lock (tlock) { count++; }
                }
            };

            // ********** Test No.1 **********
            // Start 3 threads and wait them to finish
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(action_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_pass_counter));

            Task.WaitAll(tasks.ToArray());

            // Because the AutoResetEvent is not set (closed),
            // count++ is not executed, count = 0
            Assert.AreEqual(0, count);

            // ********** Test No.2 **********
            // Open the AutoResetEvent door
            are.Set();

            // Start 3 threads and wait them to finish
            tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(action_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_pass_counter));

            Task.WaitAll(tasks.ToArray());

            // Since the AutoResetEvent is set (open), count++ can be executed.
            // but the AutoResetEvent only allows a single thread to pass the door.
            // Once a thread passes the door, AutoResetEvent will be closed atomically.
            // count = 1
            // AutoResetEvent bahaves like a toll-booth
            Assert.AreEqual(1, count);

            // ********** Test No.3 **********
            // Once the AutoResetEvent toll-booth is closed, it remains closed
            // unless a Set() is issued to open it
            tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(action_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_pass_counter));

            Task.WaitAll(tasks.ToArray());

            // The count remains 1 because the AutoResetEvent is closed (Reset)
            Assert.AreEqual(1, count);
        }
    }
}

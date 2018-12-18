using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace T_sync_u_test
{
    [TestClass]
    public class T_2_ManualResetEvent_Test
    {
        [TestMethod]
        public void B_ManualResetEvent_Test()
        {
            // Initiate a ManualResetEvent with door closed (not set)
            ManualResetEvent mre = new ManualResetEvent(false);

            object tlock = new object();
            int count = 0;

            Action action_pass_counter = () =>
            {
                // WaitOne() returns true if ManualResetEvent is set,
                // returns false if ManualResetEvent is not set, but due to
                // a timeout (2 * 1000 milliseconds)
                if (mre.WaitOne(2 * 1000))
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

            // Because the ManualResetEvent is not set (closed),
            // count++ is not executed, count = 0
            Assert.AreEqual(0, count);

            // ********** Test No.2 **********
            // Open the ManualResetEvent door
            mre.Set();

            // Start 3 threads and wait them to finish
            tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(action_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_pass_counter));

            Task.WaitAll(tasks.ToArray());

            // Since the ManualResetEvent is set (open),
            // count++ is executed, count = 3
            Assert.AreEqual(3, count);


            // ********** Test No.3 **********
            // To close the ManualResetEvent, we need to call the rest()
            // method manually
            mre.Reset();

            // Start 3 threads and wait them to finish
            tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(action_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_pass_counter));

            Task.WaitAll(tasks.ToArray());

            // Since the ManualResetEvent is re-set (closed),
            // count++ is not executed, count remains 3
            Assert.AreEqual(3, count);
        }
    }
}

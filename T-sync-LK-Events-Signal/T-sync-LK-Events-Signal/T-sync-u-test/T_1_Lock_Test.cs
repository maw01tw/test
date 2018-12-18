using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace T_sync_u_test
{
    [TestClass]
    public class T_1_Lock_Test
    {
        [TestMethod]
        public void A_Lock_Test()
        {
            object tlock = new object();

            // Each thread will add 1 to the count in the test
            int count = 0;

            Action action_counter = () =>
            {
                lock (tlock)
                {
                    // Without the lock block, there is a possibility
                    // that different thread can read the same value
                    // from "prev_count"
                    int prev_count = count;
                    int next_count = count + 1;

                    count = next_count;
                }
            };

            // Start 3 threads and wait for them to finish
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(action_counter));
            tasks.Add(Task.Factory.StartNew(action_counter));
            tasks.Add(Task.Factory.StartNew(action_counter));

            Task.WaitAll(tasks.ToArray());

            // Three threads modified the count, so the count is guaranteed 3
            Assert.AreEqual(3, count);
        }
    }
}

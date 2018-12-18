using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace T_sync_u_test
{
    [TestClass]
    public class T_5_EventWaitHandle_Test
    {
        [TestMethod]
        public void E_EventWaitHandle_Test()
        {
            // EventWaitHandle
            EventWaitHandle ewh_manual
                = new EventWaitHandle(false, EventResetMode.ManualReset);

            Func<bool> func_manual_is_blocked = () =>
            {
                bool blocked = true;
                if (ewh_manual.WaitOne(2 * 1000)) { blocked = false; }

                return blocked;
            };

            // ********** Test No.1 **********
            // When initiate with EventResetMode.ManualReset, EventWaitHandle
            // bahaves like ManualResetEvent. Once the door is open, it remains
            // open, until manually reset
            ewh_manual.Set();
            Assert.IsFalse(func_manual_is_blocked());
            Assert.IsFalse(func_manual_is_blocked());

            // Reset the EventWaitHandle, it blocks the func_manual()
            ewh_manual.Reset();
            Assert.IsTrue(func_manual_is_blocked());

            // EventWaitHandle
            EventWaitHandle ewh_auto
                = new EventWaitHandle(false, EventResetMode.AutoReset);

            int count = 0;
            object tlock = new object();

            Action action_auto_pass_counter = () =>
            {
                // If not blocked, add count by 1
                if (ewh_auto.WaitOne(2 * 1000))
                {
                    lock (tlock) { count++; }
                }
            };

            // ********** Test No.2 **********
            // When initiate with EventResetMode.AutoReset, EventWaitHandle
            // behaves like AutoResetEvent. When the door is open, it only
            // allows a single thread to pass and then closed (reset) atomically
            ewh_auto.Set();

            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(action_auto_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_auto_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_auto_pass_counter));

            Task.WaitAll(tasks.ToArray());

            // Since only one thread can pass WaitOne(), the
            // count = 1
            Assert.AreEqual(1, count);

            // ********** Test No.3 **********
            // Since the door is closed, no thread can go through,
            // the count remains 1
            tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(action_auto_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_auto_pass_counter));
            tasks.Add(Task.Factory.StartNew(action_auto_pass_counter));

            Task.WaitAll(tasks.ToArray());

            Assert.AreEqual(1, count);
        }
    }
}

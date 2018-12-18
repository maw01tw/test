using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace T_sync_u_test
{
    [TestClass]
    public class T_4_CountdownEvent_Test
    {
        [TestMethod]
        public void D_CountdownEvent_Test()
        {
            // Initiate a CountdownEvent that needs 3 signals to set (open)
            CountdownEvent cde = new CountdownEvent(3);

            Func<bool> func_is_blocked = () =>
            {
                bool blocked = true;

                // The Wait method returns true if CountdownEvent is set
                // by getting enough signals (3). It returns false, if it
                // reaches the timeout (2 * 1000 milliseconds)
                if (cde.Wait(2 * 1000)) { blocked = false; }

                return blocked;
            };

            // ********** Test No.1 **********
            // Since there is no sigal, the CountdownEvent.Wait() method
            // will timeout (blocked == true)
            Assert.IsTrue(func_is_blocked());

            // Start 3 threads. Each give the CountdownEvent a signal
            Action action_issue_signal = () => { cde.Signal(); };

            Task.Run(action_issue_signal);
            Task.Run(action_issue_signal);
            Task.Run(action_issue_signal);

            // ********** Test No.2 **********
            // The CountdownEvent has 3 signals, the CountdownEvent.Wait()
            // returns immediately with true.
            Assert.IsFalse(func_is_blocked());

            // ********** Test No.3 **********
            // Once the door is open, it remains open until reset by the program
            Assert.IsFalse(func_is_blocked());

            // ********** Test No.4 **********
            // Calling the Signal() method more than the initial value on the
            // CountdownEvent causes an exception
            bool exceptionThrown = false;
            try { cde.Signal(); }
            catch (Exception) { exceptionThrown = true; }

            Assert.IsTrue(exceptionThrown);
        }
    }
}

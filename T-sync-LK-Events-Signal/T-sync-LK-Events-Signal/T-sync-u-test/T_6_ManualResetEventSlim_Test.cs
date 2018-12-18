using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace T_sync_u_test
{
    [TestClass]
    public class T_6_ManualResetEventSlim_Test
    {
        [TestMethod]
        public void F_ManualResetEventSlim_Test()
        {
            ManualResetEventSlim mres = new ManualResetEventSlim(false);

            Func<bool> func_mres_is_blocked = () =>
            {
                bool blocked = true;
                if (mres.Wait(2 * 1000)) { blocked = false; }

                return blocked;
            };

            // ********** Test No.1 **********
            // Since the ManualResetEventSlim is initiated as closed (not set),
            // it blocks the thread.
            Assert.IsTrue(func_mres_is_blocked());

            // ********** Test No.2 **********
            // Set the ManualResetEventSlim opens the door, and it remains open
            // until it is closed (reset)
            mres.Set();
            Assert.IsFalse(func_mres_is_blocked());
            Assert.IsFalse(func_mres_is_blocked());

            // ********** Test No.3 **********
            // Reset the ManualResetEventSlim closes the door, it remains closed
            // if not openned by Set()
            mres.Reset();
            Assert.IsTrue(func_mres_is_blocked());
            Assert.IsTrue(func_mres_is_blocked());
        }
    }
}

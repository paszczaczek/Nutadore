﻿using Nutadore;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nutadore.Tests
{
    [TestClass()]
    public class MiscTests
    {
        [TestMethod()]
        public void Test1()
        {
            var sp = StaffPosition.ByLineNumber(5.5);
        }
    }
}
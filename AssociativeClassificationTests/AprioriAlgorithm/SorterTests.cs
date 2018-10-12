using AssociativeClassification.AprioriAlgorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociativeClassificationTests.AprioriAlgorithm
{
    [TestClass()]
    public class SorterTests
    {
        [TestMethod()]
        public void SortTest()
        {
            //Arrange
            Sorter target = new Sorter();
            var token = "121;120;100";

            //Act
            string actual = target.Sort(token);

            //Assert
            Assert.AreEqual(actual, "100;120;121");
        }

        [TestMethod()]
        public void SortTestSingle()
        {
            //Arrange
            Sorter target = new Sorter();
            var token = "100";

            //Act
            string actual = target.Sort(token);

            //Assert
            Assert.AreEqual(actual, "100");
        }
    }
}

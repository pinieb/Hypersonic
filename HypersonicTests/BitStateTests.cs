using NUnit.Framework;
using System;
namespace HypersonicTests
{
	[TestFixture()]
	public class BitStateTests
	{
		[Test()]
		public void GetBitIndexTest_0_0_Returns0()
		{
			Assert.AreEqual(0, BitState.GetBitIndex(0, 0));
		}

		[Test()]
		public void GetBitIndexTest_12_10_Returns112()
		{
			Assert.AreEqual(112, BitState.GetBitIndex(12, 10));
		}

		[Test()]
		public void GetBitIndexTest_8_1_Returns17()
		{
			Assert.AreEqual(17, BitState.GetBitIndex(8, 1));
		}

		[Test()]
		public void GetBitIndexTest_1_1_ReturnsNeg1()
		{
			Assert.AreEqual(-1, BitState.GetBitIndex(1, 1));
		}

		[Test()]
		public void GetMapIndexTest_0_Returns0_0()
		{
			Assert.AreEqual(new Tuple<int, int>(0, 0), BitState.GetMapIndex(0));
		}

		[Test()]
		public void GetMapIndexTest_112_Returns12_10()
		{
			Assert.AreEqual(new Tuple<int, int>(12, 10), BitState.GetMapIndex(112));
		}
	}
}

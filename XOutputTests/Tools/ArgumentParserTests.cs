using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using XOutput.UpdateChecker;

namespace XOutput.Tools.Tests
{
	[TestClass()]
	public class ArgumentParserTests
	{
		[DataRow(new string[] { }, false)]
		[DataRow(new string[] { "--minimized" }, true)]
		[DataRow(new string[] { "other" }, false)]
		[DataRow(new string[] { "other", "--minimized" }, true)]
		[DataTestMethod]
		public void CompareTest(string[] arguments, bool minimized)
		{
			ArgumentParser parser = new ArgumentParser(arguments);
			Assert.AreEqual(minimized, parser.Minimized);
		}
	}
}

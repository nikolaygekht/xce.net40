// 
// ReplaceWithSingleCallToAnyIssueTests.cs
//
// Author:
//       Mike Kr?ger <mkrueger@xamarin.com>
// 
// Copyright (c) 2013 Xamarin <http://xamarin.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
    public class ReplaceWithSingleCallToAnyIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestWhereAnyCase1 ()
		{
			var input = @"using System.Linq;
public class CSharpDemo {
	public void Bla () {
		int[] arr;
		var bla = arr.Where (x => x < 4).Any ();
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new ReplaceWithSingleCallToAnyIssue (), input, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues, @"using System.Linq;
public class CSharpDemo {
	public void Bla () {
		int[] arr;
		var bla = arr.Any (x => x < 4);
	}
}");
		}
		
		[Test]
		public void TestWhereAnyWrongWhere1 ()
		{
			var input = @"using System.Linq;
public class CSharpDemo {
	public void Bla () {
		int[] arr;
		var bla = arr.Where ((x, i) => x + i < 4).Any ();
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new ReplaceWithSingleCallToAnyIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}
		
		[Test]
		public void TestWhereAnyWrongWhere2 ()
		{
			var input = @"using System;
using System.Linq;
public class X
{
	X Where (Func<int,int> f) { return null; }
	bool Any () { return false; }
	public void Bla () {
		X ex = null;
		var bla = ex.Where (x => x + 1).Any ();
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new ReplaceWithSingleCallToAnyIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

        [Test]
        public void TestDisable()
        {
            var input = @"using System.Linq;
public class CSharpDemo {
	public void Bla () {
		int[] arr;
        // ReSharper disable once ReplaceWithSingleCallToAny
		var bla = arr.Where (x => x < 4).Any ();
	}
}";

            TestRefactoringContext context;
            var issues = GetIssues(new ReplaceWithSingleCallToAnyIssue(), input, out context);
            Assert.AreEqual(0, issues.Count);
        }
	}
}

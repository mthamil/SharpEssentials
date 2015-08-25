using System.Collections.Generic;
using System.IO;
using SharpEssentials.InputOutput;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.InputOutput
{
	public class FileSystemInfoPathEqualityComparerTests
	{
		public static IEnumerable<object[]> EqualsData
		{
			get
			{
				var sameFile = new FileInfo(@"c:\test.txt");
				return new TheoryData<bool, FileSystemInfo, FileSystemInfo>
				{
					{ true,  sameFile, sameFile },
					{ true,  new FileInfo(@"c:\test.txt"),   new FileInfo(@"c:\test.txt") },
					{ true,  new FileInfo(@"c:\test.txt"),   new FileInfo(@"C:\test.TXT") },
                    { true,  new DirectoryInfo(@"c:\test\"), new DirectoryInfo(@"C:\TEST\") },
					{ false, new FileInfo(@"c:\test.txt"),   new FileInfo(@"test.txt") },
					{ false, new FileInfo(@"c:\test.txt"),   new FileInfo(@"c:\testDir\test.txt") },
					{ false, new FileInfo(@"c:\test.txt"),   null }
				};
			}
		}

		[Theory]
		[MemberData("EqualsData")]
        public void Test_Equals(bool expected, FileSystemInfo first, FileSystemInfo second)
		{
			// Act.
			bool actual = comparer.Equals(first, second);

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(@"c:\test.txt")]
		[InlineData(@"C:\test.TXT")]
		public void Test_GetHashCode(string path)
		{
			// Arrange.
			var file = new FileInfo(path);
			var expected = comparer.GetHashCode(new FileInfo(@"c:\test.txt"));

			// Act.
			int actual = comparer.GetHashCode(file);

			// Assert.
			Assert.Equal(expected, actual);
		}

		private readonly FileSystemInfoPathEqualityComparer comparer = new FileSystemInfoPathEqualityComparer();
	}
}
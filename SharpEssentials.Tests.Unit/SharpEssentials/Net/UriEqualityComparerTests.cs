using System;
using System.Collections.Generic;
using SharpEssentials.Net;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Net
{
	public class UriEqualityComparerTests
	{
		public static IEnumerable<object[]> EqualsData
		{
			get
			{
				var sameFile = new Uri(@"http://example.com");
				return new TheoryData<bool, Uri, Uri>
				{
					{ true,  sameFile, sameFile },
					{ true,  new Uri(@"http://example.com"), new Uri(@"http://example.com") },
					{ true,  new Uri(@"http://example.com"), new Uri(@"http://EXAMPLE.com") },
                    { true,  new Uri(@"http://example.com"), new Uri(@"http://example.com/") },
					{ false, new Uri(@"http://example.com"), new Uri(@"https://example.com") },
					{ false, new Uri(@"http://example.com"), new Uri(@"http://example") },
					{ false, new Uri(@"http://example.com"), null }
				};
			}
		}

		[Theory]
		[MemberData(nameof(EqualsData))]
        public void Test_Equals(bool expected, Uri first, Uri second)
		{
			// Act.
			bool actual = _underTest.Equals(first, second);

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(@"http://example.com/")]
		[InlineData(@"http://EXAMPLE.com")]
		public void Test_GetHashCode(string path)
		{
			// Arrange.
			var uri = new Uri(path);
			var expected = _underTest.GetHashCode(new Uri(@"http://example.com"));

			// Act.
			int actual = _underTest.GetHashCode(uri);

			// Assert.
			Assert.Equal(expected, actual);
		}

		private readonly UriEqualityComparer _underTest = new UriEqualityComparer();
	}
}
using System;
using System.Globalization;
using System.IO;
using SharpEssentials.Controls.Converters;
using Xunit;

namespace Tests.Unit.SharpEssentials.Controls.Converters
{
	public class FileInfoToUriConverterTests
	{
		[Fact]
		public void Test_Convert()
		{
			// Arrange.
			var file = new FileInfo(@"C:\file.txt");

			// Act.
			var uri = (Uri)converter.Convert(file, typeof(Uri), null, CultureInfo.CurrentCulture);

			// Assert.
			Assert.NotNull(uri);
			Assert.True(uri.IsFile);
			Assert.Equal(@"C:\file.txt", uri.LocalPath);
		}

		[Fact]
		public void Test_Convert_Null()
		{
			// Act.
			var uri = (Uri)converter.Convert(null, typeof(Uri), null, CultureInfo.CurrentCulture);

			// Assert.
			Assert.Null(uri);
		}

		[Fact]
		public void Test_ConvertBack()
		{
			// Arrange.
			var uri = new Uri(@"C:\file.txt");

			// Act.
			var file = (FileInfo)converter.ConvertBack(uri, typeof(FileInfo), null, CultureInfo.CurrentCulture);

			// Assert.
			Assert.NotNull(file);
			Assert.Equal(@"C:\file.txt", file.FullName);
		}

		[Fact]
		public void Test_ConvertBack_Null()
		{
			// Act.
			var file = (FileInfo)converter.ConvertBack(null, typeof(FileInfo), null, CultureInfo.CurrentCulture);

			// Assert.
			Assert.Null(file);
		}

		private readonly FileInfoToUriConverter converter = new FileInfoToUriConverter();
	}
}
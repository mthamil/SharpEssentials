using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SharpEssentials.InputOutput;
using SharpEssentials.Testing;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.InputOutput
{
	public class AsyncStreamExtensionTests
	{
		[Fact]
		public async Task Test_ReadAllBytesAsync()
		{
			// Arrange.
			var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

			// Act.
			var result = await stream.ReadAllBytesAsync(CancellationToken.None);

			// Assert.
			AssertThat.SequenceEqual(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, result);
		}

		[Fact]
		public async Task Test_WriteAllBytesAsync()
		{
			// Arrange.
			var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var stream = new MemoryStream();

			// Act.
			await stream.WriteAllBytesAsync(data, CancellationToken.None);

			byte[] streamData = new byte[data.Length];
			stream.Seek(0, SeekOrigin.Begin);
			await stream.ReadAsync(streamData, 0, streamData.Length);

			// Assert.
			AssertThat.SequenceEqual(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, streamData);
		}

		[Fact]
		public async Task Test_CopyToAsync()
		{
			// Arrange.
			var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var source = new MemoryStream(data);
			var destination = new MemoryStream();

			// Act.
			await source.CopyToAsync(destination, CancellationToken.None);

			byte[] destinationData = new byte[data.Length];
			destination.Seek(0, SeekOrigin.Begin);
			await destination.ReadAsync(destinationData, 0, destinationData.Length);

			// Assert.
			AssertThat.SequenceEqual(data, destinationData);
		}
	}
}

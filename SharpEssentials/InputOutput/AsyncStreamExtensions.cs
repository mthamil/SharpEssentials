// Sharp Essentials
// Copyright 2017 Matthew Hamilton - matthamilton@live.com
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SharpEssentials.InputOutput
{
    /// <summary>
    /// Contains extension methods for Streams.
    /// </summary>
    public static class AsyncStreamExtensions
    {
        /// <summary>
        /// Reads all bytes in a stream asynchronously.
        /// </summary>
        /// <param name="source">The stream to read.</param>
        /// <param name="cancellationToken">Allows cancellation of the read operation.</param>
        /// <returns>A task that can be used to retrieve the result.</returns>
        public static async Task<byte[]> ReadAllBytesAsync(this Stream source, CancellationToken cancellationToken = default(CancellationToken))
        {
            // We don't really care about the number of bytes read, so return the buffer instead.
            var tcs = new TaskCompletionSource<byte[]>();
            try
            {
                var buffer = new byte[source.Length];
                await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                tcs.TrySetResult(buffer);
            }
            catch (TaskCanceledException)
            {
                tcs.TrySetCanceled();
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }

            return await tcs.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Writes the given bytes to a stream asynchronously.
        /// </summary>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="data">The bytes to write.</param>
        /// <param name="cancellationToken">Allows cancellation of the write operation.</param>
        /// <returns>A task that can be used to wait for the operation to complete.</returns>
        public static Task WriteAllBytesAsync(this Stream destination, byte[] data, CancellationToken cancellationToken = default(CancellationToken))
        {
            return destination.WriteAsync(data, 0, data.Length, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads all bytes from the current stream and writes them to a 
        /// destination stream.
        /// </summary>
        /// <param name="source">The stream being read.</param>
        /// <param name="destination">The stream being copied to</param>
        /// <param name="cancellationToken">Allows cancellation of the copy operation.</param>
        /// <returns>A task representing the copy operation.</returns>
        public static Task CopyToAsync(this Stream source, Stream destination, CancellationToken cancellationToken = default(CancellationToken))
        {
            return source.CopyToAsync(destination, DefaultCopyBufferSize, cancellationToken);
        }

        private const int DefaultCopyBufferSize = 81920; 
    }
}
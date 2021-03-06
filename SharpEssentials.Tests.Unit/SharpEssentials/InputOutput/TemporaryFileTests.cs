﻿using System;
using System.IO;
using SharpEssentials.InputOutput;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.InputOutput
{
    public class TemporaryFileTests
    {
        [Fact]
        public void Test_File()
        {
            // Arrange.
            var temp = new TemporaryFile();

            // Act.
            var file = temp.File;

            // Assert.
            Assert.NotNull(file);
            Assert.Equal(Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar), file.DirectoryName.TrimEnd(Path.DirectorySeparatorChar));
        }

        [Fact]
        public void Test_File_With_Specific_Filename()
        {
            // Arrange.
            var temp = new TemporaryFile("testFile1.txt");

            // Act.
            var file = temp.File;

            // Assert.
            Assert.NotNull(file);
            Assert.Equal(Path.Combine(Path.GetTempPath(), "testFile1.txt"), file.FullName);
        }

        [Fact]
        public void Test_Touch()
        {
            // Arrange.
            using (var temp = new TemporaryFile())
            {
                // Act.
                var returned = temp.Touch();

                // Assert.
                Assert.True(temp.File.Exists);
                Assert.Same(temp, returned);
            }
        }

        [Fact]
        public void Test_WithContent()
        {
            // Arrange.
            using (var temp = new TemporaryFile())
            {
                // Act.
                var returned = temp.WithContent("stuff");

                // Assert.
                Assert.True(temp.File.Exists);
                Assert.Equal("stuff", File.ReadAllText(temp.File.FullName));
                Assert.Same(temp, returned);
            }
        }

        [Fact]
        public void Test_Dispose()
        {
            // Arrange.
            var temp = new TemporaryFile().Touch();

            // Act.
            temp.Dispose();
            temp.File.Refresh();

            // Assert.
            Assert.False(temp.File.Exists);
        }

        private FileInfo GetTempFile()
        {
            var temp = new TemporaryFile().Touch();
            return temp.File;
        }
    }
}
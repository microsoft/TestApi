// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Microsoft.Test.VariationGeneration;

namespace Microsoft.Test.AcceptanceTests
{
    public class ModelTests
    {
        [Fact]
        public void BasicModelTest()
        {
            var parameters = new List<Parameter>
            {
                new Parameter("P1")
                {
                    "one",
                    "two",
                },
                new Parameter("P2")
                {
                    1,
                    2
                },
                new Parameter("P3")
                {
                    1.0,
                    2.0,
                    3.14
                }
            };

            Model model = new Model(parameters);

            Assert.True(model.GenerateVariations(3).Count() == 2 * 2 * 3, "Incorrect number of variations");

            var expectedVariations = new List<string>
            {
                "one 1 1",
                "one 2 2",
                "one 1 3.14",
                "two 2 1",
                "two 1 2",
                "two 2 3.14"
            };

            var actualVariations = WriteVariations(model.GenerateVariations());

            Assert.True(expectedVariations.Count == actualVariations.Count, "Expected: " + expectedVariations.Count + " Actual: " + actualVariations.Count);

            for (int i = 0; i < expectedVariations.Count; i++)
            {
                Assert.True(expectedVariations[i] == actualVariations[i], "Expected: " + expectedVariations[i] + " Actual: " + actualVariations[i]);
            }
        }



        [Fact]
        public void LargeModelTest()
        {
            var parameters = new List<Parameter>
            {
                new Parameter("Type")
                {
                    "Single", "Spanned", "Striped", "Mirror", "RAID-5"
                },
                new Parameter("Size")
                {
                    10, 100, 1000, 10000, 40000
                },
                new Parameter("Format Method")
                {
                    FormatMethod.Quick, FormatMethod.Slow
                },
                new Parameter("File System")
                {
                    "FAT", "FAT32", "NTFS"
                },
                new Parameter("Cluster Size")
                {
                    512, 1024, 2048, 4096, 8192, 16384
                },
                new Parameter("Compression")
                {
                    true, false
                }
            };

            Model model = new Model(parameters);

            Assert.True(model.GenerateVariations(6).Count() == 5 * 5 * 2 * 3 * 6 * 2);

            var expectedVariations = new List<string>
            {
                "Single 10 Quick FAT 512 True",
                "Single 100 Slow FAT32 1024 False",
                "Single 1000 Quick NTFS 2048 False",
                "Single 10000 Quick FAT32 4096 True",
                "Single 40000 Quick FAT 8192 False",
                "Single 10 Slow NTFS 16384 True",
                "Spanned 1000 Slow FAT 512 True",
                "Spanned 10000 Quick NTFS 1024 False",
                "Spanned 10 Slow FAT32 2048 False",
                "Spanned 40000 Slow NTFS 4096 True",
                "Spanned 100 Quick NTFS 8192 True",
                "Spanned 100 Quick FAT 16384 False",
                "Striped 40000 Quick FAT32 512 False",
                "Striped 10000 Slow FAT 1024 True",
                "Mirror 10 Quick FAT 1024 True",
                "RAID-5 10 Quick FAT 4096 False",
                "Mirror 10 Slow FAT32 8192 False",
                "Mirror 100 Slow NTFS 512 True",
                "RAID-5 100 Slow FAT32 2048 True",
                "Striped 100 Slow NTFS 4096 True",
                "RAID-5 1000 Slow NTFS 1024 False",
                "Mirror 1000 Slow FAT32 4096 False",
                "Striped 1000 Quick FAT 8192 True",
                "Striped 1000 Slow FAT32 16384 True",
                "RAID-5 10000 Quick FAT 512 False",
                "Mirror 10000 Quick FAT 2048 True",
                "RAID-5 10000 Quick NTFS 8192 True",
                "Mirror 10000 Slow FAT 16384 False",
                "Striped 10 Quick FAT32 2048 False",
                "Mirror 40000 Slow NTFS 1024 False",
                "RAID-5 40000 Quick FAT32 2048 True",
                "RAID-5 40000 Quick FAT 16384 False",
            };

            var actualVariations = WriteVariations(model.GenerateVariations());

            Assert.True(expectedVariations.Count == actualVariations.Count, "Expected: " + expectedVariations.Count + " Actual: " + actualVariations.Count);

            for (int i = 0; i < expectedVariations.Count; i++)
            {
                Assert.True(expectedVariations[i] == actualVariations[i], "Expected: " + expectedVariations[i] + " Actual: " + actualVariations[i]);
            }
        }

        public static IList<string> WriteVariations(IEnumerable<Variation> variations)
        {
            List<string> strings = new List<string>();
            foreach (var v in variations)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var value in v)
                {
                    builder.Append(value.Value.ToString() + " ");
                }

                strings.Add(builder.ToString().TrimEnd());
            }

            return strings;
        }
    }

    public enum FormatMethod
    {
        Quick,
        Slow
    }
}

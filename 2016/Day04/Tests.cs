using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Jeroen
{

    public partial class Tests
    {
        ITestOutputHelper output;

        public Tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Can_Parse_Room()
        {
            var input = "aaaaa-bbb-z-y-x-123[abxyz]";
            var room = Room.Parse(input);
            Assert.Equal("aaaaa-bbb-z-y-x", room.Id);
            Assert.Equal("abxyz", room.Checksum);
            Assert.Equal(123, room.SectorId);
        }


        [Theory]
        [InlineData("aaaaa-bbb-z-y-x-123[abxyz]", true)]
        [InlineData("a-b-c-d-e-f-g-h-987[abcde]", true)]
        [InlineData("not-a-real-room-404[oarel]", true)]
        [InlineData("totally-real-room-200[decoy]", false)]
        public void Test(string s, bool expected)
        {
            Room room = Room.Parse(s);
            var result = room.IsValid();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part1()
        {
            var query = from line in ReadInput()
                        let room = Room.Parse(line)
                        where room.IsValid()
                        select room;
            var count = query.Sum(r => r.SectorId);
            output.WriteLine(count.ToString());
        }
        [Fact]
        public void Part2()
        {
            var query = from line in ReadInput()
                        let room = Room.Parse(line)
                        let name = room.Name
                        where name.Contains("northpole")
                        select room.SectorId;

            var result = query.Single();

            output.WriteLine(result.ToString());
        }


        [Theory]
        [InlineData('a', 0, 'a')]
        [InlineData('a', 1, 'b')]
        [InlineData('a', 25, 'z')] //  1 -> 26 =>  1 + 25 =  1 + 
        [InlineData('a', 26, 'a')] //  1 ->  1 =>  1 +  0 =  1 + 26 - 26
        [InlineData('b', 25, 'a')] //  2 ->  1 =>  2 -  1 =  2 + 25 - 26
        [InlineData('y', 3, 'b')]  //  'a' + (3+25)%26 = 'a' + 2 
        [InlineData('z', 1, 'a')]
        [InlineData('q', 343, 'v')]
        [InlineData('z', 343, 'e')]
        public void TestModulo26(char c, int rotations, char expected)
        {
            var offset = (c + rotations - 'a') % 26;
            var result = (char)('a' + offset);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestPart2()
        {
            var encrypted = "qzmt-zixmtkozy-ivhz";
            var decrypted = encrypted.Decrypt(343);
            Assert.Equal("very encrypted name", decrypted);
        }

        private IEnumerable<string> ReadInput()
        {
            using (var stream = File.OpenRead("input.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    yield return reader.ReadLine();
                }
            }
        }
    }
}

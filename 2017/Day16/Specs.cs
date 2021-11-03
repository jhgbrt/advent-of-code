using System.IO;
using Xunit;

namespace dance
{
    public class Specs
    {
        [Fact]
        // Spin, written sX, makes X programs move from the end to the front, but maintain 
        // their order otherwise. (For example, s3 on abcde produces cdeab)
        public void Spin_makes_programs_move_from_end_to_front_maintaining_order()
        {
            var input = "abcde";
            var result = input.Spin(3);
            Assert.Equal("cdeab", result);
        }

        [Fact]
        // Spin, written sX, makes X programs move from the end to the front, but maintain 
        // their order otherwise. (For example, s3 on abcde produces cdeab)
        public void s1_Results_in_spin()
        {
            var input = "abcde";
            var program = "s1";
            var result = new Dancer(new StringReader(program)).Run(input);
            Assert.Equal("eabcd", result);
        }

        [Fact]
        // Exchange, written xA/B, makes the programs at positions A and B swap places
        public void Exchange_swaps_positions()
        {
            var input = "abcde";
            var result = input.Exchange(2, 4);
            Assert.Equal("abedc", result);
        }
        [Fact]
        // Exchange, written xA/B, makes the programs at positions A and B swap places
        public void x3_4_Exchange_swaps_positions()
        {
            var input = "eabcd";
            var program = "x3/4";
            var result = new Dancer(new StringReader(program)).Run(input);
            Assert.Equal("eabdc", result);
        }

        [Fact]
        // Partner, written pA/B, makes the programs named A and B swap places
        public void Partner_swaps_characters()
        {
            var input = "eabdc";
            var result = input.Partner('e', 'b');
            Assert.Equal("baedc", result);
        }
        [Fact]
        // Partner, written pA/B, makes the programs named A and B swap places
        public void p_e_b_Partner_swaps_characters()
        {
            var input = "eabdc";
            var program = "pe/b";
            var result = new Dancer(new StringReader(program)).Run(input);
            Assert.Equal("baedc", result);
        }

        [Fact]
        public void SampleProgram()
        {
            var program = "s1," +
                          "x3/4," +
                          "pe/b";
            var dancer = new Dancer(new StringReader(program));
            var result = dancer.Run("abcde");
            Assert.Equal("baedc", result);
        }

        [Fact]
        public void FindCycle()
        {
            var program = "x15/1,s15,x2/3,s15,x11/1,pm/a,x6/0,s4,x8/7,s4,x12/9,pi/l,x10/2";

            var dancer = new Dancer(new StringReader(program));
            var input = "abcdefghijklmnop";

            var s = input;
            int cycle;
            for (int i = 1; true; i++)
            {
                s = dancer.Run(s);
                if (input == s)
                {
                    cycle = i;
                    break;
                }
            }

            Assert.Equal(input, dancer.Run(input, cycle));



            string reference = dancer.Run(input, 10000);

            string withcycles = dancer.Run(input, 10000 % cycle);

            Assert.Equal(reference, withcycles);
        }
    }
}
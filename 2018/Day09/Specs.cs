namespace AdventOfCode
{
    public class Specs
    {

        [Theory]
        [InlineData(9, 25, 32)]
        [InlineData(10, 1618, 8317)]
        [InlineData(13, 7999, 146373)]
        [InlineData(17, 1104, 2764)]
        [InlineData(21, 6111, 54718)]
        [InlineData(30, 5807, 37305)]
        public void TestPart1(int players, int marbles, int expected)
        {
            var result = AoC.Part1(players, marbles);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Game_FirstMarble_SetsCurrent()
        {
            var game = new Game(9);
            Assert.Equal(0, game.CurrentMarble);
        }

        [Fact]
        public void Game_CyclesPlayers()
        {
            var game = new Game(3);
            game.Play(4);
            Assert.Equal(1, game.CurrentPlayer);
        }

        [Fact]
        public void Game_SecondMarble_SetsCurrent()
        {
            var game = new Game(9);
            game.Play(1);
            Assert.Equal(1, game.CurrentMarble);
        }
        [Fact]
        public void Game_ThirdMarble_SetsCurrent()
        {
            var game = new Game(9);
            game.Play(2);
            Assert.Equal(2, game.CurrentMarble);
        }
        [Fact]
        public void Game_ThirdMarble_AddedBetweenOneAndTwo()
        {
            var game = new Game(9);
            game.Play(2);
            Assert.Equal(2, game.CurrentPlayer);
        }
        [Fact]
        public void Game_NineMarbles_AddedBetweenOneAndTwo()
        {
            var game = new Game(9);
            game.Play(9);
            Assert.Equal(9, game.CurrentMarble);
        }

        [Fact]
        public void Game_TenMarbles_AddedBetweenOneAndTwo()
        {
            var game = new Game(9);
            game.Play(10);
            Assert.Equal(10, game.CurrentMarble);
            Assert.Equal(1, game.CurrentPlayer);
        }

        [Fact]
        public void Game_Marble23()
        {
            var game = new Game(9);
            game.Play(23);
            Assert.Equal(19, game.CurrentMarble);
            Assert.Equal(32, game[5]);
        }
        [Fact]
        public void Game_Marble24()
        {
            var game = new Game(9);
            game.Play(24);
            Assert.Equal(24, game.CurrentMarble);
            Assert.Equal(32, game[5]);
        }

        [Fact]
        public void Game_CanPlayInDifferentStages()
        {
            var game = new Game(9);
            game.Play(10);
            game.Play(13);
            game.Play(1);
            Assert.Equal(24, game.CurrentMarble);
        }

        [Fact]
        public void TestPart2()
        {
        }
    }
}

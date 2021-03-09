namespace Flow.Tests
{
    using Core;
    using Shouldly;
    using Xunit;

    public class ChainTests
    {
        [Fact]
        public void ShouldProvideDefaultLinkLink()
        {
            // Given

            // When
            var chain = new Chain<string>("base");

            // Then
            chain.Current.ShouldBe("base");
        }

        [Fact]
        public void ShouldAppendLink()
        { 
            // Given
            var chain = new Chain<string>("base");

            // When
            chain.Append("1");

            // Then
            chain.Current.ShouldBe("1");
        }

        [Fact]
        public void ShouldRemoveLink()
        {
            // Given
            var chain = new Chain<string>("base");
            var token = chain.Append("1");

            // When
            token.Dispose();

            // Then
            chain.Current.ShouldBe("base");
        }

        [Fact]
        public void ShouldAppendLinks()
        {
            // Given
            var chain = new Chain<string>("base");

            // When
            chain.Append("1");
            chain.Append("2");
            chain.Append("3");

            // Then
            chain.Current.ShouldBe("3");
        }

        [Fact]
        public void ShouldRemoveLinks()
        {
            // Given
            var chain = new Chain<string>("base");
            var token1 = chain.Append("1");
            var token2 = chain.Append("2");
            var token3 = chain.Append("3");

            // When
            token3.Dispose();
            chain.Current.ShouldBe("2");

            token2.Dispose();
            chain.Current.ShouldBe("1");

            token1.Dispose();
            chain.Current.ShouldBe("base");

            token1.Dispose();
            // Then
        }
    }
}

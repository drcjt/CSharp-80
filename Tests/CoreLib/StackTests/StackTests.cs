using Xunit;

namespace System.Collections.Tests
{
    public static class StackTests
    {
        [Fact]
        public static void Ctor_Empty()
        {
            var stack = new Stack();
            Assert.Equal(0, stack.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public static void Ctor_Int(int initialCapacity)
        {
            var stack = new Stack(initialCapacity);
            Assert.Equal(0, stack.Count);
        }

        [Fact]
        public static void Contains()
        {
            Stack stack = Helpers.CreateIntStack(100);

            for (int i = 0; i < stack.Count; i++)
            {
                Assert.True(stack.Contains(i));
            }

            Assert.False(stack.Contains(101));
            Assert.False(stack.Contains("hello"));
            Assert.False(stack.Contains(null));

            stack.Push(null);
            Assert.True(stack.Contains(null));

            Assert.False(stack.Contains(-1));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public static void GetEnumerator(int count)
        {
            Stack stack = Helpers.CreateIntStack(count);

            Assert.NotSame(stack.GetEnumerator(), stack.GetEnumerator());
            IEnumerator enumerator = stack.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (enumerator.MoveNext())
                {
                    counter++;
                    Assert.NotNull(enumerator.Current);
                }
                Assert.Equal(count, counter);
                enumerator.Reset();
            }
        }

        [Fact]
        public static void Peek()
        {
            int count = 100;
            Stack stack = Helpers.CreateIntStack(count);

            for (int i = 0; i < count; i++)
            {
                int peek1 = (int)stack.Peek()!;

                Assert.Equal(stack.Pop(), peek1);
                Assert.Equal(count - i - 1, peek1);
            }
        }

        [Fact]
        public static void Peek_EmptyStack_ThrowsInvalidOperationException()
        {
            bool exceptionThrown = false;
            var stack = new Stack(100);
            try
            {
                stack.Peek();
            }
            catch (InvalidOperationException) 
            {
                exceptionThrown = true;
            }
            Assert.True(exceptionThrown);

            for (int i = 0; i < stack.Count; i++)
            {
                stack.Push(i);
            }

            for (int i = 0; i < stack.Count; i++)
            {
                stack.Pop();
            }

            exceptionThrown = false;
            try
            {
                stack.Peek();
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }
            Assert.True(exceptionThrown);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(10, 10)]
        [InlineData(100, 100)]
        [InlineData(10, 5)]
        [InlineData(100, 50)]
        public static void Pop(int pushCount, int popCount)
        {
            Stack stack = Helpers.CreateIntStack(pushCount);
            for (int i = 0; i < popCount; i++)
            {
                Assert.Equal(pushCount - i - 1, stack.Pop());
                Assert.Equal(pushCount - i - 1, stack.Count);
            }
            Assert.Equal(pushCount - popCount, stack.Count);
        }

        [Fact]
        public static void Pop_Null()
        {
            var stack = new Stack();

            stack.Push(null);
            stack.Push(-1);
            stack.Push(null);

            Assert.Null(stack.Pop());
            Assert.Equal(-1, stack.Pop());
            Assert.Null(stack.Pop());
        }

        [Fact]
        public static void Pop_EmptyStack_ThrowsInvalidOperationException()
        {
            bool exceptionThrown = false;
            var stack = new Stack();
            try
            {
                stack.Pop();
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }
            Assert.True(exceptionThrown);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public static void Push(int count)
        {
            var stack = new Stack();
            for (int i = 0; i < count; i++)
            {
                stack.Push(i);
                Assert.Equal(i + 1, stack.Count);
            }
            Assert.Equal(count, stack.Count);
        }


        [Fact]
        public static void Push_Null()
        {
            var stack = new Stack();

            stack.Push(null);
            stack.Push(-1);
            stack.Push(null);

            Assert.True(stack.Contains(null));
            Assert.True(stack.Contains(-1));
        }
    }
}

namespace Common.FluentChain
{
    class Combined<A, B, C> : IProcessor<A, C>
    {
        readonly IProcessor<A, B> first;
        readonly IProcessor<B, C> second;

        public Combined(IProcessor<A, B> first, IProcessor<B, C> second)
        {
            this.first = first;
            this.second = second;
        }

        public C Process(A input)
        {
            return this.second.Process(this.first.Process(input));
        }
    }
}
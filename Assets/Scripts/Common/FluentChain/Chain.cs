namespace Common.FluentChain
{

    public class Chain<TIn, TOut> : IProcessor<TIn, TOut>
    {
        private readonly IProcessor<TIn, TOut> _processor;

        public Chain(IProcessor<TIn, TOut> processor)
        {
            _processor = processor;
        }

        public static Chain<TIn, TOut> Start(IProcessor<TIn, TOut> processor)
        {
            return new Chain<TIn, TOut>(processor);
        }

        public Chain<TIn, TNext> Then<TNext>(IProcessor<TOut, TNext> next)
        {
            return new Chain<TIn, TNext>(
                new Combined<TIn, TOut, TNext>(_processor, next)
            );
        }

        public TOut Process(TIn input)
        {
            return _processor.Process(input);
        }

        public TOut Run(TIn input)
        {
            return _processor.Process(input);
        }
        
        public ProcessorDelegate<TIn, TOut> Compile() => input => _processor.Process(input); 
        
    }

    
    public delegate TChain ChainFactory<out TIn, in TOut, out TChain>(IProcessor<TIn, TOut> processor)
        where TChain : FluentChain<TIn, TOut, TChain>;
}
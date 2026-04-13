using System;
namespace Common.FluentChain
{
    public abstract class FluentChain<TIn, TOut, TDerived> where TDerived : FluentChain<TIn, TOut, TDerived>
    {
        public IProcessor<TIn, TOut> processor;

        protected FluentChain(IProcessor<TIn, TOut> processor)
        {
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        protected TNextSelf Then<TNext, TNextSelf, TProcessor>(
            TProcessor nextProcessor,
            ChainFactory<TIn, TNext, TNextSelf> factory)
            where TNextSelf : FluentChain<TIn, TNext, TNextSelf>
            where TProcessor : class, IProcessor<TOut, TNext>
        {
            if (nextProcessor == null) throw new ArgumentNullException(nameof(nextProcessor));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            return factory(new Combined<TIn, TOut, TNext>(this.processor, nextProcessor));
        }

        public TOut Run(TIn input)
        {
            if (this.processor == null) throw new InvalidOperationException("Processor is not initialized. Use Chain.Start() to begin a chain.");
            return this.processor.Process(input);
        }

        public ProcessorDelegate<TIn, TOut> Compile()
        {
            if (this.processor == null) throw new InvalidOperationException("Processor is not initialized. Use Chain.Start() to begin a chain.");
            return input => this.processor.Process(input);
        }
    }
}
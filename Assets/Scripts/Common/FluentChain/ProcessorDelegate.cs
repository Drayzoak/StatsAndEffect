namespace Common.FluentChain
{
    public delegate TOut ProcessorDelegate<in TIn, out TOut>(TIn input);
}
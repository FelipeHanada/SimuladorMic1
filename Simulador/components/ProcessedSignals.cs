using Simulador.utils;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Simulador.components;

public class ProcessedSignalSender : ISignalSender
{
    private readonly ISignalSender _source;
    private readonly Func<BitArray, BitArray> _func;

    public event EventHandler<BitArray> SignalChanged = delegate { };

    public ProcessedSignalSender(ISignalSender source, Func<BitArray, BitArray> func)
    {
        _source = source;
        _func = func;
        _source.SignalChanged += OnSignalChange;
    }

    public int Length => Signal().Length;

    public BitArray Signal()
    {
        return _func(_source.Signal());
    }

    private void OnSignalChange(object? sender, BitArray _)
    {
        SignalChanged?.Invoke(sender, _func(_source.Signal()));
    }

    public static ISignalSender Interval(ISignalSender source, int offset, int length)
    {
        BitArray func(BitArray signal)
        {
            var result = new BitArray(length);
            for (int i = 0; i < length && (i + offset) < signal.Length; i++)
            {
                result[i] = signal[i + offset];
            }
            return result;
        }

        return new ProcessedSignalSender(source, func);
    }

    public static ISignalSender Increment(ISignalSender source, int increment = 1)
    {
        BitArray func(BitArray signal)
        {
            int[] temp = new int[1];
            signal.CopyTo(temp, 0);
            int value = temp[0] + increment;

            var incBits = new BitArray([value]);
            var result = new BitArray(signal.Length);
            for (int i = 0; i < signal.Length && i < incBits.Length; i++)
            {
                result[i] = incBits[i];
            }

            return result;
        }

        return new ProcessedSignalSender(source, func);
    }

    public static ISignalSender Decoder4to16(ISignalSender source)
    {
        static BitArray func(BitArray input)
        {
            // Garante que temos pelo menos 4 bits de entrada
            int value = 0;
            for (int i = 0; i < 4 && i < input.Length; i++)
            {
                if (input[i])
                    value |= (1 << i);
            }

            var result = new BitArray(16, false);
            if (value >= 0 && value < 16)
                result[value] = true;

            return result;
        }

        return new ProcessedSignalSender(source, func);
    }
}

public class CombinationalSignalSender : ISignalSender
{
    private readonly ISignalSender[] _source;
    private readonly Func<BitArray[], BitArray> _func;

    public event EventHandler<BitArray> SignalChanged = delegate { };

    public CombinationalSignalSender(ISignalSender[] sources, Func<BitArray[], BitArray> func)
    {
        _source = sources;
        _func = func;
        foreach (var src in _source)
        {
            src.SignalChanged += OnSignalChange;
        }
    }

    public int Length => Signal().Length;

    public BitArray Signal()
    {
        var signals = new BitArray[_source.Length];
        for (int i = 0; i < _source.Length; i++)
            signals[i] = _source[i].Signal();
        return _func(signals);
    }

    private void OnSignalChange(object? sender, BitArray _)
    {
        var signals = new BitArray[_source.Length];
        for (int i = 0; i < _source.Length; i++)
            signals[i] = _source[i].Signal();
        SignalChanged?.Invoke(this, _func(signals));
    }

    public static CombinationalSignalSender And(ISignalSender[] sources)
    {
        return new CombinationalSignalSender(
            sources,
            (signals) =>
            {
                if (signals.Length == 0)
                {
                    return new BitArray(0);
                }

                int minLength = int.MaxValue;
                foreach (var s in signals)
                    if (s.Length < minLength)
                        minLength = s.Length;

                var result = new BitArray(signals[0]);
                result.Length = minLength;
                for (int i = 1; i < signals.Length; i++)
                {
                    var other = new BitArray(signals[i]);
                    other.Length = minLength;
                    result = result.And(other);
                }
                return result;
            }
        );
    }
}

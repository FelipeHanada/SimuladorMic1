using Simulador.utils;
using System;
using System.Collections;

namespace Simulador.components;

public class Alu
{
    private static Func<BitArray, BitArray, BitArray>[] functions = [ Sum, And, Identity, Inverse ];

    private readonly ISignalSender _inA;
    private readonly ISignalSender _inB;
    private readonly ISignalSender _inControl;

    private readonly SignalSender _out;
    private readonly SingleSignalSender _outN;
    private readonly SingleSignalSender _outZ;

    public ISignalSender Out { get => _out; }
    public ISignalSender OutN { get => _outN; }
    public ISignalSender OutZ { get => _outZ; }

    public Alu(ISignalSender inA, ISignalSender inB, ISignalSender inControl)
    {
        if (inA.Signal().Length != inB.Signal().Length)
            throw new ArgumentException("Input signals must be of the same size.");

        _inA = inA;
        _inB = inB;
        _inControl = inControl;

        _inA.SignalChanged += Update;
        _inB.SignalChanged += Update;
        _inControl.SignalChanged += Update;

        _out = new SignalSender(_inA.Signal().Length);
        _outN = new SingleSignalSender();
        _outZ = new SingleSignalSender();
    }

    private void Update(object? sender, BitArray _)
    {
        BitArray result = functions[_inControl.Signal().ToInt32()].Invoke(_inA.Signal(), _inB.Signal());
        _out.SetData(result);
        _outN.SetEnable(result.Get(result.Length - 1));
        _outZ.SetEnable(!result.HasAnySet());
    }

    private static BitArray Sum(BitArray a, BitArray b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("BitArrays must be of the same length.");

        BitArray result = new(a.Length);
        bool carry = false;

        for (int i = 0; i < a.Length; i++)
        {
            bool bitA = a[i];
            bool bitB = b[i];
            result[i] = bitA ^ bitB ^ carry;
            carry = (bitA && bitB) || (bitA && carry) || (bitB && carry);
        }

        return result;
    }

    private static BitArray And(BitArray a, BitArray b)
    {
        BitArray result = new(a);
        result.And(b);
        return result;
    }

    private static BitArray Identity(BitArray a, BitArray b)
    {
        return new BitArray(a);
    }

    private static BitArray Inverse(BitArray a, BitArray b)
    {
        BitArray result = new(a);
        result.Not();
        return result;
    }
}

/**
 * Autogenerated by Thrift Compiler (0.11.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Thrift;
using Thrift.Collections;

using Thrift.Protocols;
using Thrift.Protocols.Entities;
using Thrift.Protocols.Utilities;
using Thrift.Transports;
using Thrift.Transports.Client;
using Thrift.Transports.Server;



/// <summary>
/// Represents a single cell and the amount to increment it by
/// </summary>
public partial class TColumnIncrement : TBase
{
  private long _amount;

  public byte[] Family { get; set; }

  public byte[] Qualifier { get; set; }

  public long Amount
  {
    get
    {
      return _amount;
    }
    set
    {
      __isset.amount = true;
      this._amount = value;
    }
  }


  public Isset __isset;
  public struct Isset
  {
    public bool amount;
  }

  public TColumnIncrement()
  {
    this._amount = 1;
    this.__isset.amount = true;
  }

  public TColumnIncrement(byte[] family, byte[] qualifier) : this()
  {
    this.Family = family;
    this.Qualifier = qualifier;
  }

  public async Task ReadAsync(TProtocol iprot, CancellationToken cancellationToken)
  {
    iprot.IncrementRecursionDepth();
    try
    {
      bool isset_family = false;
      bool isset_qualifier = false;
      TField field;
      await iprot.ReadStructBeginAsync(cancellationToken);
      while (true)
      {
        field = await iprot.ReadFieldBeginAsync(cancellationToken);
        if (field.Type == TType.Stop)
        {
          break;
        }

        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.String)
            {
              Family = await iprot.ReadBinaryAsync(cancellationToken);
              isset_family = true;
            }
            else
            {
              await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            }
            break;
          case 2:
            if (field.Type == TType.String)
            {
              Qualifier = await iprot.ReadBinaryAsync(cancellationToken);
              isset_qualifier = true;
            }
            else
            {
              await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            }
            break;
          case 3:
            if (field.Type == TType.I64)
            {
              Amount = await iprot.ReadI64Async(cancellationToken);
            }
            else
            {
              await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            }
            break;
          default: 
            await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            break;
        }

        await iprot.ReadFieldEndAsync(cancellationToken);
      }

      await iprot.ReadStructEndAsync(cancellationToken);
      if (!isset_family)
      {
        throw new TProtocolException(TProtocolException.INVALID_DATA);
      }
      if (!isset_qualifier)
      {
        throw new TProtocolException(TProtocolException.INVALID_DATA);
      }
    }
    finally
    {
      iprot.DecrementRecursionDepth();
    }
  }

  public async Task WriteAsync(TProtocol oprot, CancellationToken cancellationToken)
  {
    oprot.IncrementRecursionDepth();
    try
    {
      var struc = new TStruct("TColumnIncrement");
      await oprot.WriteStructBeginAsync(struc, cancellationToken);
      var field = new TField();
      field.Name = "family";
      field.Type = TType.String;
      field.ID = 1;
      await oprot.WriteFieldBeginAsync(field, cancellationToken);
      await oprot.WriteBinaryAsync(Family, cancellationToken);
      await oprot.WriteFieldEndAsync(cancellationToken);
      field.Name = "qualifier";
      field.Type = TType.String;
      field.ID = 2;
      await oprot.WriteFieldBeginAsync(field, cancellationToken);
      await oprot.WriteBinaryAsync(Qualifier, cancellationToken);
      await oprot.WriteFieldEndAsync(cancellationToken);
      if (__isset.amount)
      {
        field.Name = "amount";
        field.Type = TType.I64;
        field.ID = 3;
        await oprot.WriteFieldBeginAsync(field, cancellationToken);
        await oprot.WriteI64Async(Amount, cancellationToken);
        await oprot.WriteFieldEndAsync(cancellationToken);
      }
      await oprot.WriteFieldStopAsync(cancellationToken);
      await oprot.WriteStructEndAsync(cancellationToken);
    }
    finally
    {
      oprot.DecrementRecursionDepth();
    }
  }

  public override string ToString()
  {
    var sb = new StringBuilder("TColumnIncrement(");
    sb.Append(", Family: ");
    sb.Append(Family);
    sb.Append(", Qualifier: ");
    sb.Append(Qualifier);
    if (__isset.amount)
    {
      sb.Append(", Amount: ");
      sb.Append(Amount);
    }
    sb.Append(")");
    return sb.ToString();
  }
}


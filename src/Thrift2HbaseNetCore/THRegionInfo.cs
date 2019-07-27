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



public partial class THRegionInfo : TBase
{
  private byte[] _startKey;
  private byte[] _endKey;
  private bool _offline;
  private bool _split;
  private int _replicaId;

  public long RegionId { get; set; }

  public byte[] TableName { get; set; }

  public byte[] StartKey
  {
    get
    {
      return _startKey;
    }
    set
    {
      __isset.startKey = true;
      this._startKey = value;
    }
  }

  public byte[] EndKey
  {
    get
    {
      return _endKey;
    }
    set
    {
      __isset.endKey = true;
      this._endKey = value;
    }
  }

  public bool Offline
  {
    get
    {
      return _offline;
    }
    set
    {
      __isset.offline = true;
      this._offline = value;
    }
  }

  public bool Split
  {
    get
    {
      return _split;
    }
    set
    {
      __isset.split = true;
      this._split = value;
    }
  }

  public int ReplicaId
  {
    get
    {
      return _replicaId;
    }
    set
    {
      __isset.replicaId = true;
      this._replicaId = value;
    }
  }


  public Isset __isset;
  public struct Isset
  {
    public bool startKey;
    public bool endKey;
    public bool offline;
    public bool split;
    public bool replicaId;
  }

  public THRegionInfo()
  {
  }

  public THRegionInfo(long regionId, byte[] tableName) : this()
  {
    this.RegionId = regionId;
    this.TableName = tableName;
  }

  public async Task ReadAsync(TProtocol iprot, CancellationToken cancellationToken)
  {
    iprot.IncrementRecursionDepth();
    try
    {
      bool isset_regionId = false;
      bool isset_tableName = false;
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
            if (field.Type == TType.I64)
            {
              RegionId = await iprot.ReadI64Async(cancellationToken);
              isset_regionId = true;
            }
            else
            {
              await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            }
            break;
          case 2:
            if (field.Type == TType.String)
            {
              TableName = await iprot.ReadBinaryAsync(cancellationToken);
              isset_tableName = true;
            }
            else
            {
              await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            }
            break;
          case 3:
            if (field.Type == TType.String)
            {
              StartKey = await iprot.ReadBinaryAsync(cancellationToken);
            }
            else
            {
              await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            }
            break;
          case 4:
            if (field.Type == TType.String)
            {
              EndKey = await iprot.ReadBinaryAsync(cancellationToken);
            }
            else
            {
              await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            }
            break;
          case 5:
            if (field.Type == TType.Bool)
            {
              Offline = await iprot.ReadBoolAsync(cancellationToken);
            }
            else
            {
              await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            }
            break;
          case 6:
            if (field.Type == TType.Bool)
            {
              Split = await iprot.ReadBoolAsync(cancellationToken);
            }
            else
            {
              await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
            }
            break;
          case 7:
            if (field.Type == TType.I32)
            {
              ReplicaId = await iprot.ReadI32Async(cancellationToken);
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
      if (!isset_regionId)
      {
        throw new TProtocolException(TProtocolException.INVALID_DATA);
      }
      if (!isset_tableName)
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
      var struc = new TStruct("THRegionInfo");
      await oprot.WriteStructBeginAsync(struc, cancellationToken);
      var field = new TField();
      field.Name = "regionId";
      field.Type = TType.I64;
      field.ID = 1;
      await oprot.WriteFieldBeginAsync(field, cancellationToken);
      await oprot.WriteI64Async(RegionId, cancellationToken);
      await oprot.WriteFieldEndAsync(cancellationToken);
      field.Name = "tableName";
      field.Type = TType.String;
      field.ID = 2;
      await oprot.WriteFieldBeginAsync(field, cancellationToken);
      await oprot.WriteBinaryAsync(TableName, cancellationToken);
      await oprot.WriteFieldEndAsync(cancellationToken);
      if (StartKey != null && __isset.startKey)
      {
        field.Name = "startKey";
        field.Type = TType.String;
        field.ID = 3;
        await oprot.WriteFieldBeginAsync(field, cancellationToken);
        await oprot.WriteBinaryAsync(StartKey, cancellationToken);
        await oprot.WriteFieldEndAsync(cancellationToken);
      }
      if (EndKey != null && __isset.endKey)
      {
        field.Name = "endKey";
        field.Type = TType.String;
        field.ID = 4;
        await oprot.WriteFieldBeginAsync(field, cancellationToken);
        await oprot.WriteBinaryAsync(EndKey, cancellationToken);
        await oprot.WriteFieldEndAsync(cancellationToken);
      }
      if (__isset.offline)
      {
        field.Name = "offline";
        field.Type = TType.Bool;
        field.ID = 5;
        await oprot.WriteFieldBeginAsync(field, cancellationToken);
        await oprot.WriteBoolAsync(Offline, cancellationToken);
        await oprot.WriteFieldEndAsync(cancellationToken);
      }
      if (__isset.split)
      {
        field.Name = "split";
        field.Type = TType.Bool;
        field.ID = 6;
        await oprot.WriteFieldBeginAsync(field, cancellationToken);
        await oprot.WriteBoolAsync(Split, cancellationToken);
        await oprot.WriteFieldEndAsync(cancellationToken);
      }
      if (__isset.replicaId)
      {
        field.Name = "replicaId";
        field.Type = TType.I32;
        field.ID = 7;
        await oprot.WriteFieldBeginAsync(field, cancellationToken);
        await oprot.WriteI32Async(ReplicaId, cancellationToken);
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
    var sb = new StringBuilder("THRegionInfo(");
    sb.Append(", RegionId: ");
    sb.Append(RegionId);
    sb.Append(", TableName: ");
    sb.Append(TableName);
    if (StartKey != null && __isset.startKey)
    {
      sb.Append(", StartKey: ");
      sb.Append(StartKey);
    }
    if (EndKey != null && __isset.endKey)
    {
      sb.Append(", EndKey: ");
      sb.Append(EndKey);
    }
    if (__isset.offline)
    {
      sb.Append(", Offline: ");
      sb.Append(Offline);
    }
    if (__isset.split)
    {
      sb.Append(", Split: ");
      sb.Append(Split);
    }
    if (__isset.replicaId)
    {
      sb.Append(", ReplicaId: ");
      sb.Append(ReplicaId);
    }
    sb.Append(")");
    return sb.ToString();
  }
}


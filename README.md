# Dotnet core read/write hbase by thrift

## Preparation for hbase

hbase version=1.2.8

```bash
cd $HBASE_HOME
./bin/hbase thrift -b HadoopMaster -p 9090 start
```

## Gen Hbase.thrift

thrift version=0.11.0

```bash
thrift --gen netcore Hbase.thrift
```

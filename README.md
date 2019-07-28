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

## 特性

### 自动映射

类 `Utilities.Parsers.HbaseParser` 带有将自定义类映射为 `habse` 类 `Mutation` 的功能，同时也带有将 `hbase` 类 `TRowResult` 映射到自定义类的功能。只需在类的属性上添加特性 `Utilities.Attributes.HbaseColumnAttribute`。测试[HbaseParserTest](test/HbaseNetCore/HbaseParserTest.cs)可作为数据转换示例，完整 Hbase 读写转换可在[HbaseNetCoreTest](test/HbaseNetCore/HbaseNetCoreTest.cs)中找到。

！！！ 在自定义类中，复杂的属性或字段类型值将被转换为 Json 字符串进行 Hbase 的存储。

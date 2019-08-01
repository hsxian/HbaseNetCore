using System.Collections.Generic;
using System.Threading.Tasks;

namespace HbaseNetCore.Interfaces
{
    public interface IHbaseParser
    {
        List<Mutation> ToMutations<T>(T obj) where T : class;
        BatchMutation ToBatchMutation<T>(T obj) where T : class, IHbaseTable;
        List<BatchMutation> ToBatchMutations<T>(IEnumerable<T> objs) where T : class, IHbaseTable;
        T ToReal<T>(TRowResult trr) where T : class, IHbaseTable, new();
        List<T> ToReals<T>(IEnumerable<TRowResult> trrs) where T : class, IHbaseTable, new();
    }
}
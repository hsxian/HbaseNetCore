using System.Collections.Generic;
using System.Threading.Tasks;

namespace HbaseNetCore.Interfaces
{
    public interface IHbaseParser
    {
        Task<List<Mutation>> ToMutationsAsync<T>(T obj) where T : class;
        Task<BatchMutation> ToBatchMutationAsync<T>(T obj) where T : class, IHbaseTable;
        Task<T> ToRealAsync<T>(TRowResult trr) where T : class, new();

    }
}
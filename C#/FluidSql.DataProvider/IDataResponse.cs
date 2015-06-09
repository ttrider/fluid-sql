using System.Collections.Generic;

namespace TTRider.FluidSql.DataProvider
{
    public interface IDataResponse
    {
        IDataRequest Request { get; }

        IEnumerable<object[]> Records { get; }

        IDictionary<string, object> Output { get; }

        int? ReturnCode { get; }

        bool HasMoreData { get; }
    }
}

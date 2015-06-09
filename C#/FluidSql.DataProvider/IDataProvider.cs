namespace TTRider.FluidSql.DataProvider
{
    public interface IDataProvider
    {
        IDataResponse ProcessRequest(IDataRequest request);
    }
}

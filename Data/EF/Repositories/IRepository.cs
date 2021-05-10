namespace Data.EF.Repositories
{
    public interface IRepository
    {
        void ExecuteSqlScript(string sql);
    }
}
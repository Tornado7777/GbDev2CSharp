namespace CloneHabrService.Services
{
    public interface IRepository<T, TId>
    {
        IList<T> GetAll();

        T GetById(TId id);

        int Create(T data);

        void Update(T data);

        void Delete(TId id);
    }
}

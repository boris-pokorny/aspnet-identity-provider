namespace Persistence.Mappers;

public interface IMapper<TSource, TDestination>
{
    void Map(TSource source, TDestination destination);
}
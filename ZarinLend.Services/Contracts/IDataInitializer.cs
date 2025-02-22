using Common;

namespace Services.Contracts
{
    public interface IDataInitializer : IScopedDependency
    {
        int Order { get; }
        void InitializeData();
    }
}

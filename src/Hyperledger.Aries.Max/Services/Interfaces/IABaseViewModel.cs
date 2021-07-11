using System.Threading.Tasks;

namespace Hyperledger.Aries.Max.Services.Interfaces
{
    public interface IABaseViewModel
    {
        string Name { get; set; }

        string Title { get; set; }

        bool IsBusy { get; set; }

        Task InitializeAsync(object navigationData);
    }
}

using System.Windows.Input;

namespace Hyperledger.Aries.Max.ViewModels.Connections
{
    public class TransactionItem
    {
        public string Type { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string PrimaryActionTitle { get; set; }

        public string SecondaryActionTitle { get; set; }

        public ICommand PrimaryActionCommand { get; set; }

        public ICommand SecondaryActionCommand { get; set; }
    }
}

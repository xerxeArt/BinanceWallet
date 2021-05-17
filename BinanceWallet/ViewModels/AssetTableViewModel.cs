using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinanceWallet.ViewModels
{
    public class AssetTableViewModel
    {
        public string TableId { get; set; }
        public IEnumerable<AssetInventoryInformationViewModel> Assets { get; set; }
    }
}

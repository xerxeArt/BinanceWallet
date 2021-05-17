using BinanceWallet.ViewModels;
using Data.Business;
using Data.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BinanceWallet.Controllers
{
    public class PortfolioController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IBusinessFlow _businessFlow;

        public PortfolioController(ILogger<HomeController> logger, IBusinessFlow businessFlow)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _businessFlow = businessFlow ?? throw new ArgumentNullException(nameof(businessFlow));
        }

        [HttpGet]
        public IActionResult GetAssets()
        {
            var assets = _businessFlow.GetStoredPrices();
            var model = ConvertFromAssetList(assets);

            return View("Assets", model);
        }

        [HttpGet]
        public async Task<IActionResult> InitializeWalletData()
        {
            await _businessFlow.InitializeData();
            return GetAssets();
        }

        [HttpGet]
        public async Task<IActionResult> RefreshWalletHoldings()
        {
            await _businessFlow.UpdateWalletHoldings();
            return GetAssets();
        }

        [HttpGet]
        public async Task<IActionResult> GetRefreshedAssets()
        {
            await _businessFlow.UpdateCurrentPrices();
            return GetAssets();
        }

        private AssetInventoryViewModel ConvertFromAssetList(List<Asset> assets)
        {
            decimal btcEur, btcBusd;
            (btcEur, btcBusd) = _businessFlow.GetBtcFiatValues();
            var model = new AssetInventoryViewModel(btcEur, btcBusd);
            foreach (var asset in assets)
            {
                model.AddAsset(asset);
            }

            return model;
        }
    }
}


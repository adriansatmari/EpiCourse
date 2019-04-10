using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Infrastructure.Pricing
{
    public class PriceCalculator
    {
        // added
        Injected<IContentLoader> contentLoader;
        Injected<ReferenceConverter> referenceConverter;
        Injected<ICatalogSystem> catalogSystem;
        Injected<ICurrentMarket> currentMarket;

        // R/O
        private readonly IPriceService _priceService;
        // R/W
        private readonly IPriceDetailService _priceDetailService;

        private readonly ICurrentMarket _marketService;

        PriceCalculator(IPriceService priceService
            , ICurrentMarket marketService
            , IPriceDetailService priceDetailService)
        {
            _priceService = priceService;
            _marketService = marketService;
            _priceDetailService = priceDetailService;
        }


        private void CheckPricingLoaders(ContentReference contentReference, CustomerPricing customerPricing)
        {
            // The one to use when using "custom" price-types
            PricingLoader RwLoader = new PricingLoader(
                contentLoader.Service
                , _priceDetailService // Note: PriceDetailService
                , referenceConverter.Service
                , catalogSystem.Service);

            // Note: the customerPricing arg. handles custom price-types 
            ItemCollection<PriceDetail> prices = RwLoader.GetPrices
                (contentReference, MarketId.Default, customerPricing);

            // This one may not be the best choice, it's from 7.5/R3
            // For non-complex scenarios it may be sufficient, or if the inconsistent behaviour is benneficial
            // Several overloads are marked...
            // "Obsolete" - "Use the constructor with currentMarketService instead."
            ReadOnlyPricingLoader RoLoader = new ReadOnlyPricingLoader( //  first overload obsoleted in 9
                contentLoader.Service
                , _priceService // Note: PriceService
                , referenceConverter.Service
                , catalogSystem.Service
                , currentMarket.Service // added
                                        //, Mediachase.Commerce.Security.SecurityContext.Current // Obsoleted
                , CustomerContext.Current // added
                                          //, FrameworkContext.Current // Obsoleted
                ); // added

            ItemCollection<Price> custPrices = RoLoader.GetCustomerPrices(contentReference);


        }

        // Demo - Fund
        public void CheckPrices(EntryContentBase CurrentContent)
        {
            // Pricing, a long story with many alternative

            /*
             * Two services are available for interacting with price data. 
             *  - The IPriceService API is typically used by order processing to fetch prices for actual use
             *  - IPriceDetailService is typically used in "integration" and interface/code for display (all prices) and edit prices. 
             
             The difference between these APIs is that the IPriceService works with optimized sets of price data, 
             * while the IPriceDetailService works with "the truth". 
             * The optimization in the IPriceService 
             * removes prices that will cannot be used, and trims or splits prices that overlap. 
             */

            #region StoreHelper...does this

            /*Steps in StoreHelper
             *
             * Check CurrentMarket
             * Add AllCustomers
             * Add PriceType.USerName (IPrincipal)
             * Check Cust-Group, add ...Effiective-Cust-Group
             * Get the service
             * Set the filter
             * Fetch ...  priceService.GetPrices + get the cheapest (note: always cheapest we want)
             * Get a ... IPriceValue and then...
             * ....return new Price(priceValue.UnitPrice);
             *  
             * ...the one that does the job 
             * public static Price GetSalePrice(Entry entry, decimal quantity, IMarket market, Currency currency)
             * 
             */
            #endregion

            // ... look in VariantController for nice extensions

            #region GetDefaultPrice...does this

            /*
             Could have a look at .GetDefaultPrice in Reflector
             PricingExtensions (EPiServer.Commerce.Catalog.ContentTypes)
              - GetDefaultPrice() // gets the R/O-loader
              - tries to figure out  the Currency
              - tries to figure out Market, DateTime, CatalogKey... and does...
              - IPriceValue GetDefaultPrice(MarketId market, DateTime validOn, CatalogKey catalogKey, Currency currency);
               ...goes to the concrete impl. för the R/O-provider and in there...creates a PriceFilter 
               ....and gets a "default price" for a catalog entry. The "default price" for a 
                    ....market, currency, and catalog entry is the price available to 
                    "all customers" at a minimum quantity of 0.
                ...that in turn goes to the Provider and say GetPrices() ... with all the stuff found
             */

            #endregion


            // PriceDetails ... administrative stuff, don´t use for retrieving prices for the web
            // 3 overloads... the 3:rd with filter & Market
            List<IPriceDetailValue> priceList1 =
                _priceDetailService.List(CurrentContent.ContentLink).ToList();
            Price ppp = new Price(priceList1.First());

            // this price class takes a IPriceValue in .ctor (there are 3 different Price-classes)
            EPiServer.Commerce.SpecializedProperties.Price aPrice =
                new EPiServer.Commerce.SpecializedProperties.Price(priceList1.FirstOrDefault()); // just checking
            // ...have a CreatePrice("theCode"); // further below


            /* dbo.PriceType.sql
                0	All Customers
                1	Customer
                2	Customer Price Group
                ...can add custom PriceTypes
            */
            // PriceDetail - "R/W"
            // PriceValue, PriceGroup - "R/O" (...but it´s not, did write to it)
            // Replicate...changes between the two services

            // !! Note: don't get it by the ServiceLocator, use injected or .ctor-injection !!
            // DB-side paging, GetPrices() takes start-count, is R/O ... it´s a Loader
            PricingLoader detailedPricingLoader = ServiceLocator.Current.GetInstance<PricingLoader>();
            // detailedPricingLoader.GetPrices()

            // Good set of methods, no paging (GetChildrenPrices-obsoleted), gets allmost all
            ReadOnlyPricingLoader readOnlyPricingLoader = ServiceLocator.Current.GetInstance<ReadOnlyPricingLoader>();

            // detailedPricingLoader. as the service
            //readOnlyPricingLoader.GetChildrenPrices(...) // deprecated
            var p = readOnlyPricingLoader.GetDefaultPrice(CurrentContent.ContentLink);
            // could use this "loader" on the front-end...instead of the service

            // PriceService R/O-pricing ("the optimized service" (from the good old R3-era ´:`))
            // ... that´s why we have some confusing stuff left in the system (it entered here before the "Content-Model" was in place)
            CatalogKey catKey = new CatalogKey(CurrentContent.Code); // Catalogkey... example of legacy
            // Note: CatalogKey is gone when using the new Inventory system. There you can go by "code"
            // ...will probably file a blemish-bug on this

            // return full sets of price data for the specified catalog entries?
            // ...also takes an Enumerable 
            var pricesByKey = _priceService.GetCatalogEntryPrices(catKey); // need catalogKey ( or IEnumerable of those )
            var priceByDefault = _priceService.GetDefaultPrice(// Market, time, CatKey & Currency
                currentMarket.Service.GetCurrentMarket().MarketId.Value
                , DateTime.UtcNow
                , catKey
                , new Currency("usd"));


            //priceService.GetPrices(  //...3:rd overload takes an IEnumerable of CatalogKeyAndQuantity
            // The GetPrices methods return filtered data for the specified catalog entries. Returned price values 
            // ...will match all specified The CatalogKeyAndQuantity class may be used to get prices 
            // ...for multiple entries, with different quantities for each, in a single request

            // for the R/O-PriceService
            CatalogKeyAndQuantity keyAndQtyExample =
                new CatalogKeyAndQuantity(catKey, 12);
            // second as an IEnumerable of keys
            // third as an IEnumerable of "KeysAndQty"
            //_priceService.GetPrices();

            // If custom built "Optimized"...
            // ...then need a mechanism for synchronizing with a custom detail service, then it must call 
            // IPriceDetailService.ReplicatePriceServiceChanges on all edits to update the optimized data store

            #region ... not much of a demo - PrintToPage and Housekeeping

            //Response.Write("<br/>");
            //Response.Write("ContentTypeID" + CurrentContent.ContentTypeID + "<br/>");
            //Response.Write("PriceRef.ID: " + CurrentContent.PriceReference.ID + "<br/>");

            //Response.Write("PriceDetails <br/>");
            //p1.ForEach(p => Response.Write("UnitPrice: " + p.UnitPrice.Amount.ToString() + "<br/>"));
            //Response.Write("PriceValueId: " + p1.FirstOrDefault().PriceValueId + "<br/>");
            //Response.Write("UnitPrice: " + p1.FirstOrDefault().UnitPrice.Amount.ToString() + "<br/>");

            //Response.Write("<br/>");
            //Response.Write("PriceService <br/>"); //  + "<br/>"
            //p2.ToList().ForEach(p => Response.Write("UnitPrice: " + p.UnitPrice.Amount.ToString() + "<br/>"));

            //Response.Write("<br/>");



            //// node
            //Response.Write("PriceList at ParentNode: " + contentLoader.Get<IContent>(CurrentContent.ParentLink).Name + "<br/>");
            //List<IPriceDetailValue> listFromNode = priceDetails.List(CurrentContent.ParentLink).ToList();
            //listFromNode.ForEach(nl => Response.Write(
            //    "CatalogKey: " + nl.CatalogKey.CatalogEntryCode
            //        + " :: " +
            //    "UnitPrice: " + nl.UnitPrice.Amount.ToString() + "<br/>"));

            //Response.Write("<br/>");

            // EPiServer.Commerce.Catalog.ContentTypes.PricingExtensions
            // ...getPrices() ...  by the "ReadOnlyPricingLoader" 
            // Below, get all prices
            //EPiServer.Commerce.SpecializedProperties.ItemCollection
            //    <EPiServer.Commerce.SpecializedProperties.Price>
            //    prices = CurrentContent.GetPrices(); // 5 overloads, the third takes "Market" and "CustomerPricing"

            //var pp = CurrentContent.GetPrices(MarketId.Default, new CustomerPricing(CustomerPricing.PriceType.PriceGroup, "SpecialFriends"));
            //// PriceType & PriceCode in API , SaleType & SaleCode in UI 
            //// should have the qty here ... ?
            //Response.Write("By Extesion methods <br/>");
            //prices.ToList().ForEach(p => Response.Write(p.UnitPrice.Amount + "<br/>"));
            //pp.ToList().ForEach(p => Response.Write("SpecialFriends: " + p.UnitPrice.Amount.ToString() + "<br/>"));

            #endregion


            /* Price Filter - good stuff */

            List<Currency> currencies = new List<Currency>();
            List<CustomerPricing> custprices = new List<CustomerPricing>();

            // SaleCode (UI) or PriceGroup (code) (string) ... the Cust-Group
            // CM / CMS UI: SaleType - SaleCode
            // API: PriceType, PriceCode (string)

            PriceFilter filter = new PriceFilter()
            {
                Quantity = 2,
                Currencies = new Currency[] { "USD", "SEK" },
                CustomerPricing = new CustomerPricing[]
                {
                    new CustomerPricing(CustomerPricing.PriceType.AllCustomers,null),
                    new CustomerPricing(CustomerPricing.PriceType.UserName,"Kalle"),
                    new CustomerPricing(CustomerPricing.PriceType.PriceGroup, "MyBuddies") // or several...
                    // may also want to add the personal account and/or custom price-types
                },
                ReturnCustomerPricing = false // ...see below for info
                                              // interpretation of the CustomerPricing property... if true; gets all that applies



            };

            #region Info ReturnCustomerPricing

            /* The ReturnCustomerPricing property controls the interpretation of the CustomerPricing property. 
             * If the value of this property is false, and multiple price values that are identical except for the customer pricing 
             * (but both match the prices targeted by the method call) could be returned, then only the entry in that grouping with 
             * the lowest price will be returned (this is the more common use case). If the value of this property is true, 
             * then all prices will be returned individually. The default value is false. As an example, 
             *  suppose a catalog entry has a price of $10.00 for all customers, and $9.00 for one particular customer. 
             *  A call to a GetPrices method that would match both prices, and has ReturnCustomerPricing set to false, 
             *  would only get the $9.00 price in the result set. If ReturnCustomerPricing was set to true for the same call, 
             *  both the $9.00 and $10.00 price would be returned. */

            #endregion

            // The rest needed, CatKey, Market, TimeStamp
            CatalogKey catalogKey = new CatalogKey(CurrentContent.Code); // 4 overloads App-ID is gone

            IEnumerable<IPriceValue> priceValues = _priceService.GetPrices( // overloaded - one or more CatKey or CatalogKeyAndQuantity
                MarketId.Default, FrameworkContext.Current.CurrentDateTime
                , catalogKey, filter);

            Price pp = new Price(priceValues.First());
            PriceValue pv2 = new PriceValue(priceValues.First());

            // ppp is a Price (above), created from an IPriceDetailValue
            IPriceValue PV3 = ppp.ToPriceValue(); // 
            // just checking
            decimal onePrice = priceValues.FirstOrDefault().UnitPrice.Amount;
        }

        // no demo
        public IEnumerable<IPriceValue> GetPrices(string code)
        {
            List<IPriceValue> priceList = new List<IPriceValue>();
            IMarket market = _marketService.GetCurrentMarket(); // DEFAULT

            if (String.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException("Code is needed");
            }

            // need the key 
            var catalogKey = new CatalogKey(code);

            priceList = _priceService.GetCatalogEntryPrices(catalogKey).ToList();

            // just checking
            var p = new PriceValue();
            p.UnitPrice = new Money(99, "USD");
            p.MinQuantity = 2;
            priceList.Add((IPriceValue)p);

            return priceList;
        }

        public void CreatePrice(string code)
        {
            // ...need to read out what we have and add to the list
            List<IPriceDetailValue> newPrices = new List<IPriceDetailValue>();

            var priceDetailValue = new PriceDetailValue
            {
                CatalogKey = new CatalogKey(code),
                MarketId = new MarketId("DEFAULT"),
                CustomerPricing = CustomerPricing.AllCustomers,
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidUntil = DateTime.UtcNow.AddYears(1),
                MinQuantity = 5m,
                UnitPrice = new Money(95m, Currency.USD)
            };

            newPrices.Add(priceDetailValue);

            _priceDetailService.Save(newPrices);

        }

    }
}
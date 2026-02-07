using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EarthUnderFreelancer.Economy
{
    /// <summary>
    /// Auction House System for player-to-player trading
    /// Similar to WoW's Auction House with buy, sell, and bidding
    /// </summary>
    public class AuctionHouseSystem : MonoBehaviour
    {
        public static AuctionHouseSystem Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private float auctionCutPercent = 5f; // Platform fee
        [SerializeField] private float depositPercent = 5f; // Refunded on sale
        [SerializeField] private int maxActiveAuctions = 50;
        [SerializeField] private float[] auctionDurations = { 12f, 24f, 48f }; // hours

        public List<AuctionListing> AllListings { get; private set; }
        public List<AuctionListing> MyListings { get; private set; }
        public List<AuctionBid> MyBids { get; private set; }

        public event Action<AuctionListing> OnListingCreated;
        public event Action<AuctionListing> OnListingPurchased;
        public event Action<AuctionListing, AuctionBid> OnBidPlaced;
        public event Action<AuctionListing> OnAuctionEnded;
        public event Action<AuctionListing> OnOutbid;

        #region Data Structures

        [Serializable]
        public class AuctionListing
        {
            public string listingId;
            public string sellerId;
            public string sellerName;
            
            // Item
            public string itemId;
            public string itemName;
            public string itemDescription;
            public ItemCategory category;
            public ItemRarity rarity;
            public int quantity;
            public string itemIcon;
            public Dictionary<string, float> itemStats;
            
            // Pricing
            public long buyoutPrice;
            public long currentBid;
            public long minBidIncrement;
            public long deposit;
            
            // Bidding
            public string highestBidderId;
            public string highestBidderName;
            public List<AuctionBid> bidHistory;
            
            // Time
            public DateTime listedAt;
            public DateTime expiresAt;
            public float durationHours;
            
            // Status
            public AuctionStatus status;
        }

        [Serializable]
        public class AuctionBid
        {
            public string bidId;
            public string bidderId;
            public string bidderName;
            public long bidAmount;
            public DateTime bidTime;
        }

        public enum AuctionStatus
        {
            Active,
            Sold,
            Expired,
            Cancelled
        }

        public enum ItemCategory
        {
            Aircraft,
            Weapons,
            Equipment,
            Materials,
            Consumables,
            Cosmetics,
            Blueprints,
            Misc
        }

        public enum ItemRarity
        {
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary
        }

        [Serializable]
        public class AuctionSearchParams
        {
            public string searchText;
            public ItemCategory? category;
            public ItemRarity? minRarity;
            public long? minPrice;
            public long? maxPrice;
            public bool buyoutOnly;
            public AuctionSortBy sortBy;
            public bool sortDescending;
        }

        public enum AuctionSortBy
        {
            TimeRemaining,
            Price,
            Rarity,
            Name,
            RecentlyListed
        }

        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            AllListings = new List<AuctionListing>();
            MyListings = new List<AuctionListing>();
            MyBids = new List<AuctionBid>();

            // Generate sample listings
            GenerateSampleListings();

            // Check for expired auctions periodically
            InvokeRepeating(nameof(ProcessExpiredAuctions), 60f, 60f);
        }

        private void GenerateSampleListings()
        {
            string[] itemNames = { "P-51D Mustang", "Bf 109 G-6", "MG 151/20 Cannon", "Engine Upgrade Kit", 
                                   "Premium Fuel", "Rare Paint Scheme", "Advanced Radar", "Repair Kit x10" };
            ItemCategory[] categories = { ItemCategory.Aircraft, ItemCategory.Aircraft, ItemCategory.Weapons,
                                          ItemCategory.Equipment, ItemCategory.Consumables, ItemCategory.Cosmetics,
                                          ItemCategory.Equipment, ItemCategory.Consumables };
            ItemRarity[] rarities = { ItemRarity.Epic, ItemRarity.Rare, ItemRarity.Rare, ItemRarity.Uncommon,
                                      ItemRarity.Common, ItemRarity.Legendary, ItemRarity.Epic, ItemRarity.Common };

            for (int i = 0; i < 20; i++)
            {
                int itemIndex = i % itemNames.Length;
                AllListings.Add(new AuctionListing
                {
                    listingId = Guid.NewGuid().ToString(),
                    sellerId = $"seller_{i}",
                    sellerName = $"Trader{i + 100}",
                    itemId = $"item_{itemIndex}",
                    itemName = itemNames[itemIndex],
                    category = categories[itemIndex],
                    rarity = rarities[itemIndex],
                    quantity = UnityEngine.Random.Range(1, 10),
                    buyoutPrice = UnityEngine.Random.Range(1000, 500000),
                    currentBid = UnityEngine.Random.Range(500, 100000),
                    minBidIncrement = 100,
                    listedAt = DateTime.UtcNow.AddHours(-UnityEngine.Random.Range(1, 24)),
                    expiresAt = DateTime.UtcNow.AddHours(UnityEngine.Random.Range(1, 48)),
                    durationHours = 24f,
                    status = AuctionStatus.Active,
                    bidHistory = new List<AuctionBid>()
                });
            }
        }

        #region Listing Management

        public async Task<bool> CreateListing(string itemId, string itemName, int quantity,
            ItemCategory category, ItemRarity rarity, long startingBid, long buyoutPrice, float durationHours)
        {
            if (MyListings.Count >= maxActiveAuctions)
            {
                Debug.LogError("[Auction] Maximum active auctions reached!");
                return false;
            }

            if (!auctionDurations.Contains(durationHours))
            {
                Debug.LogError("[Auction] Invalid auction duration!");
                return false;
            }

            // Calculate deposit
            long deposit = (long)(buyoutPrice * depositPercent / 100);
            // Would deduct deposit from player

            var listing = new AuctionListing
            {
                listingId = Guid.NewGuid().ToString(),
                sellerId = GetCurrentPlayerId(),
                sellerName = GetCurrentPlayerName(),
                itemId = itemId,
                itemName = itemName,
                category = category,
                rarity = rarity,
                quantity = quantity,
                buyoutPrice = buyoutPrice,
                currentBid = startingBid,
                minBidIncrement = Math.Max(100, startingBid / 20),
                deposit = deposit,
                listedAt = DateTime.UtcNow,
                expiresAt = DateTime.UtcNow.AddHours(durationHours),
                durationHours = durationHours,
                status = AuctionStatus.Active,
                bidHistory = new List<AuctionBid>()
            };

            AllListings.Add(listing);
            MyListings.Add(listing);

            await Task.Delay(100);
            OnListingCreated?.Invoke(listing);

            Debug.Log($"[Auction] Listed {itemName} for {buyoutPrice} credits");
            return true;
        }

        public async Task<bool> CancelListing(string listingId)
        {
            var listing = MyListings.Find(l => l.listingId == listingId);
            if (listing == null) return false;

            // Can't cancel if there are bids
            if (listing.bidHistory.Count > 0)
            {
                Debug.LogError("[Auction] Cannot cancel auction with bids!");
                return false;
            }

            listing.status = AuctionStatus.Cancelled;
            AllListings.Remove(listing);
            MyListings.Remove(listing);

            // Return item to player inventory
            // Return deposit

            await Task.Delay(50);
            Debug.Log($"[Auction] Cancelled listing for {listing.itemName}");
            return true;
        }

        #endregion

        #region Buying & Bidding

        public async Task<bool> Buyout(string listingId)
        {
            var listing = AllListings.Find(l => l.listingId == listingId && l.status == AuctionStatus.Active);
            if (listing == null)
            {
                Debug.LogError("[Auction] Listing not found or not active!");
                return false;
            }

            if (listing.sellerId == GetCurrentPlayerId())
            {
                Debug.LogError("[Auction] Cannot buy your own listing!");
                return false;
            }

            // Would check and deduct buyer credits
            long totalCost = listing.buyoutPrice;

            // Process sale
            listing.status = AuctionStatus.Sold;
            listing.highestBidderId = GetCurrentPlayerId();
            listing.highestBidderName = GetCurrentPlayerName();

            // Calculate seller earnings (minus cut)
            long sellerEarnings = listing.buyoutPrice - (long)(listing.buyoutPrice * auctionCutPercent / 100);
            sellerEarnings += listing.deposit; // Return deposit

            // Send item to buyer via mail
            Social.MailSystem.Instance?.SendSystemMail(
                GetCurrentPlayerId(),
                $"Auction Won: {listing.itemName}",
                $"You purchased {listing.quantity}x {listing.itemName} for {listing.buyoutPrice} credits.",
                new List<Social.MailSystem.MailAttachment>
                {
                    new Social.MailSystem.MailAttachment
                    {
                        itemId = listing.itemId,
                        itemName = listing.itemName,
                        quantity = listing.quantity
                    }
                },
                0,
                Social.MailSystem.MailType.AuctionHouse
            );

            // Send credits to seller via mail
            Social.MailSystem.Instance?.SendSystemMail(
                listing.sellerId,
                $"Auction Sold: {listing.itemName}",
                $"Your {listing.itemName} sold for {listing.buyoutPrice} credits. After fees: {sellerEarnings} credits.",
                null,
                sellerEarnings,
                Social.MailSystem.MailType.AuctionHouse
            );

            // Refund outbid players
            foreach (var bid in listing.bidHistory)
            {
                if (bid.bidderId != GetCurrentPlayerId())
                {
                    Social.MailSystem.Instance?.SendSystemMail(
                        bid.bidderId,
                        $"Outbid: {listing.itemName}",
                        $"The item you bid on was purchased. Your bid of {bid.bidAmount} credits has been refunded.",
                        null,
                        bid.bidAmount,
                        Social.MailSystem.MailType.AuctionHouse
                    );
                }
            }

            AllListings.Remove(listing);
            if (listing.sellerId == GetCurrentPlayerId())
            {
                MyListings.Remove(listing);
            }

            await Task.Delay(100);
            OnListingPurchased?.Invoke(listing);

            Debug.Log($"[Auction] Purchased {listing.itemName} for {listing.buyoutPrice}");
            return true;
        }

        public async Task<bool> PlaceBid(string listingId, long bidAmount)
        {
            var listing = AllListings.Find(l => l.listingId == listingId && l.status == AuctionStatus.Active);
            if (listing == null)
            {
                Debug.LogError("[Auction] Listing not found or not active!");
                return false;
            }

            if (listing.sellerId == GetCurrentPlayerId())
            {
                Debug.LogError("[Auction] Cannot bid on your own listing!");
                return false;
            }

            long minBid = listing.currentBid + listing.minBidIncrement;
            if (bidAmount < minBid)
            {
                Debug.LogError($"[Auction] Bid must be at least {minBid}!");
                return false;
            }

            // Refund previous highest bidder
            if (!string.IsNullOrEmpty(listing.highestBidderId))
            {
                var previousBid = listing.bidHistory.LastOrDefault();
                if (previousBid != null)
                {
                    Social.MailSystem.Instance?.SendSystemMail(
                        listing.highestBidderId,
                        $"Outbid: {listing.itemName}",
                        $"You have been outbid on {listing.itemName}. Your bid of {previousBid.bidAmount} credits has been refunded.",
                        null,
                        previousBid.bidAmount,
                        Social.MailSystem.MailType.AuctionHouse
                    );

                    OnOutbid?.Invoke(listing);
                }
            }

            // Would deduct bid amount from player

            var bid = new AuctionBid
            {
                bidId = Guid.NewGuid().ToString(),
                bidderId = GetCurrentPlayerId(),
                bidderName = GetCurrentPlayerName(),
                bidAmount = bidAmount,
                bidTime = DateTime.UtcNow
            };

            listing.bidHistory.Add(bid);
            listing.currentBid = bidAmount;
            listing.highestBidderId = bid.bidderId;
            listing.highestBidderName = bid.bidderName;

            MyBids.Add(bid);

            await Task.Delay(50);
            OnBidPlaced?.Invoke(listing, bid);

            Debug.Log($"[Auction] Placed bid of {bidAmount} on {listing.itemName}");
            return true;
        }

        #endregion

        #region Search & Browse

        public List<AuctionListing> Search(AuctionSearchParams searchParams)
        {
            var results = AllListings.Where(l => l.status == AuctionStatus.Active);

            if (!string.IsNullOrEmpty(searchParams.searchText))
            {
                string search = searchParams.searchText.ToLower();
                results = results.Where(l => l.itemName.ToLower().Contains(search));
            }

            if (searchParams.category.HasValue)
            {
                results = results.Where(l => l.category == searchParams.category.Value);
            }

            if (searchParams.minRarity.HasValue)
            {
                results = results.Where(l => l.rarity >= searchParams.minRarity.Value);
            }

            if (searchParams.minPrice.HasValue)
            {
                results = results.Where(l => l.buyoutPrice >= searchParams.minPrice.Value);
            }

            if (searchParams.maxPrice.HasValue)
            {
                results = results.Where(l => l.buyoutPrice <= searchParams.maxPrice.Value);
            }

            if (searchParams.buyoutOnly)
            {
                results = results.Where(l => l.buyoutPrice > 0);
            }

            // Sort
            results = searchParams.sortBy switch
            {
                AuctionSortBy.TimeRemaining => searchParams.sortDescending
                    ? results.OrderByDescending(l => l.expiresAt)
                    : results.OrderBy(l => l.expiresAt),
                AuctionSortBy.Price => searchParams.sortDescending
                    ? results.OrderByDescending(l => l.buyoutPrice)
                    : results.OrderBy(l => l.buyoutPrice),
                AuctionSortBy.Rarity => searchParams.sortDescending
                    ? results.OrderByDescending(l => l.rarity)
                    : results.OrderBy(l => l.rarity),
                AuctionSortBy.Name => searchParams.sortDescending
                    ? results.OrderByDescending(l => l.itemName)
                    : results.OrderBy(l => l.itemName),
                AuctionSortBy.RecentlyListed => searchParams.sortDescending
                    ? results.OrderBy(l => l.listedAt)
                    : results.OrderByDescending(l => l.listedAt),
                _ => results
            };

            return results.ToList();
        }

        public List<AuctionListing> GetByCategory(ItemCategory category)
        {
            return AllListings.Where(l => l.category == category && l.status == AuctionStatus.Active).ToList();
        }

        public AuctionListing GetListing(string listingId)
        {
            return AllListings.Find(l => l.listingId == listingId);
        }

        #endregion

        #region Auction Processing

        private void ProcessExpiredAuctions()
        {
            var now = DateTime.UtcNow;
            var expired = AllListings.Where(l => l.expiresAt <= now && l.status == AuctionStatus.Active).ToList();

            foreach (var listing in expired)
            {
                if (listing.bidHistory.Count > 0 && !string.IsNullOrEmpty(listing.highestBidderId))
                {
                    // Auction won by highest bidder
                    listing.status = AuctionStatus.Sold;

                    long sellerEarnings = listing.currentBid - (long)(listing.currentBid * auctionCutPercent / 100);
                    sellerEarnings += listing.deposit;

                    // Send item to winner
                    Social.MailSystem.Instance?.SendSystemMail(
                        listing.highestBidderId,
                        $"Auction Won: {listing.itemName}",
                        $"Congratulations! You won the auction for {listing.itemName} with a bid of {listing.currentBid} credits.",
                        new List<Social.MailSystem.MailAttachment>
                        {
                            new Social.MailSystem.MailAttachment
                            {
                                itemId = listing.itemId,
                                itemName = listing.itemName,
                                quantity = listing.quantity
                            }
                        },
                        0,
                        Social.MailSystem.MailType.AuctionHouse
                    );

                    // Send credits to seller
                    Social.MailSystem.Instance?.SendSystemMail(
                        listing.sellerId,
                        $"Auction Sold: {listing.itemName}",
                        $"Your {listing.itemName} sold for {listing.currentBid} credits. After fees: {sellerEarnings} credits.",
                        null,
                        sellerEarnings,
                        Social.MailSystem.MailType.AuctionHouse
                    );
                }
                else
                {
                    // No bids - expired
                    listing.status = AuctionStatus.Expired;

                    // Return item and deposit to seller
                    Social.MailSystem.Instance?.SendSystemMail(
                        listing.sellerId,
                        $"Auction Expired: {listing.itemName}",
                        $"Your auction for {listing.itemName} expired with no bids. Item and deposit returned.",
                        new List<Social.MailSystem.MailAttachment>
                        {
                            new Social.MailSystem.MailAttachment
                            {
                                itemId = listing.itemId,
                                itemName = listing.itemName,
                                quantity = listing.quantity
                            }
                        },
                        listing.deposit,
                        Social.MailSystem.MailType.AuctionHouse
                    );
                }

                AllListings.Remove(listing);
                MyListings.Remove(listing);
                OnAuctionEnded?.Invoke(listing);
            }

            if (expired.Count > 0)
            {
                Debug.Log($"[Auction] Processed {expired.Count} expired auctions");
            }
        }

        #endregion

        private string GetCurrentPlayerId() => "player_local";
        private string GetCurrentPlayerName() => "Player";
    }
}

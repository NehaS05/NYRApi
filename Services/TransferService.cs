using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;
using System.Linq.Expressions;

namespace NYR.API.Services
{
    public class TransferService : ITransferService
    {
        private readonly IVanInventoryRepository _vanInventoryRepository;
        private readonly IRestockRequestRepository _restockRequestRepository;
        private readonly IFollowupRequestRepository _followupRequestRepository;
        private readonly ITransferInventoryRepository _transferInventoryRepository;
        private readonly ILocationInventoryDataRepository _locationInventoryDataRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWarehouseInventoryRepository _warehouseInventoryRepository;
        private readonly IVanRepository _vanRepository;
        private readonly AutoMapper.IMapper _mapper;

        public TransferService(
            IVanInventoryRepository vanInventoryRepository,
            IRestockRequestRepository restockRequestRepository,
            IFollowupRequestRepository followupRequestRepository,
            ITransferInventoryRepository transferInventoryRepository,
            ILocationInventoryDataRepository locationInventoryDataRepository,
            IUserRepository userRepository,
            IWarehouseInventoryRepository warehouseInventoryRepository,
            IVanRepository vanRepository,
            AutoMapper.IMapper mapper)
        {
            _vanInventoryRepository = vanInventoryRepository;
            _restockRequestRepository = restockRequestRepository;
            _followupRequestRepository = followupRequestRepository;
            _transferInventoryRepository = transferInventoryRepository;
            _locationInventoryDataRepository = locationInventoryDataRepository;
            _userRepository = userRepository;
            _warehouseInventoryRepository = warehouseInventoryRepository;
            _vanRepository = vanRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TransferDto>> GetAllTransfersAsync()
        {
            var vanTransfers = await _vanInventoryRepository.GetAllWithDetailsAsync();
            var restockRequests = await _restockRequestRepository.GetAllWithDetailsAsync();
            var followupRequests = await _followupRequestRepository.GetAllWithDetailsAsync();

            var transfers = new List<TransferDto>();

            // Map van transfers
            transfers.AddRange(vanTransfers.Select(vt => new TransferDto
            {
                Id = vt.Id,
                Type = "VanTransfer",
                LocationId = vt.LocationId,
                LocationName = vt.Location?.LocationName ?? "Unknown",
                CustomerId = vt.Location?.CustomerId ?? 0,
                CustomerName = vt.Location?.Customer?.CompanyName ?? "Unknown",
                DeliveryDate = vt.DeliveryDate,
                RequestDate = vt.TransferDate,
                DriverName = vt.DriverName ?? vt.Van?.DefaultDriverName,
                Status = vt.Status,
                TotalItems = vt.Items?.Sum(i => i.Quantity) ?? 0,
                CreatedAt = vt.CreatedAt
            }));

            // Map restock requests
            transfers.AddRange(restockRequests.Select(rr => new TransferDto
            {
                Id = rr.Id,
                Type = "RestockRequest",
                LocationId = rr.LocationId,
                LocationName = rr.Location.LocationName,
                CustomerId = rr.CustomerId,
                CustomerName = rr.Customer.CompanyName,
                DeliveryDate = null,
                RequestDate = rr.RequestDate,
                DriverName = null,
                Status = rr.Status == "Restock Request" ? "Restock Requested" : rr.Status,
                TotalItems = rr.Items?.Sum(i => i.Quantity) ?? 0,
                CreatedAt = rr.CreatedAt
            }));

            // Map followup requests
            transfers.AddRange(followupRequests.Select(fr => new TransferDto
            {
                Id = fr.Id,
                Type = "FollowupRequest",
                LocationId = fr.LocationId,
                LocationName = fr.Location.LocationName,
                CustomerId = fr.CustomerId,
                CustomerName = fr.Customer.CompanyName,
                DeliveryDate = null,
                RequestDate = fr.FollowupDate,
                DriverName = null,
                Status = fr.Status,
                TotalItems = 0,
                CreatedAt = fr.CreatedAt
            }));

            return transfers.OrderByDescending(t => t.RequestDate);
        }

        public async Task<PagedResultDto<TransferDto>> GetAllTransfersPagedAsync(PaginationParamsDto paginationParams)
        {
            var transfers = await GetAllTransfersAsync();
            return CreatePagedTransfers(transfers, paginationParams);
        }

        public async Task<TransferDto?> GetTransferByIdAsync(int id, string type)
        {
            if (type == "VanTransfer")
            {
                var vanTransfer = await _vanInventoryRepository.GetByIdWithDetailsAsync(id);
                if (vanTransfer == null) return null;

                return new TransferDto
                {
                    Id = vanTransfer.Id,
                    Type = "VanTransfer",
                    LocationId = vanTransfer.LocationId,
                    LocationName = vanTransfer.Location?.LocationName ?? "Unknown",
                    CustomerId = vanTransfer.Location?.CustomerId ?? 0,
                    CustomerName = vanTransfer.Location?.Customer?.CompanyName ?? "Unknown",
                    DeliveryDate = vanTransfer.DeliveryDate,
                    RequestDate = vanTransfer.TransferDate,
                    DriverName = vanTransfer.DriverName ?? vanTransfer.Van?.DefaultDriverName,
                    Status = vanTransfer.Status,
                    TotalItems = vanTransfer.Items?.Sum(i => i.Quantity) ?? 0,
                    CreatedAt = vanTransfer.CreatedAt
                };
            }
            else if (type == "RestockRequest")
            {
                var restockRequest = await _restockRequestRepository.GetByIdWithDetailsAsync(id);
                if (restockRequest == null) return null;

                return new TransferDto
                {
                    Id = restockRequest.Id,
                    Type = "RestockRequest",
                    LocationId = restockRequest.LocationId,
                    LocationName = restockRequest.Location.LocationName,
                    CustomerId = restockRequest.CustomerId,
                    CustomerName = restockRequest.Customer.CompanyName,
                    DeliveryDate = null,
                    RequestDate = restockRequest.RequestDate,
                    DriverName = null,
                    Status = restockRequest.Status,
                    TotalItems = restockRequest.Items?.Sum(i => i.Quantity) ?? 0,
                    CreatedAt = restockRequest.CreatedAt
                };
            }

            return null;
        }

        public async Task<IEnumerable<TransferDto>> GetTransfersByLocationIdAsync(int locationId)
        {
            // Get all van transfers and filter by location
            var allVanTransfers = await _vanInventoryRepository.GetAllWithDetailsAsync();
            var vanTransfers = allVanTransfers.Where(vt => vt.LocationId == locationId);
            
            var restockRequests = await _restockRequestRepository.GetByLocationIdAsync(locationId);

            var transfers = new List<TransferDto>();

            transfers.AddRange(vanTransfers.Select(vt => new TransferDto
            {
                Id = vt.Id,
                Type = "VanTransfer",
                LocationId = vt.LocationId,
                LocationName = vt.Location?.LocationName ?? "Unknown",
                CustomerId = vt.Location?.CustomerId ?? 0,
                CustomerName = vt.Location?.Customer?.CompanyName ?? "Unknown",
                DeliveryDate = vt.DeliveryDate,
                RequestDate = vt.TransferDate,
                DriverName = vt.DriverName ?? vt.Van?.DefaultDriverName,
                Status = vt.Status,
                TotalItems = vt.Items?.Sum(i => i.Quantity) ?? 0,
                CreatedAt = vt.CreatedAt
            }));

            transfers.AddRange(restockRequests.Select(rr => new TransferDto
            {
                Id = rr.Id,
                Type = "RestockRequest",
                LocationId = rr.LocationId,
                LocationName = rr.Location.LocationName,
                CustomerId = rr.CustomerId,
                CustomerName = rr.Customer.CompanyName,
                DeliveryDate = null,
                RequestDate = rr.RequestDate,
                DriverName = null,
                Status = rr.Status,
                TotalItems = rr.Items?.Sum(i => i.Quantity) ?? 0,
                CreatedAt = rr.CreatedAt
            }));

            return transfers.OrderByDescending(t => t.RequestDate);
        }

        public async Task<IEnumerable<TransferDto>> GetTransfersByCustomerIdAsync(int customerId)
        {
            var vanTransfers = await _vanInventoryRepository.GetAllWithDetailsAsync();
            var restockRequests = await _restockRequestRepository.GetByCustomerIdAsync(customerId);

            var transfers = new List<TransferDto>();

            // Filter van transfers by customer
            var customerVanTransfers = vanTransfers.Where(vt => vt.Location?.CustomerId == customerId);
            transfers.AddRange(customerVanTransfers.Select(vt => new TransferDto
            {
                Id = vt.Id,
                Type = "VanTransfer",
                LocationId = vt.LocationId,
                LocationName = vt.Location?.LocationName ?? "Unknown",
                CustomerId = vt.Location?.CustomerId ?? 0,
                CustomerName = vt.Location?.Customer?.CompanyName ?? "Unknown",
                DeliveryDate = vt.DeliveryDate,
                RequestDate = vt.TransferDate,
                DriverName = vt.DriverName ?? vt.Van?.DefaultDriverName,
                Status = vt.Status,
                TotalItems = vt.Items?.Sum(i => i.Quantity) ?? 0,
                CreatedAt = vt.CreatedAt
            }));

            transfers.AddRange(restockRequests.Select(rr => new TransferDto
            {
                Id = rr.Id,
                Type = "RestockRequest",
                LocationId = rr.LocationId,
                LocationName = rr.Location.LocationName,
                CustomerId = rr.CustomerId,
                CustomerName = rr.Customer.CompanyName,
                DeliveryDate = null,
                RequestDate = rr.RequestDate,
                DriverName = null,
                Status = rr.Status,
                TotalItems = rr.Items?.Sum(i => i.Quantity) ?? 0,
                CreatedAt = rr.CreatedAt
            }));

            return transfers.OrderByDescending(t => t.RequestDate);
        }

        public async Task<IEnumerable<TransferDto>> GetTransfersByStatusAsync(string status)
        {
            var vanTransfers = await _vanInventoryRepository.GetAllWithDetailsAsync();
            var restockRequests = await _restockRequestRepository.GetAllWithDetailsAsync();

            var transfers = new List<TransferDto>();

            // Filter van transfers by status
            var filteredVanTransfers = vanTransfers.Where(vt => vt.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            transfers.AddRange(filteredVanTransfers.Select(vt => new TransferDto
            {
                Id = vt.Id,
                Type = "VanTransfer",
                LocationId = vt.LocationId,
                LocationName = vt.Location?.LocationName ?? "Unknown",
                CustomerId = vt.Location?.CustomerId ?? 0,
                CustomerName = vt.Location?.Customer?.CompanyName ?? "Unknown",
                DeliveryDate = vt.DeliveryDate,
                RequestDate = vt.TransferDate,
                DriverName = vt.DriverName ?? vt.Van?.DefaultDriverName,
                Status = vt.Status,
                TotalItems = vt.Items?.Sum(i => i.Quantity) ?? 0,
                CreatedAt = vt.CreatedAt
            }));

            // Filter restock requests by status
            var filteredRestockRequests = restockRequests.Where(rr => rr.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            transfers.AddRange(filteredRestockRequests.Select(rr => new TransferDto
            {
                Id = rr.Id,
                Type = "RestockRequest",
                LocationId = rr.LocationId,
                LocationName = rr.Location.LocationName,
                CustomerId = rr.CustomerId,
                CustomerName = rr.Customer.CompanyName,
                DeliveryDate = null,
                RequestDate = rr.RequestDate,
                DriverName = null,
                Status = rr.Status,
                TotalItems = rr.Items?.Sum(i => i.Quantity) ?? 0,
                CreatedAt = rr.CreatedAt
            }));

            return transfers.OrderByDescending(t => t.RequestDate);
        }

        public async Task<IEnumerable<TransferDto>> GetTransfersByTypeAsync(string type)
        {
            var transfers = new List<TransferDto>();

            if (type.Equals("VanTransfer", StringComparison.OrdinalIgnoreCase))
            {
                var vanTransfers = await _vanInventoryRepository.GetAllWithDetailsAsync();
                transfers.AddRange(vanTransfers.Select(vt => new TransferDto
                {
                    Id = vt.Id,
                    Type = "VanTransfer",
                    LocationId = vt.LocationId,
                    LocationName = vt.Location?.LocationName ?? "Unknown",
                    CustomerId = vt.Location?.CustomerId ?? 0,
                    CustomerName = vt.Location?.Customer?.CompanyName ?? "Unknown",
                    DeliveryDate = vt.DeliveryDate,
                    RequestDate = vt.TransferDate,
                    DriverName = vt.DriverName ?? vt.Van?.DefaultDriverName,
                    Status = vt.Status,
                    TotalItems = vt.Items?.Sum(i => i.Quantity) ?? 0,
                    CreatedAt = vt.CreatedAt,
                    LocationAddress = vt.Location.AddressLine1 + ", " + vt.Location.AddressLine2 + ", " + vt.Location.City + ", " + vt.Location.State + ", " + vt.Location.ZipCode
                }));
            }
            else if (type.Equals("RestockRequest", StringComparison.OrdinalIgnoreCase))
            {
                var restockRequests = await _restockRequestRepository.GetAllWithDetailsAsync();
                var followupRequests = await _followupRequestRepository.GetAllWithDetailsAsync();

                transfers.AddRange(restockRequests.Select(rr => new TransferDto
                {
                    Id = rr.Id,
                    Type = "RestockRequest",
                    LocationId = rr.LocationId,
                    LocationName = rr.Location.LocationName,
                    CustomerId = rr.CustomerId,
                    CustomerName = rr.Customer.CompanyName,
                    DeliveryDate = null,
                    RequestDate = rr.RequestDate,
                    DriverName = rr.Location.User?.Name,
                    DriverId = rr.Location.UserId,
                    Status = rr.Status == "Restock Request" ? "Restock Requested" : rr.Status,
                    TotalItems = rr.Items?.Sum(i => i.Quantity) ?? 0,
                    CreatedAt = rr.CreatedAt,
                    LocationAddress = rr.Location.AddressLine1 + ", " + rr.Location.AddressLine2 + ", " + rr.Location.City + ", " + rr.Location.State + ", " + rr.Location.ZipCode
                }));

                transfers.AddRange(followupRequests.Select(fr => new TransferDto
                {
                    Id = fr.Id,
                    Type = "FollowupRequest",
                    LocationId = fr.LocationId,
                    LocationName = fr.Location.LocationName,
                    CustomerId = fr.CustomerId,
                    CustomerName = fr.Customer.CompanyName,
                    DeliveryDate = null,
                    RequestDate = fr.FollowupDate,
                    DriverName = fr.Location.User?.Name,
                    DriverId = fr.Location.UserId,
                    Status = fr.Status,
                    TotalItems = 0,
                    CreatedAt = fr.CreatedAt,
                    LocationAddress = fr.Location.AddressLine1 + ", " + fr.Location.AddressLine2 + ", " + fr.Location.City + ", " + fr.Location.State + ", " + fr.Location.ZipCode
                }));
                
                // Load shipping inventory and location inventory for RestockRequest types
                //foreach (var transfer in transfers.Where(x => x.Type == "RestockRequest"))
                //{
                //    await LoadShippingInventoryForTransfer(transfer);
                //    await LoadLocationInventoryForTransfer(transfer);
                //}
            }

            return transfers.OrderByDescending(t => t.RequestDate);
        }
        
        public async Task<PagedResultDto<TransferDto>> GetTransfersByTypePagedAsync(string type, PaginationParamsDto paginationParams)
        {
            var transfers = await GetTransfersByTypeAsync(type);
            return CreatePagedTransfers(transfers, paginationParams);
        }
        
        private async Task LoadShippingInventoryForTransfer(TransferDto transfer)
        {
            if (transfer.Type == "RestockRequest")
            {
                try
                {
                    // Load RestockRequest by ID to get its items
                    var restockRequest = await _restockRequestRepository.GetByIdWithDetailsAsync(transfer.Id);
                    if (restockRequest != null && restockRequest.Items != null && restockRequest.Items.Any())
                    {
                        // Map RestockRequestItems to TransferInventoryItemDto
                        transfer.ShippingInventory = restockRequest.Items.Select(item => new TransferInventoryItemDto
                        {
                            Id = item.Id,
                            ProductId = item.ProductId,
                            ProductName = item.Product?.Name ?? "Unknown",
                            SkuCode = item.ProductVariant?.BarcodeSKU,
                            ProductVariantId = item.ProductVariantId,
                            VariantName = item.ProductVariant?.VariantName,
                            VariationType = item.ProductVariant?.Attributes.FirstOrDefault()?.Variation.Name,
                            VariationValue = item.ProductVariant?.Attributes.FirstOrDefault()?.VariationOption.Name,
                            Quantity = item.Quantity
                        }).ToList();
                    }
                }
                catch (Exception)
                {
                    // If there's an error loading shipping inventory, just skip it
                    transfer.ShippingInventory = new List<TransferInventoryItemDto>();
                }
            }
        }

        private async Task LoadLocationInventoryForTransfer(TransferDto transfer)
        {
            try
            {
                // Load LocationInventoryData by location ID
                var locationInventoryData = await _locationInventoryDataRepository.GetByLocationIdAsync((int)(transfer?.LocationId));
                if (locationInventoryData != null && locationInventoryData.Any())
                {
                    transfer.LocationInventory = _mapper.Map<List<LocationInventoryDataDto>>(locationInventoryData);
                }
                else
                {
                    transfer.LocationInventory = new List<LocationInventoryDataDto>();
                }
            }
            catch (Exception)
            {
                // If there's an error loading location inventory, just skip it
                transfer.LocationInventory = new List<LocationInventoryDataDto>();
            }
        }

        public async Task<TransferSummaryDto> GetTransfersSummaryAsync()
        {
            var vanTransfers = await _vanInventoryRepository.GetAllWithDetailsAsync();
            var restockRequests = await _restockRequestRepository.GetAllWithDetailsAsync();

            var totalVanTransfers = vanTransfers.Count();
            var totalRestockRequests = restockRequests.Count();
            var totalTransfers = totalVanTransfers + totalRestockRequests;

            var pendingVanTransfers = vanTransfers.Count(vt => vt.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase));
            var pendingRestockRequests = restockRequests.Count(rr => rr.Status.Contains("Request", StringComparison.OrdinalIgnoreCase));
            var pendingTransfers = pendingVanTransfers + pendingRestockRequests;

            var completedVanTransfers = vanTransfers.Count(vt => vt.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase));
            var completedTransfers = completedVanTransfers;

            return new TransferSummaryDto
            {
                TotalVanTransfers = totalVanTransfers,
                TotalRestockRequests = totalRestockRequests,
                TotalTransfers = totalTransfers,
                PendingTransfers = pendingTransfers,
                CompletedTransfers = completedTransfers
            };
        }

        public async Task<InventoryCountsByDriverDto?> GetInventoryCountsByDriverIdAsync(int driverId)
        {
            // Get user by driverId
            var user = await _userRepository.GetByIdAsync(driverId);
            if (user == null)
                return null;

            var result = new InventoryCountsByDriverDto
            {
                DriverId = driverId,
                DriverName = user.Name,
                WarehouseId = user.WarehouseId,
                WarehouseName = user.Warehouse?.Name
            };

            // Get warehouse inventories if user has a warehouse
            if (user.WarehouseId.HasValue)
            {
                var warehouseInventories = await _warehouseInventoryRepository.GetByWarehouseIdAsync(user.WarehouseId.Value);
                result.WarehouseInventories = warehouseInventories.Where(wi => wi.IsActive).Select(wi => new WarehouseInventoryCountDto
                {
                    Id = wi.Id,
                    ProductId = wi.ProductId,
                    ProductName = wi.Product?.Name ?? "Unknown",
                    SkuCode = wi.ProductVariant?.BarcodeSKU,
                    ProductVariantId = wi.ProductVariantId,
                    VariantName = wi.ProductVariant?.VariantName,
                    Quantity = wi.Quantity,
                    Notes = wi.Notes,
                    CreatedAt = wi.CreatedAt
                }).ToList();

                result.TotalWarehouseItems = result.WarehouseInventories.Sum(wi => wi.Quantity);
            }

            // Get van inventories for vans assigned to this driver
            var assignedVans = await _vanRepository.GetByDriverIdAsync(driverId);

            var vanInventoryItems = new List<VanInventoryCountDto>();
            var vanInventories = await _vanInventoryRepository.GetAllWithDetailsAsync();
            foreach (var van in assignedVans)
            {                
                var vanItems = vanInventories
                    .Where(vi => vi.VanId == van.Id && vi.IsActive)
                    .SelectMany(vi => vi.Items.Select(item => new VanInventoryCountDto
                    {
                        Id = item.Id,
                        VanId = vi.VanId,
                        VanName = van.VanName,
                        ProductId = item.ProductId,
                        ProductName = item.Product?.Name ?? "Unknown",
                        SkuCode = item.ProductVariant?.BarcodeSKU,
                        ProductVariantId = item.ProductVariantId,
                        VariantName = item.ProductVariant?.VariantName,
                        Quantity = item.Quantity,
                        Status = vi.Status,
                        CreatedAt = item.CreatedAt
                    }));

                vanInventoryItems.AddRange(vanItems);
            }

            result.VanInventories = vanInventoryItems;
            result.TotalVanItems = result.VanInventories.Sum(vi => vi.Quantity);
            result.TotalItems = result.TotalWarehouseItems + result.TotalVanItems;

            return result;
        }

        private PagedResultDto<TransferDto> CreatePagedTransfers(IEnumerable<TransferDto> transfers, PaginationParamsDto paginationParams)
        {
            PaginationServiceHelper.NormalizePaginationParams(paginationParams);

            var query = transfers.AsQueryable();

            // Apply search
            if (!string.IsNullOrWhiteSpace(paginationParams.Search))
            {
                var search = paginationParams.Search.Trim().ToLower();
                query = query.Where(t =>
                    (t.LocationName ?? string.Empty).ToLower().Contains(search) ||
                    (t.CustomerName ?? string.Empty).ToLower().Contains(search) ||
                    (t.DriverName ?? string.Empty).ToLower().Contains(search) ||
                    (t.Status ?? string.Empty).ToLower().Contains(search));
            }

            var totalCount = query.Count();

            // Apply sorting
            string sortBy = paginationParams.SortBy?.ToLower() ?? string.Empty;
            string sortOrder = paginationParams.SortOrder.ToLower();

            Expression<Func<TransferDto, object>> defaultSort = t => t.DeliveryDate ?? t.RequestDate;

            var sortFields = new Dictionary<string, Expression<Func<TransferDto, object>>>
            {
                { "locationname", t => t.LocationName },
                { "customername", t => t.CustomerName },
                { "deliverydate", t => t.DeliveryDate ?? t.RequestDate },
                { "drivername", t => t.DriverName ?? string.Empty },
                { "status", t => t.Status },
                { "createdat", t => t.CreatedAt }
            };

            if (string.IsNullOrWhiteSpace(sortBy) || !sortFields.ContainsKey(sortBy))
            {
                query = sortOrder == "desc"
                    ? query.OrderByDescending(defaultSort)
                    : query.OrderBy(defaultSort);
            }
            else
            {
                var sortField = sortFields[sortBy];
                query = sortOrder == "desc"
                    ? query.OrderByDescending(sortField)
                    : query.OrderBy(sortField);
            }

            // Apply pagination
            var skip = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
            var data = query.Skip(skip).Take(paginationParams.PageSize).ToList();

            return PaginationServiceHelper.CreatePagedResult(data, totalCount, paginationParams);
        }
    }
}

using NYR.API.Models.DTOs;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class TransferService : ITransferService
    {
        private readonly IVanInventoryRepository _vanInventoryRepository;
        private readonly IRestockRequestRepository _restockRequestRepository;

        public TransferService(
            IVanInventoryRepository vanInventoryRepository,
            IRestockRequestRepository restockRequestRepository)
        {
            _vanInventoryRepository = vanInventoryRepository;
            _restockRequestRepository = restockRequestRepository;
        }

        public async Task<IEnumerable<TransferDto>> GetAllTransfersAsync()
        {
            var vanTransfers = await _vanInventoryRepository.GetAllWithDetailsAsync();
            var restockRequests = await _restockRequestRepository.GetAllWithDetailsAsync();

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
                Status = rr.Status,
                TotalItems = rr.Items?.Sum(i => i.Quantity) ?? 0,
                CreatedAt = rr.CreatedAt
            }));

            return transfers.OrderByDescending(t => t.RequestDate);
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
                    CreatedAt = vt.CreatedAt
                }));
            }
            else if (type.Equals("RestockRequest", StringComparison.OrdinalIgnoreCase))
            {
                var restockRequests = await _restockRequestRepository.GetAllWithDetailsAsync();
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
            }

            return transfers.OrderByDescending(t => t.RequestDate);
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
    }
}

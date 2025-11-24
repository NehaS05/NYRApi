namespace NYR.API.Models.DTOs
{
    // Request models for Spoke API
    public class SpokeRouteOptimizationRequest
    {
        public List<SpokeStop> Stops { get; set; } = new List<SpokeStop>();
        public SpokeVehicle? Vehicle { get; set; }
        public DateTime? StartTime { get; set; }
        public SpokeLocation? StartLocation { get; set; }
        public SpokeLocation? EndLocation { get; set; }
    }

    public class SpokeStop
    {
        public string Id { get; set; } = string.Empty;
        public SpokeLocation Location { get; set; } = new SpokeLocation();
        public int? ServiceTime { get; set; } // in minutes
        public DateTime? TimeWindowStart { get; set; }
        public DateTime? TimeWindowEnd { get; set; }
        public string? Notes { get; set; }
    }

    public class SpokeLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Address { get; set; }
    }

    public class SpokeVehicle
    {
        public string Id { get; set; } = string.Empty;
        public int? Capacity { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    // Response models from Spoke API
    public class SpokeRouteOptimizationResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Id { get; set; }
        public string? Type { get; set; }
        public bool Done { get; set; }
        public OptimizationMetadata? Metadata { get; set; }
        public OptimizationResult? Result { get; set; }
    }

    public class OptimizationMetadata
    {
        public bool Canceled { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public string? StartedBy { get; set; }
        public string? TargetPlanId { get; set; }
    }

    public class OptimizationResult
    {
        public int NumOptimizedStops { get; set; }
        public List<string> SkippedStops { get; set; } = new List<string>();
    }

    public class SpokeOptimizedRoute
    {
        public string RouteId { get; set; } = string.Empty;
        public List<SpokeOptimizedStop> Stops { get; set; } = new List<SpokeOptimizedStop>();
        public double TotalDistance { get; set; } // in miles or km
        public int TotalDuration { get; set; } // in minutes
    }

    public class SpokeOptimizedStop
    {
        public string StopId { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public SpokeLocation Location { get; set; } = new SpokeLocation();
        public DateTime? ArrivalTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public double DistanceFromPrevious { get; set; }
        public int DurationFromPrevious { get; set; }
    }

    public class SpokeRouteMetrics
    {
        public double TotalDistance { get; set; }
        public int TotalDuration { get; set; }
        public int TotalStops { get; set; }
        public double EstimatedCost { get; set; }
    }

    // Tracking models
    public class SpokeTrackingUpdate
    {
        public string RouteId { get; set; } = string.Empty;
        public string StopId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "in_progress", "completed", "failed"
        public SpokeLocation? CurrentLocation { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Notes { get; set; }
    }

    public class SpokeTrackingResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? TrackingUrl { get; set; }
    }

    // Plan creation models
    public class CreateSpokePlanRequest
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public List<string> DriverIds { get; set; } = new List<string>();
    }

    public class SpokePlanResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Depot { get; set; }
        //public DateTime? Starts { get; set; }
    }

    // Stop import models
    public class SpokeStopImport
    {
        public SpokeStopAddress Address { get; set; } = new SpokeStopAddress();
        //public string? CustomerName { get; set; }
        //public string? ContactPhone { get; set; }
        //public string? Notes { get; set; }
        //public int? ServiceTime { get; set; }
        //public DateTime? TimeWindowStart { get; set; }
        //public DateTime? TimeWindowEnd { get; set; }
    }

    public class SpokeStopAddress
    {
        public string AddressLineOne { get; set; } = string.Empty;
        //public string? AddressLineTwo { get; set; }
        //public string? City { get; set; }
        //public string? State { get; set; }
        //public string? ZipCode { get; set; }
        //public string? Country { get; set; }
    }

    public class SpokeStopsBatchImportResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int ImportedCount { get; set; }
        public List<string> stops { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    // Update stop status request
    public class UpdateStopStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class ImportStopsResult
    {
        public List<string> success { get; set; }
        public List<string> failed { get; set; }
    }

    // Driver models
    public class CircuitDriver
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? DisplayName { get; set; }
        public bool Active { get; set; }
        public List<string> Depots { get; set; } = new List<string>();
        public RouteOverrides? RouteOverrides { get; set; }
    }

    public class RouteOverrides
    {
        public TimeOverride? StartTime { get; set; }
        public TimeOverride? EndTime { get; set; }
        public AddressOverride? StartAddress { get; set; }
        public AddressOverride? EndAddress { get; set; }
        public int? MaxStops { get; set; }
        public string? DrivingSpeed { get; set; }
        public string? DeliverySpeed { get; set; }
        public string? VehicleType { get; set; }
    }

    public class TimeOverride
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
    }

    public class AddressOverride
    {
        public string? Address { get; set; }
        public string? AddressLineOne { get; set; }
        public string? AddressLineTwo { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? PlaceId { get; set; }
        public List<string>? PlaceTypes { get; set; }
    }

    public class CircuitDriversApiResponse
    {
        public List<CircuitDriver> Drivers { get; set; } = new List<CircuitDriver>();
        public string? NextPageToken { get; set; }
    }

    public class GetDriversResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<CircuitDriver> Drivers { get; set; } = new List<CircuitDriver>();
        public string? NextPageToken { get; set; }
    }

    // Plan stops models
    public class PlanStop
    {
        public string Id { get; set; } = string.Empty;
        public string? OrderId { get; set; }
        public StopAddress? Address { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientPhone { get; set; }
        public string? RecipientEmail { get; set; }
        public string? Notes { get; set; }
        public List<string>? Packages { get; set; }
        public int? Priority { get; set; }
        public string? Status { get; set; }
        public string? DriverId { get; set; }
        public int? Position { get; set; }
    }

    public class StopAddress
    {
        public string? AddressLineOne { get; set; }
        public string? AddressLineTwo { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class GetPlanStopsResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<PlanStop> Stops { get; set; } = new List<PlanStop>();
        public string? NextPageToken { get; set; }
    }

    public class PlanStopsApiResponse
    {
        public List<PlanStop> Stops { get; set; } = new List<PlanStop>();
        public string? NextPageToken { get; set; }
    }
}

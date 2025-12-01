using ApiContracts.Enums;
using System.Text.RegularExpressions;

namespace Entities;

public class Job
{
    public int jobId { get; set; }
    public int dispatcherId { get; private set; }
    public int driverId { get; private set; } = 0;

    public string Title { get; private set; }
    public string Description { get; private set; }

    public int loaded_miles { get; private set; }
    public int weight_of_cargo { get; private set; }

    public TrailerType type_of_trailer_needed { get; private set; } = TrailerType.dry_van;
    public int total_price { get; private set; }

    public string cargo_info { get; private set; }

    public DateTime pickup_time { get; private set; }
    public DateTime delivery_time { get; private set; }

    public string pickup_location_state { get; private set; }
    public int pickup_location_zip { get; private set; }

    public string drop_location_state { get; private set; }
    public int drop_location_zip { get; private set; }

    public JobStatus current_status { get; private set; } = JobStatus.available;

    public Job() { }

    // ------------------------- BUILDER -------------------------
    public class Builder
    {
        private int _jobId = 1;
        private int _dispatcherId = 2;
        private int _driverId = 0;

        private string _title = "Title";
        private string _description = "Description";

        private int _loadedMiles = 100;
        private int _weight = 1000;

        private TrailerType _trailerType = TrailerType.dry_van;
        private int _totalPrice = 1000;

        private string _cargoInfo = "Cargo Info";

        private DateTime _pickupTime = DateTime.Now;
        private DateTime _deliveryTime = DateTime.Now.AddHours(5);

        private string _pickupState = "AL";
        private int _pickupZip = 35010;

        private string _dropState = "AL";
        private int _dropZip = 35049;

        private JobStatus _status = JobStatus.available;

        public Builder SetId(int id)
        {
            _jobId = id;
            return this;
        }

        public Builder SetDispatcherId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Dispatcher ID must be positive.");
            _dispatcherId = id;
            return this;
        }

        public Builder SetDriverId(int id)
        {
            if (id < 0)
                throw new ArgumentException("Driver ID cannot be negative.");
            _driverId = id;
            return this;
        }

        public Builder SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");
            if (title.Length > 20)
                throw new ArgumentException("Title cannot exceed 20 characters.");
            _title = title;
            return this;
        }

        public Builder SetDescription(string desc)
        {
            if (string.IsNullOrWhiteSpace(desc))
                throw new ArgumentException("Description is required.");
            if (desc.Length > 300)
                throw new ArgumentException("Description cannot exceed 300 characters.");
            _description = desc;
            return this;
        }

        public Builder SetLoadedMiles(int miles)
        {
            if (miles <= 0)
                throw new ArgumentException("Loaded miles must be > 0.");
            _loadedMiles = miles;
            return this;
        }

        public Builder SetWeight(int weight)
        {
            if (weight <= 0)
                throw new ArgumentException("Weight must be > 0.");
            _weight = weight;
            return this;
        }

        public Builder SetTrailerType(TrailerType type)
        {
            _trailerType = type;
            return this;
        }

        public Builder SetTotalPrice(int price)
        {
            if (price <= 0)
                throw new ArgumentException("Price must be > 0.");
            _totalPrice = price;
            return this;
        }

        public Builder SetCargoInfo(string info)
        {
            if (string.IsNullOrWhiteSpace(info))
                throw new ArgumentException("Cargo info is required.");
            if (info.Length > 30)
                throw new ArgumentException("Cargo info cannot exceed 30 characters.");
            _cargoInfo = info;
            return this;
        }

        public Builder SetPickupTime(DateTime time)
        {
            _pickupTime = time;
            return this;
        }

        public Builder SetDeliveryTime(DateTime time)
        {
            if (time <= _pickupTime)
                throw new ArgumentException("Delivery time must be after pickup time.");
            _deliveryTime = time;
            return this;
        }

        public Builder SetPickupState(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("Pickup state is required.");

            if (!Regex.IsMatch(state, "^[A-Za-z]{2}$"))
                throw new ArgumentException("State must be 2 letters.");

            _pickupState = state.ToUpper();
            return this;
        }

        public Builder SetPickupZip(int zip)
        {
            if (zip <= 0 || zip >= 100000)
                throw new ArgumentException("ZIP must be 1–99999.");
            _pickupZip = zip;
            return this;
        }

        public Builder SetDropState(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("Drop state is required.");

            if (!Regex.IsMatch(state, "^[A-Za-z]{2}$"))
                throw new ArgumentException("State must be 2 letters.");

            _dropState = state.ToUpper();
            return this;
        }

        public Builder SetDropZip(int zip)
        {
            if (zip <= 0 || zip >= 100000)
                throw new ArgumentException("ZIP must be 1–99999.");
            _dropZip = zip;
            return this;
        }

        public Builder SetStatus(JobStatus status)
        {
            _status = status;
            return this;
        }

        // --------------------- BUILD ---------------------
        public Job Build()
        {
            return new Job
            {
                jobId = _jobId,
                dispatcherId = _dispatcherId,
                driverId = _driverId,

                Title = _title,
                Description = _description,

                loaded_miles = _loadedMiles,
                weight_of_cargo = _weight,
                type_of_trailer_needed = _trailerType,
                total_price = _totalPrice,
                cargo_info = _cargoInfo,

                pickup_time = _pickupTime,
                delivery_time = _deliveryTime,

                pickup_location_state = _pickupState,
                pickup_location_zip = _pickupZip,

                drop_location_state = _dropState,
                drop_location_zip = _dropZip,

                current_status = _status
            };
        }
    }
}

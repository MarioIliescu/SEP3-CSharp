using ApiContracts.Enums;
using System.Text.RegularExpressions;

namespace Entities;

public class Job
{
    public int JobId { get; set; }
    public int DispatcherId { get; private set; }
    public int DriverId { get; private set; } = 0;

    public string Title { get; private set; }
    public string Description { get; private set; }

    public int Loaded_Miles { get; private set; }
    public int Weight_Of_Cargo { get; private set; }

    public TrailerType Type_Of_Trailer_Needed { get; private set; } = TrailerType.Dry_van;
    public int Total_Price { get; private set; }

    public string Cargo_Info { get; private set; }

    public DateTime Pickup_Time { get; private set; }
    public DateTime Delivery_Time { get; private set; }

    public string Pickup_Location_State { get; private set; }
    public int Pickup_Location_Zip { get; private set; }

    public string Drop_Location_State { get; private set; }
    public int Drop_Location_Zip { get; private set; }

    public JobStatus Current_Status { get; private set; } = JobStatus.Available;

    public Job() { }
    
    public class Builder
    {
        private int _jobId = 0;
        private int _dispatcherId = 0;
        private int _driverId = 0;

        private string _title = "Title";
        private string _description = "Description";

        private int _loadedMiles = 100;
        private int _weight = 1000;

        private TrailerType _trailerType = TrailerType.Dry_van;
        private int _totalPrice = 1000;

        private string _cargoInfo = "Cargo Info";

        private DateTime _pickupTime = DateTime.Now;
        private DateTime _deliveryTime = DateTime.Now.AddHours(5);

        private string _pickupState = "AL";
        private int _pickupZip = 35010;

        private string _dropState = "AL";
        private int _dropZip = 35049;

        private JobStatus _status = JobStatus.Available;

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
            if (time < DateTime.Now && (_status == JobStatus.Available || _status == JobStatus.Assigned))
            {
                SetStatus(JobStatus.Expired);
            }
            _pickupTime = time;
            return this;
        }

        public Builder SetDeliveryTime(DateTime time)
        {
            if (time <= _pickupTime)
            {
                throw new ArgumentException(
                    "Delivery time must be after pickup time.");
            }

            if (time < DateTime.Now && _status != JobStatus.Completed)
            {
                SetStatus(JobStatus.Expired);
            }
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
            if (zip is <= 0 or >= 100000)
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
            if (zip is <= 0 or >= 100000)
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
                JobId = _jobId,
                DispatcherId = _dispatcherId,
                DriverId = _driverId,

                Title = _title,
                Description = _description,

                Loaded_Miles = _loadedMiles,
                Weight_Of_Cargo = _weight,
                Type_Of_Trailer_Needed = _trailerType,
                Total_Price = _totalPrice,
                Cargo_Info = _cargoInfo,

                Current_Status = _status,

                Pickup_Time = _pickupTime,
                Delivery_Time = _deliveryTime,

                Pickup_Location_State = _pickupState,
                Pickup_Location_Zip = _pickupZip,

                Drop_Location_State = _dropState,
                Drop_Location_Zip = _dropZip


            };
        }
    }
}

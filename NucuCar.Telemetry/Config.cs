// ReSharper disable UnusedAutoPropertyAccessor.Global

using NucuCar.Telemetry.Publishers;

namespace NucuCar.Telemetry
{
    public class Config
    {
        /// <summary>
        ///  The Publisher is used by <see cref="PublisherFactory"/> to instantiate
        /// the correct <see cref="BasePublisher"/>. For available types see <see cref="PublisherType"/>
        /// </summary>
        public string Publisher { get; set; }

        public bool ServiceEnabled { get; set; }
        public int PublishInterval { get; set; }
        public string ConnectionString { get; set; }
    }
}
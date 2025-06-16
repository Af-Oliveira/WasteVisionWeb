using System;

namespace DDDSample1.Domain.Detections
{
    public class DetectionDto
    {
        public Guid Id { get; set; }

        public string Description { get; set; }
        public bool? Active { get; set; }


        public DetectionDto()
        {
        }

        public DetectionDto(Guid id, string description, bool? active)
        {
            Id = id;
            Description = description;
            Active = active;
        }
    }
}
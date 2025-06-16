namespace DDDSample1.Domain.Roles
{
    public class CreatingRoleDto
    {
        public string Description { get; set; }


        public CreatingRoleDto(string description)
        {
            this.Description = description;
        }

        
    }
}
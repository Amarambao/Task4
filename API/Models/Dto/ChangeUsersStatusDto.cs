namespace API.Models.Dto
{
    public class ChangeUsersStatusDto
    {
        public bool RequestedStatus { get; set; }
        public IEnumerable<Guid> UserIds { get; set; }
    }
}

namespace PublicModule.Models
{
    public class VMPublicVideoTag
    {
        public int Id { get; set; }
        public virtual VMTag Tag { get; set; } = null!;
        public virtual VMVideo Video { get; set; } = null!;
    }
}

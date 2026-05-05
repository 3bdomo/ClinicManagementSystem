namespace BLL.DTOs.Procedure
{
    public class ProcedureTypeDto
    {
         public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DefaultCost { get; set; }
        public bool IsActive { get; set; }
    }
}


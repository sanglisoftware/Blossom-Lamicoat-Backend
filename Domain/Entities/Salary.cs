namespace Api.Domain.Entities;

public class Salary
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public short? Type { get; set; }
    public int Attendance { get; set; }
    public decimal ExtraHours { get; set; }
    public decimal TotalLate { get; set; }
    public decimal HalfDay { get; set; }
    public decimal TotalSalary { get; set; }
    public DateTime CreatedDate { get; set; }

    public Employee? Employee { get; set; }
}

namespace IntegrationTests.Models;

public class TestAddProductRequest
{
    public string name { get; set; }
    public double cost { get; set; }
    public double weight { get; set; }
    public int productType { get; set; }
    public string creationDate { get; set; }
    public long warehouseId { get; set; }
}
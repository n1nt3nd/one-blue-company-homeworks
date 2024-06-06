namespace hw_3.Services;

public interface IDemandService
{
    Task<int> Calculate(int prediction, int stock);
}
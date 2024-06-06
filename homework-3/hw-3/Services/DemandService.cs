namespace hw_3.Services;

public class DemandService : IDemandService
{
    public async Task<int> Calculate(int prediction, int stock)
    {
        var result = Math.Max(prediction - stock, 0);

        if (Random.Shared.Next() % 2 == 0)
            await Task.Delay(1000);
        else await Task.Delay(1500);
        
        return result;
    }
}
using System;

namespace SleepingBarber
{
    class Program
    {
        static void Main(string[] args)
        {
            TimeSpan haircutTime = new TimeSpan(0, 0, 25);
            TimeSpan haircutVariance = new TimeSpan(0, 0, 5);
            TimeSpan avgTimeToCustomer = new TimeSpan(0, 0, 30);

            TimeSpan timeOfActivity = new TimeSpan(0, 24, 0);
            Barbershop barbershop = new Barbershop(haircutTime, haircutVariance, avgTimeToCustomer);
            barbershop.StartBarbershopActivity(timeOfActivity);
        }
    }
}

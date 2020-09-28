using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace SleepingBarber
{
    class Barbershop
    {
        private Mutex _mut;
        private Stopwatch _worldTime;
        private Stopwatch _sleepingTime;
        private Logger _logger;
        private List<Thread> _customersWaiting;
        private int _quantChairs;
        private TimeSpan _haircutTime;
        private TimeSpan _haircutVariance;
        private TimeSpan _avgTimeToCustomer;
        private int _customersHairsCut;

        public Barbershop(TimeSpan _haircutTime, TimeSpan _haircutVariance, TimeSpan _avgTimeToCustomer, int _quantChairs = 5)
        {
            this._quantChairs = _quantChairs;
            this._haircutTime = _haircutTime;
            this._haircutVariance = _haircutVariance;
            this._avgTimeToCustomer = _avgTimeToCustomer;
            _mut = new Mutex();
            _worldTime = new Stopwatch();
            _sleepingTime = new Stopwatch();
            _customersWaiting = new List<Thread>();
            _customersHairsCut = 0;
            _logger = new Logger($@"C:\Users\Public\Log Barbershop\");
        }

        public void StartBarbershopActivity(TimeSpan timeOfActivity)
        {
            _worldTime.Start();
            _logger.Log("The barbershop opened!");
            _logger.Log("The barber goes to sleep");
            _sleepingTime.Start();
            int second = -1;
            while (_worldTime.ElapsedMilliseconds <= timeOfActivity.TotalMilliseconds)
            {
                int secondInTheWorld = int.Parse(Math.Floor(_worldTime.Elapsed.TotalSeconds).ToString());
                if (second != secondInTheWorld)
                {
                    second = secondInTheWorld;
                    if (CustomerSpawns())
                    {
                        ReceiveNewCustomer();
                    }
                }
            }

            _logger.Log("The barbershop will not receive any more customers");

            while (!_sleepingTime.IsRunning) { }

            _logger.Log("The barbershop closed!");
            _logger.Log($"The barber slept {_sleepingTime.Elapsed.TotalSeconds} seconds");
            _logger.Log($"The barber cut {_customersHairsCut} customers hairs");
        }

        private bool ReceiveNewCustomer()
        {
            _logger.Log("A new customer enters the barbershop");
            if (_customersWaiting.Count >= _quantChairs)
            {
                _logger.Log("There are no chairs available for the customer to sit, therefore he leaves");
                return false;
            }
            Thread customer = new Thread(GetAHairCut);
            _customersWaiting.Add(customer);
            _logger.Log($"Customer [{customer.ManagedThreadId}] sits in a chair");
            customer.Start();
            return true;
        }

        private void GetAHairCut()
        {
            _logger.Log($"Customer [{Thread.CurrentThread.ManagedThreadId}] is waiting to receive a haircut");

            _mut.WaitOne();
            if (_sleepingTime.IsRunning)
            {
                _logger.Log($"Customer [{Thread.CurrentThread.ManagedThreadId}] wakes the barber");
                _sleepingTime.Stop();
            }
            _customersWaiting.Remove(Thread.CurrentThread);

            _logger.Log($"Customer [{Thread.CurrentThread.ManagedThreadId}] is receiving a haircut");
            Thread.Sleep(int.Parse(Math.Floor(TimeToHairCut().TotalMilliseconds).ToString()));
            _logger.Log($"Customer [{Thread.CurrentThread.ManagedThreadId}] received a haircut and leaves the barbershop");

            _customersHairsCut++;

            if (_customersWaiting.Count == 0)
            {
                _logger.Log($"The barber goes back to sleep");
                _sleepingTime.Start();
            }
            _mut.ReleaseMutex();
        }

        private bool CustomerSpawns()
        {
            Random rand = new Random();
            int max = int.Parse(Math.Round(_avgTimeToCustomer.TotalSeconds).ToString());
            int randNum = rand.Next(0, max);
            return randNum == 0;
        }

        private TimeSpan TimeToHairCut()
        {
            Random rand = new Random();
            int min = int.Parse(Math.Round(_haircutTime.TotalMilliseconds - _haircutVariance.TotalMilliseconds).ToString());
            int max = int.Parse(Math.Round(_haircutTime.TotalMilliseconds + _haircutVariance.TotalMilliseconds + 1).ToString());
            int milToHairCut = rand.Next(min, max);
            TimeSpan time = new TimeSpan(0, 0, 0, 0, milToHairCut);

            return time;
        }
    }
}

using System;


namespace App
{
    public class Program
    {
        static void Main(string[] args)
        {
            var order = new Order { Item = "Pizza with extra cheese" };

            var orderingService = new FoodOrderingService();
            var appService = new AppService();
            var mailService = new MailService();

            orderingService.AddSubscriber(appService);
            orderingService.AddSubscriber(mailService);

            orderingService.PrepareOrder(order);

            orderingService.RemoveSubscriber(appService);
            orderingService.PrepareOrder(order);


        }
    }


    public class Order
    {
        public string Item { get; set; }
        public string Ingredients { get; set; }
    }


    public interface IService
    {
        void OnFoodPrepared(object source, FoodPreparedEventArgs eventArgs);
    }


    public class AppService : IService
    {
        public void OnFoodPrepared(object source, FoodPreparedEventArgs eventArgs)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"AppService: your food '{eventArgs.Order.Item}' is prepared.");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public class MailService : IService
    {
        public void OnFoodPrepared(object source, FoodPreparedEventArgs eventArgs)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"MailService: your food '{eventArgs.Order.Item}' is prepared.");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    // 1.
    public class FoodPreparedEventArgs : EventArgs
    {
        public Order Order { get; set; }
    }



    public class FoodOrderingService
    {
        // 2.
        public delegate void FoodPreparedEventHandler(object source, FoodPreparedEventArgs args);
        private event FoodPreparedEventHandler FoodPrepared;

        public void AddSubscriber(IService service)
        {
            FoodPrepared += service.OnFoodPrepared;
        }

        public void RemoveSubscriber(IService service)
        {
            FoodPrepared -= service.OnFoodPrepared;
        }

        // 3.
        protected virtual void OnFoodPrepared(Order order)
        {
            if (FoodPrepared != null)
                FoodPrepared(this, new FoodPreparedEventArgs { Order = order });
        }

        // 4.
        public void PrepareOrder(Order order)
        {
            Console.WriteLine($"Preparing your order '{order.Item}', please wait...");
            Thread.Sleep(1000);

            OnFoodPrepared(order);
        }
    }






}



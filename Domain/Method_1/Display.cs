using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Method_1
{
    public class Display
    {
        public void Show(List<Movie> movies)
        {
            foreach (var item in movies)
            {
                if (item.CreateDate <= DateTime.Now.AddMonths(-6)) //--> repeat expression
                {
                    Console.WriteLine($"{item.Name} Has Dvd");
                }
            }
        }
    }
}

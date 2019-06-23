using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Method_2
{
    public class Display
    {
        public void Show(List<Movie> movies)
        {
            var hasDvd = new HasDvd();
            var isForChild = new IsForChild();
            var finalSpec = hasDvd.And(isForChild);
            foreach (var item in movies)
            {
                if (finalSpec.IsSatisfiedBy(item)) 
                {
                    Console.WriteLine($"{item.Name} is for child and has Dvd");
                }
            }
        }
    }
}

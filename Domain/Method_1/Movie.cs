using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Method_1
{
    //Pass "DRY" Dont repeat yourself
    //Make document code

    //هر فیلم یک رده بندی دارد
    //اگر رده 1و 2 بود کودک حساب می شود و اگر 3 بود بزرگسال

    //حال در واکشی و نمایش اگر بخواهیم فیلم های کودک رابیاوریم باید بگوییم یا رده 1 یا دو

    //در لاجیک هم فیم های کودک یعنی رده یک و دو پنجاه درصد تخفیف دارد

    //فیلم بعد شش ماه می تواند دی وی دی داشته باشد و فروخته شود

    // ==>  یک لاجیک چند جا (نمایش و محاسبه) قیمت استفاده شده

    public enum AgeRange
    {
        Kids,
        Children,
        Adult
    }

    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public AgeRange ageRange { get; set; }
    }

    public class MovieContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("connectionString");
        }
    }

    public interface IMovieRepository
    {
        List<Movie> Search(bool? hasDvd);
    }

    public class MovieRepository : IMovieRepository
    {
        public MovieContext MovieContext { get; set; }

        public MovieRepository(MovieContext movieContext)
        {
            MovieContext = movieContext;
        }
        public List<Movie> Search(bool? hasDvd)
        {
            return MovieContext.Movies.Where(x => !hasDvd.HasValue || (hasDvd.Value && x.CreateDate <= System.DateTime.Now.AddMonths(-6))).ToList();
        }
    }
}

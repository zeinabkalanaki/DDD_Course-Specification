using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Domain.Method_2
{
    //Pass "DRY" Dont repeat yourself
    //Make document code

    public abstract class Specification<T>
    {
        public static Specification<T> All()
        {
            return new AllSepec<T>();
        }

        //##############################################
        public abstract Expression<Func<T, bool>> MyExpression();

        public bool IsSatisfiedBy(T input)
        {
            var predic = MyExpression().Compile();
            return predic(input);
        }
        //##############################################
        public Specification<T> And(Specification<T> right)
        {
            return new AndSepec<T>(this,right);
        }
    }

    internal class AllSepec<T> : Specification<T>
    {
        public override Expression<Func<T, bool>> MyExpression()
        {
            return c => true;
        }
    }

    internal class AndSepec<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public AndSepec(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> MyExpression()
        {
            Expression<Func<T, bool>> leftExpression = _left.MyExpression();
            Expression<Func<T, bool>> rightExpression = _right.MyExpression();

            var param = leftExpression.Parameters[0];
            var paramExpr = Expression.Parameter(param.Type);

            BinaryExpression body = Expression.AndAlso(leftExpression.Body, rightExpression.Body);
            body = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(body);

            var finalExpr = Expression.Lambda<Func<T, bool>>(body, paramExpr);
            return finalExpr;
        }
    }

    internal class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }

        internal ParameterReplacer(ParameterExpression parameter)
        {
            _parameter = parameter;
        }
    }

    //#####################################################################
    public class HasDvd : Specification<Movie>
    {
        public override Expression<Func<Movie, bool>> MyExpression()
        {
            return x => x.CreateDate <= DateTime.Now.AddMonths(-6);
        }
    }

    public class IsForChild : Specification<Movie>
    {
        public override Expression<Func<Movie, bool>> MyExpression()
        {
            return x => x.ageRange == AgeRange.Children || x.ageRange == AgeRange.Kids;
        }
    }

    //#####################################################################
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
        List<Movie> Search(Specification<Movie> spec);
    }

    public class MovieRepository : IMovieRepository
    {
        public MovieContext MovieContext { get; set; }

        public MovieRepository(MovieContext movieContext)
        {
            MovieContext = movieContext;
        }

        public List<Movie> Search(Specification<Movie> specification)
        {
            return MovieContext.Movies.Where(x => specification.IsSatisfiedBy(x)).ToList();
        }
    }
}

using System.Linq.Expressions;
using Stockama.Helper.Utils;

namespace Stockama.Tests.Utils;

public class PredicateBuilderTests
{
   [Fact]
   public void And_ShouldCombineExpressionsWithAndAlso()
   {
      Expression<Func<int, bool>> greaterThanFive = x => x > 5;
      Expression<Func<int, bool>> even = x => x % 2 == 0;

      var combined = greaterThanFive.And(even).Compile();

      Assert.True(combined(6));
      Assert.False(combined(4));
      Assert.False(combined(7));
   }

   [Fact]
   public void Or_ShouldCombineExpressionsWithOrElse()
   {
      Expression<Func<int, bool>> greaterThanFive = x => x > 5;
      Expression<Func<int, bool>> even = x => x % 2 == 0;

      var combined = greaterThanFive.Or(even).Compile();

      Assert.True(combined(6));
      Assert.True(combined(4));
      Assert.True(combined(7));
      Assert.False(combined(3));
   }
}

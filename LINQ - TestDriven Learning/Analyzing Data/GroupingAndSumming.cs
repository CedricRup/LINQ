using System.Linq;
using Models;
using Xunit;

namespace Analyzing_Data
{
    public class GroupingAndSumming
    {
        /* GroupBy 
        - Operator used to group the data and perform analysis
        - Single Property
        - Multiple Properties
        - Parent Property
       */

        [Fact]
        public void GroupingBySingleProperty()
        {
            // GroupBy (Single Property)
            /* Parameters:
            - KeySelector: Defines the key to use for the grouping
            - elementSelector: Defines the values to select from the list
            - resultSelector: Defines the shape or form of the results
            */

            // Comparing the Market share of programming languages that 'DerivedFromC' vs the ones that don't
            var marketShare = ProgrammingLanguageRepository.GetProgrammingLanguages().GroupBy(pg => pg.DerivedFromC, pg => pg.MarketShare, (groupKey, marketShareTotal) => new
            {
                Key = groupKey,
                MarketShare = marketShareTotal.Sum()
            }).ToList();
            Assert.Equal(70, marketShare.First().MarketShare);  // Key = true
            Assert.Equal(30, marketShare.Last().MarketShare);   // Key = false
        }

        [Fact]
        public void GroupingByMultipleProperties()
        {
            // GroupBy (Multiple Properties)
            /* Parameters:
            - KeySelector: Defines the key to use for the grouping, in this case, we create an anonymous Type of two properties
            - elementSelector: Defines the values to select from the list
            - resultSelector: Defines the shape or form of the results
            */

            // Comparing the Market share of programming languages that 'DerivedFromC', grouping by the ones that contain 'C'
            var marketShare = ProgrammingLanguageRepository.GetProgrammingLanguages().GroupBy(
                pg => new
                {
                    pg.DerivedFromC,
                    NameContainsC = pg.Name.Contains('C')

                }, pg => pg.MarketShare, (groupKey, marketShareTotal) => new
                {
                    Key = "Derives From C :" + groupKey.DerivedFromC + " , Name contains 'C' : " + groupKey.NameContainsC,
                    MarketShare = marketShareTotal.Sum()
                }).ToList();

            // Three results (total):
            Assert.Equal("Derives From C :True , Name contains 'C' : True", marketShare.First().Key); //  MarketShare = 39
            Assert.Equal("Derives From C :True , Name contains 'C' : False", marketShare[1].Key); //  MarketShare = 31
            Assert.Equal("Derives From C :False , Name contains 'C' : False", marketShare.Last().Key);  //  MarketShare = 30
        }

        [Fact]
        public void GroupingByParentData()
        {
            // GroupBy (Parent Property)
         
            var programmingLanguages = ProgrammingLanguageRepository.GetProgrammingLanguages().ToList();
            var programmingLanguageTypes = ProgrammingLanguageTypeRepository.GetProgrammingLanguageTypes().ToList();


            var programmingLanguagesTypeQuery = programmingLanguages.Join(programmingLanguageTypes, pg => pg.TypeId,
                pgt => pgt.TypeId,(pl,plt) => new
                {
                    // We save an instance of the programming language to access the properties on the future
                    ProgrammingLanguageInstance = pl,
                    ProgrammingLanguageType = plt
                });


            // Getting the Market share of programming languages, Ordering by programming language Type name
            var programmingLanguagesMarketShare = programmingLanguagesTypeQuery.GroupBy(
                pg => pg.ProgrammingLanguageType, pg => pg.ProgrammingLanguageInstance.MarketShare, (groupKey, marketShareTotal) => new
                {
                    Key = groupKey.Type,
                    MarketShare = marketShareTotal.Sum()
                }).ToList();

            // Three results (total):
            Assert.Equal("Object Oriented", programmingLanguagesMarketShare.First().Key);
            Assert.Equal(77, programmingLanguagesMarketShare.First().MarketShare);

            Assert.Equal("Imperative", programmingLanguagesMarketShare[1].Key);
            Assert.Equal(4, programmingLanguagesMarketShare[1].MarketShare);

            Assert.Equal("Functional", programmingLanguagesMarketShare.Last().Key);
            Assert.Equal(19, programmingLanguagesMarketShare.Last().MarketShare);
        }
    }
}

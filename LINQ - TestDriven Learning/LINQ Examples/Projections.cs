using System.Collections.Generic;
using System.Linq;
using Models;
using Xunit;

namespace LINQ_Examples
{
    public class Projections
    {
        /* Projections */
        // Refers to the operation of transforming an object into a new form
        // The results are projected using the LINQ operators 'Select' or 'SelectMany'
        [Fact]
        public void ProjectwithSelect()
        {
            var programmingLanguages = ProgrammingLanguageRepository.GetProgrammingLanguages().ToList();

            /* Select */
            // Projects values that are based on a transform function, the transformation is often defined with a Lamda expression
            // It projects each item in the sequence into the new Type, in this case, it is a String
            var onlyNames = programmingLanguages.Select(pg => pg.Name).ToList(); 

            Assert.Equal("C#",onlyNames.First());
            Assert.Equal("Ruby",onlyNames.Last());

            // It is possible to Project into an anonymous Type, it can be created directly in the Lamda expression
            var onlyNamesAndRankings = programmingLanguages.Select(pg => 
            // The 'new' keyword defines the anonymous Type
            new
            {
                ProgrammingLanguageName = pg.Name,
                // For single fields, it is not necessary to specify the name of the property if you want to preserve the original name
                pg.Rating
            }).ToList();

            Assert.Equal("C#",onlyNamesAndRankings.First().ProgrammingLanguageName);
            Assert.Equal(10, onlyNamesAndRankings.First().Rating);
            Assert.Equal("Ruby",onlyNamesAndRankings.Last().ProgrammingLanguageName);
            Assert.Equal(7,onlyNamesAndRankings.Last().Rating);
        }

        [Fact]
        public void JoinLists()
        {
            var programmingLanguages = ProgrammingLanguageRepository.GetProgrammingLanguages().ToList();
            var programmingLanguageTypes = ProgrammingLanguageTypeRepository.GetProgrammingLanguageTypes().ToList();

            /* Join */
            // Allows to combine the desired data into one query and tranform the results into a new Type
            // The caller(programmingLanguages) is considered the outer list
            // The parameter(programmingLanguageTypes) is considered the inner list
            /* 
            Parameters:
            - outerKeySelector: Key selector that is used to match the columns on the join from the outer list
            - innerKeySelector: Key selector that is used to match the columns on the join from the inner list
            - resultSelector: Select delegate used for projection
            */
            var programmingLanguagesAndTypes = programmingLanguages.Join(programmingLanguageTypes, pl => pl.TypeId, plt => plt.TypeId, (pl, plt) => new
            {
                pl.Name, plt.Type
            }).ToList();

            Assert.Equal("C#", programmingLanguagesAndTypes.First().Name);
            Assert.Equal("Object Oriented", programmingLanguagesAndTypes.First().Type);
            Assert.Equal("Ruby", programmingLanguagesAndTypes.Last().Name);
            Assert.Equal("Object Oriented", programmingLanguagesAndTypes.First().Type);
        }

        /* Parent/Child Data*/
        // A parent object has a collection of related or child objects
        [Fact]
        public void ProjectParentChildDataWithSelect()
        {
            var programmingLanguages = ProgrammingLanguageRepository.GetProgrammingLanguages().ToList();

            /* Select */
            // The projection is created when defining a search criteria inside a list, which is a property of the parent object
            var programmingLanguegesWithIntTypes = programmingLanguages.Select(pg => pg.ObjectTypes?.Where(ot =>  ot.Name == "Int") ?? new List<ObjectType>()).ToList();

            // Note: When working with parent/child relationships, the use of Select is not optimal since the child does not have information about the parent, since it is an IEnumerable<T>
            Assert.Equal("Int", programmingLanguegesWithIntTypes.First().First().Name);
            Assert.Equal("Int", programmingLanguegesWithIntTypes.Last().First().Name);
        }

        [Fact]
        public void ProjectParentChildDataWithSelectMany()
        {
            var programmingLanguages = ProgrammingLanguageRepository.GetProgrammingLanguages().ToList();

            /* SelectMany */
            // Specialized LINQ operator that allows to easily work with Parent/Child Data
            // Projects multiple sequences based on a transform function and flattens them into one sequence
            /*
            Parameters:
            - Single parameter: Selector, defines the transform function to apply to each element
            - Second paramter: Invokes a result selector function on each element therein, it is defined with a Lamda expression, 
              which has two parameters, the instance of the parent, and the instance of the child, this permits the shaping of data from either sequence
            */
            var programmingLanguagesWithIntTypes = programmingLanguages.SelectMany(pg => pg.ObjectTypes?.Where(ot => ot.Name == "Int") ?? new List<ObjectType>(),(pl,ot) => pl).ToList();

            /* Find the programming languages with an 'Int' Type */
            Assert.Equal("C#", programmingLanguagesWithIntTypes.First().Name);
            Assert.Equal("Ruby", programmingLanguagesWithIntTypes.Last().Name);
        }
    }
}

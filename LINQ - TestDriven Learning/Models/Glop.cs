using System.Collections.Generic;

namespace Models
{
    public static  class Glop
    {
        public static T Quelquechose<T>(this IEnumerable<T> source)
        {
            return default(T);
        }
        
        public static IEnumerable<T> AutreChose<T>(this IEnumerable<T> source)
        {
            return null;
        }
    }
}
using prova.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace prova.Test
{
    class TestQuery : ITestQuery
    {
        IDbStore rethinkDbStore;

        public TestQuery(IDbStore rethinkDbStore)
        {
            this.rethinkDbStore = rethinkDbStore;
        }
        
        public void PrintAuthorStatus()
        {
            List<AuthorStatus> listStatus = rethinkDbStore.GetAuthorsStatus();
            Console.WriteLine();
            Console.WriteLine("Numero di post per ogni autore:");
            Console.WriteLine();
            foreach (var list in listStatus)
            {
                Console.WriteLine(list.ToString());
                Console.WriteLine();
            }
        }
    }
}

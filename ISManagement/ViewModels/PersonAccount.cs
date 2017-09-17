using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISManagement.Models;

namespace ISManagement.ViewModels
{
    public class PersonAccount
    {
        public Person Person { get; set; }
        
        //List of Accounts
        public List<Account> Accounts { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISManagement.Models;

namespace ISManagement.ViewModels
{
    public class AccountTransactions
    {
        public Account Account { get; set; }

        //List of Transactions for this account
        public List<Transaction> Transactions { get; set; }
    }
}
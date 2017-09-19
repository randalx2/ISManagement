using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ISManagement.Models;

namespace ISManagement.Controllers
{
    public class TransactionsController : Controller
    {
        private masterEntities db = new masterEntities();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.Account);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "account_number");
            return View();
        }

        // GET: Transactions/Create/5
        //NB: This accepts the parameter as the AccountId not the PersonId
        [HttpGet]
        public ActionResult Create(int id)
        {
            //Check for at least one transaction under this account
            var transaction = (from u in db.Transactions
                                where u.AccountId == id
                                select u).FirstOrDefault();

            var account = db.Accounts.Find(id);

            if (transaction == null)
            {
                var newTransaction = new Transaction
                {
                    capture_date = DateTime.Now,
                    transaction_date = null,
                    amount = 0,
                    description = "",
                    AccountId = id,
                    Account = account
                };

                ViewBag.AccountId = new SelectList(db.Accounts, "Id", "account_number", newTransaction.AccountId);
                return View(newTransaction);
            }
            else
            {
                transaction.capture_date = DateTime.Now;
                transaction.transaction_date = null;
                transaction.amount = 0;
                transaction.description = "";

                ViewBag.AccountId = new SelectList(db.Accounts, "Id", "account_number", transaction.AccountId);
                return View(transaction);
            }

        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,transaction_date,capture_date,amount,description")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);

                //Update the account outstanding balancing
                var account = (from u in db.Accounts
                              where u.Id == transaction.AccountId
                              select u).FirstOrDefault();

                if (account != null)
                {
                    account.outstanding_balance = account.outstanding_balance + transaction.amount;
                }
                
                db.SaveChanges();
                return RedirectToAction("Details", "Accounts", new { id = transaction.AccountId });
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "account_number", transaction.AccountId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "account_number", transaction.AccountId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,transaction_date,capture_date,amount,description")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;

                //Update the account outstanding balancing
                var account = (from u in db.Accounts
                    where u.Id == transaction.AccountId
                    select u).FirstOrDefault();

                if (account != null)
                {
                    account.outstanding_balance = account.outstanding_balance + transaction.amount;
                }

                db.SaveChanges();
                return RedirectToAction("Details", "Accounts", new {id=transaction.AccountId});
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "account_number", transaction.AccountId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
